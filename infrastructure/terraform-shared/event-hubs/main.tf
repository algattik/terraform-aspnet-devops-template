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
