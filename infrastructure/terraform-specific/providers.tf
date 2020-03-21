#Set the terraform required version

terraform {
  required_version = ">= 0.12.6"
}

# Configure the Azure Provider

provider "azurerm" {
  # It is recommended to pin to a given version of the Provider
  version = "=2.2.0"
  skip_provider_registration = true
  features {}
}

provider "kubernetes" {
}

provider "helm" {
}
