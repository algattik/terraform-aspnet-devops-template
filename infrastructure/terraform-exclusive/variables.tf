variable "location" {
  type    = string
  description = "Azure region where to create resources."
  default = "North Europe"
}

variable "resource_group" {
  type    = string
  description = "Resource group to deploy in."
}

variable "kusto_name" {
  type = string
  description = "Name of the Kusto cluster."
}
