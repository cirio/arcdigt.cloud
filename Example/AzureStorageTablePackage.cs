using Pulumi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcdigit.Cloud.Deployment.Example
{
    public class AzureStorageTablePackage : ComponentResource
    {
        public AzureStorageTablePackage(string name, AzureStorageTablArgs args, ComponentResourceOptions? options = null) : base("examples:azure:AzureStorageTable", name, options) 
        { 
            //TODO 
        }
    }


    public class AzureStorageTablArgs
    {
        public Input<string> StorageAccountName { get; set; } = null!;
    }
}
