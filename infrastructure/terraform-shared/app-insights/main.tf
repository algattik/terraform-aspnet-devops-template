# Application Insights

resource "azurerm_application_insights" "appi" {
  name                = "appi-${var.appname}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = "other"
}
