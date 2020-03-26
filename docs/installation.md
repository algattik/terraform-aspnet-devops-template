# Installation

Pick and update the $() values in azure-pipelines.yml as needed.

## Create service principals

Start by creating the following service principals:

| Service principal      | Purpose                                                                                                                    |
|------------------------|----------------------------------------------------------------------------------------------------------------------------|
| TERRAFORM_SP_CLIENT_ID | Owner on the Resource Group, used by Terraform to deploy resources and assign permissions to the other service principals. |
| AKS_SP_CLIENT_ID       | Service principal for AKS itself. AKS uses it to deploy infrastructure such as network interfaces in VNET subnet.          |
| APP_SP_CLIENT_ID       | Service principal used by the deployed ASP.NET Core application, to access Azure services.                                 |

Steps:
  - Create a service principal $(TERRAFORM_SP_CLIENT_ID)
  - Create a service principal $(AKS_SP_CLIENT_ID)
  - Enter the SP Object ID in $(AKS_SP_OBJECT_ID)
    *Should be object IDs of service principals, not object IDs of the application nor application IDs.
    To retrieve, navigate in the AAD portal from an App registration to "Managed application in local directory", or use Azure Cloud Shell / Azure CLI command `az ad sp show --id $AKS_SP_CLIENT_ID --query objectId`*
  - Do the same for service principal $(APP_SP_CLIENT_ID) and SP Object ID in $(APP_SP_OBJECT_ID).

## Create Azure resource group and Terraform state backend storage account

In Azure:
  - Create the RG $(RESOURCE_GROUP)
  - Grant TERRAFORM_SP_CLIENT_ID *Owner* permission on the RG
  - Create the storage account $(TERRAFORM_STORAGE_ACCOUNT) within the RG
  - Create the container "terraformstate" within the storage account

## Create Azure DevOps pipeline

In Azure DevOps:
  - Create an ADO agent pool named $(AGENT_POOL_NAME)
  - Create an Azure DevOps Variable Group named `terraform-aspnet-devops-template` and create the variables
    - APP_NAME: short globally unique name, e.g. "aspnetmplt000010"
    - AGENT_POOL_MANAGEMENT_TOKEN: create an ADO PAT with Agent Manage permission
    - AKS_SP_CLIENT_SECRET: secret for AKS_SP_CLIENT_ID
    - TERRAFORM_SP_CLIENT_SECRET: secret for TERRAFORM_SP_CLIENT_ID
  - Install ADO extensions:
    - Secure dev tools: https://marketplace.visualstudio.com/acquisition?itemName=securedevelopmentteam.vss-secure-development-tools
    - Terraform: https://marketplace.visualstudio.com/items?itemName=ms-devlabs.custom-terraform-tasks
    - JMeter: https://marketplace.visualstudio.com/items?itemName=AlexandreGattiker.jmeter-tasks
  - Create a new Azure Resource Manager Service Connection $(TERRAFORM_SERVICE_CONNECTION) with access to the $(RESOURCE_GROUP) resource group.
  - Run the pipeline azure-pipelines.yml
  - On the first run some of the jobs will fail with error
  
          ##[error]No agents were found in pool $AGENT_POOL_NAME. Configure an agent for the pool and try again.

    this is normal, check that the infra deployment jobs succeeded, and rerun.
