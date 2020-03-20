resource "azurerm_user_assigned_identity" "devops" {
  resource_group_name = var.resource_group_name
  location            = var.location
  name                = "agent${var.appname}${var.environment}"
}

resource "azurerm_storage_account" "devops" {
  name                     = "stado${var.appname}${var.environment}"
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_container" "devops" {
  name                  = "content"
  storage_account_name  = azurerm_storage_account.devops.name
  container_access_type = "private"
}

resource "azurerm_storage_blob" "devops" {
  name                   = "devops_agent_init-${md5(file("${path.module}/devops_agent_init.sh"))}.sh"
  storage_account_name   = azurerm_storage_account.devops.name
  storage_container_name = azurerm_storage_container.devops.name
  type                   = "Block"
  source                 = "${path.module}/devops_agent_init.sh"
}

data "azurerm_storage_account_blob_container_sas" "devops_agent_init" {
  connection_string = azurerm_storage_account.devops.primary_connection_string
  container_name    = azurerm_storage_container.devops.name
  https_only        = true

  start  = "2000-01-01"
  expiry = "2099-01-01"

  permissions {
    read   = true
    add    = false
    create = false
    write  = false
    delete = false
    list   = false
  }
}


# Create virtual machine

resource "random_password" "agent_vms" {
  length = 24
  special = true
  override_special = "!@#$%&*()-_=+[]:?"
  min_upper = 1
  min_lower = 1
  min_numeric = 1
  min_special = 1
}

resource "azurerm_linux_virtual_machine_scale_set" "devops" {
  name                  = "vm${var.appname}devops${var.environment}"
  location              = var.location
  resource_group_name   = var.resource_group_name
  network_interface {
    name                      = "nic"
    primary                   = true
    
    ip_configuration {
      name                          = "AzureDevOpsNicConfiguration"
      subnet_id                     = var.subnet_id
    }
  }
  sku                   = var.az_devops_agent_vm_size

  os_disk {
    caching              = "ReadWrite"
    storage_account_type = "Premium_LRS"
  }

  source_image_reference {
    publisher = "Canonical"
    offer     = "UbuntuServer"
    sku       = "16.04.0-LTS"
    version   = "latest"
  }

  admin_username = "azuredevopsuser"
  admin_password = random_password.agent_vms.result

  disable_password_authentication = false

  dynamic "admin_ssh_key" {
    for_each = var.az_devops_agent_sshkeys
    content {
      username = "azuredevopsuser"
      public_key = each.key
    }
  }

  boot_diagnostics {
    storage_account_uri = azurerm_storage_account.devops.primary_blob_endpoint
  }

  identity {
    type = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.devops.id]
  }

  instances = var.az_devops_agent_vm_count

  lifecycle {
    ignore_changes = [
      instances
    ]
  }
}

resource "azurerm_virtual_machine_scale_set_extension" "devops" {
  name                 = "install_azure_devops_agent"
  virtual_machine_scale_set_id = azurerm_linux_virtual_machine_scale_set.devops.id
  publisher            = "Microsoft.Azure.Extensions"
  type                 = "CustomScript"
  type_handler_version = "2.0"

  #timestamp: use this field only to trigger a re-run of the script by changing value of this field.
  #           Any integer value is acceptable; it must only be different than the previous value.
  settings = jsonencode({
    "timestamp" : 1
  })
  protected_settings = jsonencode({
  "fileUris": ["${azurerm_storage_blob.devops.url}${data.azurerm_storage_account_blob_container_sas.devops_agent_init.sas}"],
  "commandToExecute": "bash ${azurerm_storage_blob.devops.name} '${var.az_devops_url}' '${var.az_devops_pat}' '${var.az_devops_agent_pool}' '${var.az_devops_agents_per_vm}'"
  })
}
