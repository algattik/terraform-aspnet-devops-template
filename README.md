Installation:

- Create the RG $(RESOURCE_GROUP)
- Create the storage account $(TERRAFORM_STORAGE_ACCOUNT) within the RG
- Create the container "terraformstate" within the storage account
- Create a service principal $(TERRAFORM_SP_CLIENT_ID)
- Grant TERRAFORM_SP_CLIENT_ID *Owner* permission on the RG
- Create an ADO agent pool named $(AGENT_POOL_NAME)
- Create a service principal $(AKS_SP_CLIENT_ID)
- Enter the SP Object ID in $(AKS_SP_OBJECT_ID)
- Create a VG named terraform-aspnet-devops-template and create the variables
  - APP_NAME: short globally unique name, e.g. "aspnetmplt000010"
  - AGENT_POOL_MANAGEMENT_TOKEN: create an ADO PAT with Agent Manage permission
  - AKS_SP_CLIENT_SECRET: secret for AKS_SP_CLIENT_ID
  - TERRAFORM_SP_CLIENT_SECRET: secret for TERRAFORM_SP_CLIENT_SECRET
- Install ADO extensions:
  - Secure dev tools: https://marketplace.visualstudio.com/acquisition?itemName=securedevelopmentteam.vss-secure-development-tools
  - Terraform: https://marketplace.visualstudio.com/items?itemName=ms-devlabs.custom-terraform-tasks
  - JMeter: https://marketplace.visualstudio.com/items?itemName=AlexandreGattiker.jmeter-tasks
- Run the pipeline azure-pipelines.yml
- On the first run some of the jobs will fail with error

        ##[error]No agents were found in pool $AGENT_POOL_NAME. Configure an agent for the pool and try again.

  this is normal, check that the infra deployment jobs succeeded, and rerun.
