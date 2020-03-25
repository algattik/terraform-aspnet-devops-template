output "kubernetes_namespace" {
  value = kubernetes_namespace.build.metadata[0].name
}

output "provider_hub_topic" {
  value = azurerm_eventhub.provider_hub_topic.name
}
