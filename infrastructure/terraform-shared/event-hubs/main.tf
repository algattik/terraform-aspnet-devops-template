resource "azurerm_eventhub_namespace" "evh" {
  name                = "evh-${var.appname}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "Standard"
  capacity            = 1
}

resource "azurerm_role_assignment" "app_eventhubs" {
  scope                = azurerm_eventhub_namespace.evh.id
  role_definition_name = "Reader"
  principal_id         = var.app_sp_object_id
}

resource "azurerm_eventhub_namespace_authorization_rule" "providerhub_listen" {
  name                = "listen"
  namespace_name      = azurerm_eventhub_namespace.evh.name
  resource_group_name = var.resource_group_name

  listen = true
}

resource "azurerm_eventhub_namespace_authorization_rule" "providerhub_send" {
  name                = "send"
  namespace_name      = azurerm_eventhub_namespace.evh.name
  resource_group_name = var.resource_group_name

  send   = true
}
