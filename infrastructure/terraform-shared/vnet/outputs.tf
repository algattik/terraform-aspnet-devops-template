output "name" {
  value = azurerm_virtual_network.main.name
}

output "id" {
  value = azurerm_virtual_network.main.id
}

output "aks_subnet_name" {
  value = azurerm_subnet.aks.name
}

output "aks_subnet_id" {
  value = azurerm_subnet.aks.id
}

output "agents_subnet_id" {
  value = azurerm_subnet.agents.id
}
