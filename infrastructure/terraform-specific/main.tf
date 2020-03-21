# For suggested naming conventions, refer to:
#   https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging

# Resource Group

module "kusto" {
  source = "./kusto"
  kusto_cluster_name = var.kusto_cluster_name
  kusto_database_name = var.kusto_database_name
  resource_group_name = var.resource_group
  location = var.location
}
