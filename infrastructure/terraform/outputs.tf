output "kubernetes_namespace" {
  value = kubernetes_namespace.build.metadata[0].name
}

output "cosmosdb_container_id" {
  value = azurerm_cosmosdb_sql_container.sums.id
}
