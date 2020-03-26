variable "location" {
  type    = string
  description = "Azure region where to create resources."
  default = "North Europe"
}

variable "resource_group" {
  type    = string
  description = "Resource group to deploy in."
}

variable "appname" {
  type = string
  description = "Application name. Use only lowercase letters and numbers"
}

variable "area_name" {
  type = string
  description = "'Area' name to create, name from which resource names and Kubernetes namespace are derived."
}

variable "cosmosdb_account_name" {
  type = string
  description = "The Cosmos DB Account name in which to save results."
}
