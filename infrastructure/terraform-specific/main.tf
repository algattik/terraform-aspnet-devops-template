# For suggested naming conventions, refer to:
#   https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging

#resource "azurerm_kusto_database" "build" {
#  name                = var.kusto_database_name
#  resource_group_name = var.resource_group
#  location            = var.location
#  cluster_name        = var.kusto_cluster_name
#}

resource "kubernetes_namespace" "build" {
  metadata {
    name = var.kubernetes_namespace
  }
}
