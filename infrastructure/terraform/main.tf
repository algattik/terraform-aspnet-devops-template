# For suggested naming conventions, refer to:
#   https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging

resource "kubernetes_namespace" "build" {
  metadata {
    name = var.area_name
  }
}

resource "azurerm_cosmosdb_sql_database" "sums" {
  name                = var.area_name
  resource_group_name = var.resource_group
  account_name        = var.cosmosdb_account_name
  throughput          = 400
}

resource "azurerm_cosmosdb_sql_container" "sums" {
  name                = "ComputedSums"
  resource_group_name = var.resource_group
  account_name        = var.cosmosdb_account_name
  database_name       = azurerm_cosmosdb_sql_database.sums.name
  partition_key_path  = "/Id"
  throughput          = 400
}
