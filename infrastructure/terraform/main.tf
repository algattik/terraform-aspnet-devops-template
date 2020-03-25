# For suggested naming conventions, refer to:
#   https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging

resource "kubernetes_namespace" "build" {
  metadata {
    name = var.area_name
  }
}

resource "azurerm_eventhub" "build" {
  name                = "providers"
  namespace_name      = var.eventhub_namespace
  resource_group_name = var.resource_group
  partition_count     = 32
  message_retention   = 1 # days (1-7)
}
