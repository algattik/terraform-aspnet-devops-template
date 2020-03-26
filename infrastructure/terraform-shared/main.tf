# For suggested naming conventions, refer to:
#   https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging

module "acr" {
  source = "./acr"
  acr_name = var.acr_name
  resource_group_name = var.resource_group
  location = var.location
  aks_sp_object_id = var.aks_sp_object_id
}

module "vnet" {
  source = "./vnet"
  appname = var.appname
  environment = var.environment
  resource_group_name = var.resource_group
  location = var.location
}

module "devops-agent" {
  source = "./devops-agent"
  appname = var.appname
  environment = var.environment
  location = var.location
  resource_group_name = var.resource_group
  subnet_id = module.vnet.agents_subnet_id
  az_devops_url = var.az_devops_url
  az_devops_pat = var.az_devops_pat
  az_devops_agent_pool = var.az_devops_agent_pool
  az_devops_agents_per_vm = var.az_devops_agents_per_vm
  az_devops_agent_sshkeys = var.az_devops_agent_sshkeys
  az_devops_agent_vm_size = var.az_devops_agent_vm_size
}

module "aks" {
  source = "./aks"
  appname = var.appname
  environment = var.environment
  aks_version = var.aks_version
  resource_group_name = var.resource_group
  location = var.location
  subnet_id = module.vnet.aks_subnet_id
  aks_sp_client_id = var.aks_sp_client_id
  aks_sp_object_id = var.aks_sp_object_id
  aks_sp_client_secret = var.aks_sp_client_secret
}

module "cosmosdb" {
  source = "./cosmosdb"
  appname = var.appname
  environment = var.environment
  resource_group_name = var.resource_group
  location = var.location
  app_sp_object_id = var.app_sp_object_id
}

module "app-insights" {
  source = "./app-insights"
  appname = var.appname
  environment = var.environment
  resource_group_name = var.resource_group
  location = var.location
}
