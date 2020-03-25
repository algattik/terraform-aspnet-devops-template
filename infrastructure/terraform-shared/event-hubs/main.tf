resource "azurerm_eventhub_namespace" "evh" {
  name                = "evh-${var.appname}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "Standard"
  capacity            = 1
}
