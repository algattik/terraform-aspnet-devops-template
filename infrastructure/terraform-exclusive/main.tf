# For suggested naming conventions, refer to:
#   https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging

# Resource Group

module "kusto" {
  source = "./kusto"
  kusto_name = var.kusto_name
  resource_group_name = var.resource_group
  location = var.location
}
