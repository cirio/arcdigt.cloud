using Pulumi;
using Pulumi.Azure.AppService;
using Azure = Pulumi.Azure; //based on Terraform
using AzureNative = Pulumi.AzureNative; //based on Open API di Azure Resource Manager

class DevStack : Stack
{
    public DevStack()
    {
        const string _location = "WestEurope";
        const string _rgName = "adrg561825hj";
        const string _storageName = "adstorage561825";
        const string _appServicePlanCedoName = "adServiceplanCedo";
        const string _addCedoFunctionName = "addCedo561825";
        const string _getArchivesFunctionName = "getArchives561825";
        const string _getDocumentsFunctionName = "getDocuments561825";
        const string _tableArchivesName = "archives";
        const string _documentContainerName = "arcdigitcontainer";


        //Define resource group
        var rg = new Azure.Core.ResourceGroup(_rgName, new Azure.Core.ResourceGroupArgs{
            Location = _location
        });

        //Define storage account
        var storageAccount = new Azure.Storage.Account(_storageName, new Azure.Storage.AccountArgs
        {
            ResourceGroupName = rg.Name,
            Location = rg.Location,
            AccountTier = "Standard",
            AccountReplicationType = "LRS",
            Name = _storageName
        });

        //Define documents container (storage as blog)
        var documentsContainer = new Azure.Storage.Container(_documentContainerName, new Azure.Storage.ContainerArgs
        {
            StorageAccountName = storageAccount.Name,
            ContainerAccessType = "private",
            Name = _documentContainerName
        });

        //Define archive azure table
        var archivesTable = new Azure.Storage.Table(_tableArchivesName, new Azure.Storage.TableArgs
        {
            StorageAccountName = storageAccount.Name,
            Name = _tableArchivesName,
        });

        //Define azure function hosting (dynamic tier for scaling)
        var appServiceCedoPlan = new Azure.AppService.Plan(_appServicePlanCedoName, new Azure.AppService.PlanArgs
        {
            Location = rg.Location,
            ResourceGroupName = rg.Name,
            Kind = "FunctionApp",
            Sku = new Azure.AppService.Inputs.PlanSkuArgs
            {
                Tier = "Dynamic",
                Size = "Y1",
            },
            
        });

        //Define azure function - insert document
        var functionAppAddCedo = new Azure.AppService.FunctionApp(_addCedoFunctionName, new Azure.AppService.FunctionAppArgs
        {
            Name = _addCedoFunctionName,
            Location = rg.Location,
            ResourceGroupName = rg.Name,
            AppServicePlanId = appServiceCedoPlan.Id,
            StorageAccountName = storageAccount.Name,
            StorageAccountAccessKey = storageAccount.PrimaryAccessKey,
            Version = "~4"

        });

        //Define azure function - get archives
        var functionAppGetArchives = new Azure.AppService.FunctionApp(_getArchivesFunctionName, new Azure.AppService.FunctionAppArgs
        {
            Name = _getArchivesFunctionName,
            Location = rg.Location,
            ResourceGroupName = rg.Name,
            AppServicePlanId = appServiceCedoPlan.Id,
            StorageAccountName = storageAccount.Name,
            StorageAccountAccessKey = storageAccount.PrimaryAccessKey,
            Version = "~4",
            SiteConfig = new Azure.AppService.Inputs.FunctionAppSiteConfigArgs()
            {
                Cors = new Azure.AppService.Inputs.FunctionAppSiteConfigCorsArgs()
                {
                    AllowedOrigins = "*",
                    SupportCredentials = false
                }
            }

        });

        //Define azure function - get documents
        var functionAppGetDocuments = new Azure.AppService.FunctionApp(_getDocumentsFunctionName, new Azure.AppService.FunctionAppArgs
        {
            Name = _getDocumentsFunctionName,
            Location = rg.Location,
            ResourceGroupName = rg.Name,
            AppServicePlanId = appServiceCedoPlan.Id,
            StorageAccountName = storageAccount.Name,
            StorageAccountAccessKey = storageAccount.PrimaryAccessKey,
            Version = "~4",
            SiteConfig = new Azure.AppService.Inputs.FunctionAppSiteConfigArgs()
            {
                Cors = new Azure.AppService.Inputs.FunctionAppSiteConfigCorsArgs() 
                { 
                    AllowedOrigins = "*",
                    SupportCredentials = false
                }
            }

        }) ;


        /*
         * set => pulumi config set github_token xxxxxxxxxxxxxxxxxxxxx --secret
         * get => cfg.GetSecret("key")
         * 
         */
        var cfg = new Pulumi.Config();
        

    }


}
