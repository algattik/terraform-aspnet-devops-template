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

variable "instrumentation_key" {
  type = string
  description = "App Insights instrumentation key to send metrics to."
}

variable "cosmos_db_account_name" {
  type = string
  description = "Cosmos DB account in which to save data."
}

variable "cosmos_db_collection" {
  type = string
  description = "Cosmos DB collection in which to save data."
}
