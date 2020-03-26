resource "azurerm_cosmosdb_account" "db" {
  name                = "cosmos-${var.appname}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  offer_type          = "Standard"

  consistency_policy {
    consistency_level       = "BoundedStaleness"
  }

  geo_location {
    location          = var.location
    failover_priority = 0
  }
}

resource "azurerm_role_assignment" "app_cosmosdb" {
  scope                = azurerm_cosmosdb_account.db.id
  role_definition_name = "DocumentDB Account Contributor"
  principal_id         = var.app_sp_object_id
}
