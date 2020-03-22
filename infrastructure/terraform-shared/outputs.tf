output "subscription_id" {
  value = data.azurerm_client_config.current.subscription_id
}

output "vnet_name" {
  value = module.vnet.name
}

output "vnet_id" {
  value = module.vnet.id
}

output "agent_pool_name" {
  value = var.az_devops_agent_pool
}

output "agent_vmss_name" {
  value = module.devops-agent.agent_vmss_name
}

output "aks_name" {
  value = module.aks.name
}

output "kusto_name" {
  value = module.kusto.name
}

output "kube_config_base64" {
  value     = base64encode(module.aks.kube_config_raw)
  sensitive = true
}

output "kubernetes_version" {
  value = module.aks.kubernetes_version
}

output "kusto_uri" {
  value = module.kusto.uri
}
