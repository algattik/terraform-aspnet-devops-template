resource "azurerm_kusto_database" "database" {
  name                = var.kusto_database_name
  resource_group_name = var.resource_group_name
  location            = var.location
  cluster_name        = var.kusto_cluster_name
}
