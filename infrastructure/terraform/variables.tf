variable "location" {
  type    = string
  description = "Azure region where to create resources."
  default = "North Europe"
}

variable "resource_group" {
  type    = string
  description = "Resource group to deploy in."
}

variable "area_name" {
  type = string
  description = "'Area' name to create, name from which resource names and Kubernetes namespace are derived."
}
