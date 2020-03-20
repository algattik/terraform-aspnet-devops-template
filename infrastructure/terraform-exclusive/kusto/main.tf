resource "azurerm_kusto_database" "database" {
  name                = "my-kusto-database"
  resource_group_name = var.resource_group_name
  location            = var.location
  cluster_name        = var.kusto_name
}
