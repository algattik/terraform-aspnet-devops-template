variable "kubernetes_namespace" {
  type = string
  description = "Kubernetes namespace to create."
}

variable "release_name" {
  type = string
}

variable "image_repository" {
  type = string
}

variable "image_tag" {
  type = string
}

variable "client_id" {
  type = string
}

variable "client_secret" {
  type = string
}

variable "tenant_id" {
  type = string
}

variable "eventhubs_namespace" {
  type = string
  description = "Event Hubs namespace to connect to."
}

variable "instrumentation_key" {
  type = string
  description = "App Insights instrumentation key to send metrics to."
}

variable "provider_hub_send" {
  type = string
  description = "ARM authorization rule resource ID for sending messages to Provider Events Hub."
}

variable "provider_hub_topic" {
  type = string
  description = "Provider Events Hub topic to send messages to."
}
