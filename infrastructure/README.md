# Terraform templates

In the Azure DevOps pipeline, the `terraform_backend` directory is merged into the `terraform` directory
before running apply, so that state is maintained in Azure Storage. Deployed resources are named with
the environment `cd`, for example `vnet-contoso-cd`.

In local development, a local state store is used, and resources are named by default with the environment `dev`,
for example `vnet-contoso-dev`.

The Azure DevOps pipeline is based on the [Terraform Azure DevOps Starter](https://github.com/microsoft/terraform-azure-devops-starter),
but simplified to remove a separate "terraform plan" stage and manual approvals. It requires installing the
[Terraform extension for Azure DevOps](https://marketplace.visualstudio.com/items?itemName=ms-devlabs.custom-terraform-tasks).

# Using bash instead of Terraform extension because of following issues:
# - https://github.com/microsoft/azure-pipelines-extensions/issues/748
# - https://github.com/microsoft/azure-pipelines-extensions/issues/725
# - https://github.com/microsoft/azure-pipelines-extensions/issues/747
