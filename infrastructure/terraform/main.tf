# For suggested naming conventions, refer to:
#   https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging

#resource "azurerm_kusto_database" "build" {
#  name                = var.kusto_database_name
#  resource_group_name = var.resource_group
#  location            = var.location
#  cluster_name        = var.kusto_cluster_name
#}

resource "kubernetes_namespace" "build" {
  metadata {
    name = var.kubernetes_namespace
  }
}

resource "helm_release" "example" {
  name       = "contoso"
  chart      = "../../charts/contoso"
  namespace  = var.kubernetes_namespace

  wait       = true
  timeout    = 300

  set {
    name  = "image.repository"
    value = var.image_repository
  }
  set {
    name  = "image.tag"
    value = var.image_tag
  }
  set {
    name  = "replicaCount"
    value = 2
  }
  set {
    name  = "service.type"
    value = "NodePort"
  }
  set {
    name  = "settings.adxDefaultDatabaseName"
    value = "$(KUSTO_DB)" 
  }
  set {
    name  = "settings.aadClientId"
    value = var.client_id
  }
  set_sensitive {
    name  = "settings.aadClientSecret"
    value = var.client_secret
  }
  set {
    name  = "settings.aadTenantId"
    value = var.tenant_id
  }
  set {
    name  = "settings.enableQueryLogging"
    value = true 
  }
}
