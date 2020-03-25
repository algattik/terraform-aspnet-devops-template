output "eventhubs_namespace" {
  value = azurerm_eventhub_namespace.evh.name
}

output "provider_hub_listen" {
  value = azurerm_eventhub_namespace_authorization_rule.providerhub_listen.id
}

output "provider_hub_send" {
  value = azurerm_eventhub_namespace_authorization_rule.providerhub_send.id
}
