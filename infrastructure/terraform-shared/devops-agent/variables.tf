variable "appname" {
  type = string
}

variable "environment" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

variable "subnet_id" {
  type = string
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
}

variable "az_devops_agent_admin_user" {
  type        = string
  default     = "azuredevopsuser"
  description = "Username of the admin user on the agent VMs"
}

variable "az_devops_agent_sshkeys" {
  type        = list(string)
  description = "Optionally provide ssh public key(s) to logon to the VM"
}

variable "az_devops_agent_vm_size" {
  type    = string
  description = "Specify the size of the VM"
}

variable "az_devops_agents_per_vm" {
  type = number
  description = "Number of Azure DevOps agents spawned per VM. Agents will be named with a random prefix."
  default = 4
}
