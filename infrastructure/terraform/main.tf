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


data "helm_repository" "stable" {
  name = "stable"
  url  = "https://kubernetes-charts.storage.googleapis.com"
}

resource "helm_release" "prometheus" {
  name       = "prometheus-operator"
  chart      = "stable/prometheus-operator"
  namespace  = var.kubernetes_namespace

  wait       = true
  timeout    = 300

  # Workaround for https://github.com/helm/charts/issues/19452
  set {
    name  = "prometheusOperator.enabled"
    value = var.kubernetes_namespace == "promettmp"
  }
  set {
    name  = "prometheusOperator.createCustomResource"
    value = false
  }
}
