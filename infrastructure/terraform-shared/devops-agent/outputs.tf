output "agent_vmss_id" {
  value = azurerm_linux_virtual_machine_scale_set.devops.id
}

output "agent_vmss_name" {
  value = azurerm_linux_virtual_machine_scale_set.devops.name
}
