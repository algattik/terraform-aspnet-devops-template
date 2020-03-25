#aInstallation

Pick and update the $() values in azure-pipelines.yml as needed.

In Azure AD:
  - Create a service principal $(TERRAFORM_SP_CLIENT_ID)
  - Create a service principal $(AKS_SP_CLIENT_ID)
  - Enter the SP Object ID in $(AKS_SP_OBJECT_ID)
    *Should be object IDs of service principals, not object IDs of the application nor application IDs.
    To retrieve, navigate in the AAD portal from an App registration to "Managed application in local directory".*

In Azure:
  - Create the RG $(RESOURCE_GROUP)
  - Grant TERRAFORM_SP_CLIENT_ID *Owner* permission on the RG
  - Create the storage account $(TERRAFORM_STORAGE_ACCOUNT) within the RG
  - Create the container "terraformstate" within the storage account
  - Enter the subscription ID and tenant ID of the resource group respectively in $(ARM_SUBSCRIPTION_ID) and $(ARM_TENANT_ID)

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
