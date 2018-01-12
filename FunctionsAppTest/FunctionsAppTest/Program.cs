using System;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;


namespace FunctionsAppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var credentials = SdkContext.AzureCredentialsFactory
    .FromFile(@"..\..\azureauth.properties");

            var azure = Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials)
                .WithDefaultSubscription();


            var templatePath = @"https://raw.githubusercontent.com/prasannapad/ARMTemplatesPlay/master/FunctionsAppTest/FunctionsAppTest/CreateVMTemplate.json";
            var paramPath = @"https://raw.githubusercontent.com/prasannapad/ARMTemplatesPlay/master/FunctionsAppTest/FunctionsAppTest/Parameters.json";


            var deployment = azure.Deployments.Define("myDeployment")
                .WithNewResourceGroup("PrasannaTestArm1", Region.USWest2)
                .WithTemplateLink(templatePath, "1.0.0.0")
                .WithParametersLink(paramPath, "1.0.0.0")
                .WithMode(Microsoft.Azure.Management.ResourceManager.Fluent.Models.DeploymentMode.Incremental)
                .Create();
            Console.WriteLine("Press enter to delete the resource group...");
            Console.ReadLine();
        }   
    }
}
