# Installation

The variables referenced in the following guide refer to the [azure-pipelines.yml](../azure-pipelines.yml) file. To edit them, change the value there, e.g.:

  ```yml
  - name: TERRAFORM_SP_CLIENT_ID
    value: <YOUR VALUE>
  ```

## Azure Active Directory configuration

1. Create service principals for Terraform & AKS:
    - Info: An Azure service principal is a security identity used by user-created apps, services, and automation tools to access specific Azure resources. Think of it as a 'user identity' (login and password or certificate) with a specific role, and tightly controlled permissions to access your resources. We use these to grant access to Azure DevOps and Terraform to deploy and manage resources within our Azure subscription.
    
    - Navigate to the Azure Portal & open the [Cloud Shell](https://docs.microsoft.com/en-us/azure/cloud-shell/overview). You need to create two service principals (Azure Active Directory App registrations) for Terraform and AKS by entering the following commands:

      ```
      az ad sp create-for-rbac --name "YOUR_PROJECT_Azure_DevOps_Terraform_SP" --role owner

      az ad sp create-for-rbac --name "YOUR_PROJECT_AKS_SP" --skip-assignment
      ```

    - Save the outputs of these commands as you'll need these details during the installation. The output comes in the following format:

      ```json
      {
        "appId": "***",
        "displayName": "YOUR_PROJECT_Azure_DevOps_Terraform_SP",
        "name": "http://YOUR_PROJECT_Azure_DevOps_Terraform_SP",
        "password": "***",
        "tenant": "***"
      }
      ```

2. Retrieve and save the object_id for each of the Service principals by using the Cloud Shell: 
      
    - Replace XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX with the appID each of the Service Principals

      ```bash
      az ad sp show --id XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX | jq -r .objectId
      ```

3. Replace the values of the following variables in the [azure-pipelines.yml](../azure-pipelines.yml):

    | name | value |
    |--|--|
    | TERRAFORM_SP_CLIENT_ID | The appId of the Terraform Service Principal. Used by Terraform to deploy resources and assign permissions to the other service principals. |
    | AKS_SP_CLIENT_ID | The appId of the AKS Service Principal. AKS uses it to deploy infrastructure such as network interfaces in VNET subnet. |
    | AKS_SP_OBJECT_ID | The objectId of the AKS Service Principal - from the previous step |
    | APP_SP_CLIENT_ID | Service principal used by the deployed ASP.NET Core application, to access Azure services. |
    | APP_SP_OBJECT_ID | The objectId of the App Service Principal - from the previous step |

## Azure configuration

1. Create the resource group for your project in Azure by running the following command:

    ```bash
    az group create --name YOUR_PROJECT_RG --location westeurope
    ```

1. Grant TERRAFORM_SP_CLIENT_ID *Owner* permission on the RG. To do this, replace the values in assignee and scope parameters accordingly. The scope parameter is the id parameter from your previously created resource group.

    ```bash
    az role assignment create --assignee TERRAFORM_SP_CLIENT_ID --role Owner --scope /subscriptions/YourSubscriptionId/resourceGroups/YOUR_PROJECT_RG
    ```

1. Create the storage account $(TERRAFORM_STORAGE_ACCOUNT) within the RG. Info: Storage account name must be between 3 and 24 characters in length and use numbers and lower-case letters only.

    ```bash
    az storage account create --name yourprojectstorageaccount --resource-group YOUR_PROJECT_RG
    ```

1. Create the container "terraformstate" within the storage account

    ```bash
    az storage container create --name terraformstate --account-name yourprojectstorageaccount
    ```

1. Replace the values of the following variables in the [azure-pipelines.yml](../azure-pipelines.yml):

    | name | value |
    |---|---|
    | RESOURCE_GROUP | The name of the resource group |
    | TERRAFORM_STORAGE_ACCOUNT | The name of the storage account |


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
    
    1. Go to Azure DevOps Project settings
    2. Select Service Connections (if you want to learn more about this, got to [Service connections in Azure docs](https://docs.microsoft.com/en-us/azure/devops/pipelines/library/service-endpoints?view=azure-devops&tabs=yaml))
    3. Create a new Service connection
    4. Pick Azure Resource Manager (which gives access to Azure Resources commands)
    5. Create a new Azure service connection:
      - Select Service Principal (automatic)
      - Don't select a resource group, as we don't have one yet - this allows the SP to access all Resource Groups
      - Give the Service connection a name - we will need that name for Terraform later (e.g. Terraform_SP)
      - Check the "Grant access permission to all pipelines" box

  - Run the pipeline azure-pipelines.yml
  - On the first run some of the jobs will fail with error
  
          ##[error]No agents were found in pool $AGENT_POOL_NAME. Configure an agent for the pool and try again.

    this is normal, check that the infra deployment jobs succeeded, and rerun.
