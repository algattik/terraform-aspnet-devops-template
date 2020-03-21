output "subscription_id" {
  value = data.azurerm_client_config.current.subscription_id
}

output "vnet_id" {
  value = module.vnet.id
}

output "agent_pool_name" {
  value = var.az_devops_agent_pool
}

output "agent_vmss_id" {
  value = module.devops-agent.agent_vmss_id
}

output "aks_id" {
  value = module.aks.id
}

output "kube_config_raw" {
  value     = module.aks.kube_config_raw
  sensitive = true
}

output "kubernetes_version" {
  value = module.aks.kubernetes_version
}

output "kusto_name" {
  value = module.kusto.name
}

output "kusto_uri" {
  value = module.kusto.uri
}
