output "kubernetes_namespace" {
  value = kubernetes_namespace.build.metadata[0].name
}
