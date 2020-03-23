variable "appname" {
  type = string
  description = "Application name. Use only lowercase letters and numbers"
}

variable "environment" {
  type    = string
  description = "Environment name, e.g. 'dev' or 'cd'"
  default = "dev"
}

variable "location" {
  type    = string
  description = "Azure region where to create resources."
  default = "West Europe"
}

variable "resource_group" {
  type    = string
  description = "Resource group to deploy in."
}

variable "az_devops_url" {
  type = string
  description = "Specify the Azure DevOps url e.g. https://dev.azure.com/myorg"
}

variable "az_devops_pat" {
  type = string
  description = "Provide a Personal Access Token (PAT) for Azure DevOps. Create it at https://dev.azure.com/[Organization]/_usersSettings/tokens with permission Agent Pools > Read & manage"
}

variable "az_devops_agent_pool" {
  type = string
  description = "Specify the name of the agent pool - must exist before. Create it at https://dev.azure.com/[Organization]/_settings/agentpools"
  default = "pool001"
}

variable "az_devops_agent_sshkeys" {
  type        = list(string)
  description = "Optionally provide ssh public key(s) to logon to the VM"
  default     = []
}

variable "az_devops_agent_vm_size" {
  type    = string
  description = "Specify the size of the VM"
  default = "Standard_D2s_v3"
}

variable "az_devops_agents_per_vm" {
  type = number
  description = "Number of Azure DevOps agents spawned per VM. Agents will be named with a random prefix."
  default = 4
}

variable "acr_name" {
  type = string
  description = "Name of the generated Azure Container Registry instance."
}

variable "aks_version" {
  type = string
  description = "Kubernetes version of the AKS cluster."
}

variable "aks_sp_client_id" {
  type = string
  description = "Service principal client ID for the Azure Kubernetes Service cluster identity."
}

variable "aks_sp_object_id" {
  type = string
  description = "Service principal object ID for the Azure Kubernetes Service cluster identity. Should be object IDs of service principals, not object IDs of the application nor application IDs. To retrieve, navigate in the AAD portal from an App registration to 'Managed application in local directory'."
}

variable "aks_sp_client_secret" {
  type = string
  description = "Service principal client secret for the Azure Kubernetes Service cluster identity."
}

variable "kusto_admin_sp_object_id" {
  type = string
  description = "Service principal object ID for the principal to be granted Contributor permissions on the Kusto cluster."
}
