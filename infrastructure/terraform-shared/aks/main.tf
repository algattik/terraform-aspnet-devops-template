# Application Insights

resource "random_id" "workspace" {
  keepers = {
    # Generate a new id each time we switch to a new resource group
    group_name = var.resource_group_name
  }

  byte_length = 8
}

resource "azurerm_application_insights" "aks" {
  name                = "appi-${var.appname}-${var.environment}-aks"
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = "other"
}

# Subnet permission

resource "azurerm_role_assignment" "aks_subnet" {
  scope                = var.subnet_id
  role_definition_name = "Network Contributor"
  principal_id         = var.aks_sp_object_id
}

# Kubernetes Service

resource "azurerm_kubernetes_cluster" "aks" {
  name                = "aks-${var.appname}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  dns_prefix          = "aks-${var.appname}-${var.environment}"
  kubernetes_version  = var.aks_version

  default_node_pool {
    type                  = "VirtualMachineScaleSets"
    name                  = "default"
    node_count            = 4
    vm_size               = "Standard_D2s_v3"
    os_disk_size_gb       = 30
    vnet_subnet_id        = var.subnet_id
    enable_auto_scaling   = true
    max_count             = 15
    min_count             = 3
  }

  lifecycle {
    ignore_changes = [
      default_node_pool.0.node_count,
    ]
  }

  addon_profile {
   oms_agent {
     enabled                    = true
     log_analytics_workspace_id = azurerm_application_insights.aks.id
    }
  }

  service_principal {
    client_id     = var.aks_sp_client_id
    client_secret = var.aks_sp_client_secret
  }

  depends_on = [
    azurerm_role_assignment.aks_subnet
  ]
}
