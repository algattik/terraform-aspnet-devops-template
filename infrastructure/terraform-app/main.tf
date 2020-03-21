resource "helm_release" "build" {
  name       = var.release_name
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
