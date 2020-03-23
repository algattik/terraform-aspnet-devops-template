https://marketplace.visualstudio.com/acquisition?itemName=securedevelopmentteam.vss-secure-development-tools
https://marketplace.visualstudio.com/items?itemName=ms-devlabs.custom-terraform-tasks

- Serilog

- Create the RG $(RESOURCE_GROUP)
- Create the storage account $(TERRAFORM_STORAGE_ACCOUNT) within the RG
- Create the container "terraformstate" within the storage account
- Create a service principal $(TERRAFORM_SP_CLIENT_ID)
- Grant TERRAFORM_SP_CLIENT_ID *Owner* permission on the RG
- Create an ADO agent pool named $(AGENT_POOL_NAME)

