using System;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionAppArmDeploy
{
    public static class Function1
    {
        [FunctionName("DeployTemplateFunctionName")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            //Good samples here: https://github.com/Azure/azure-libraries-for-net/blob/master/Samples/ResourceManager/DeployUsingARMTemplateWithProgress.cs
            AzureEnvironment ae = new AzureEnvironment();
            ae.ResourceManagerEndpoint = "https://management.azure.com/";
            ae.AuthenticationEndpoint = "https://login.windows.net/";
            ae.ManagementEndpoint = "https://management.core.windows.net/";
            ae.GraphEndpoint = "https://graph.windows.net/";

            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal("clientid", "clientsecret", "72f988bf-86f1-41af-91ab-2d7cd011db47", ae);
            //.FromFile(@"..\..\azureauth.properties");

            var azure = Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials)
                .WithDefaultSubscription();


            var templatePath = @"https://raw.githubusercontent.com/prasannapad/ARMTemplatesPlay/master/FunctionsAppTest/FunctionsAppTest/CreateVMTemplate.json";
            var paramPath = @"https://raw.githubusercontent.com/prasannapad/ARMTemplatesPlay/master/FunctionsAppTest/FunctionsAppTest/Parameters.json";

            string deploymentName = "myDeployment";
            string rgName = "PrasannaTestArm1";

            azure.ResourceGroups.Define(rgName)
            .WithRegion(Region.USWestCentral)
            .Create();

            azure.Deployments.Define(deploymentName)
                .WithExistingResourceGroup(rgName)
                .WithTemplateLink(templatePath, "1.0.0.0")
                .WithParametersLink(paramPath, "1.0.0.0")
                .WithMode(DeploymentMode.Incremental)
                .BeginCreate();

            var deployment = azure.Deployments.GetByResourceGroup(rgName, deploymentName);
            log.Info($"Current deployment status {deployment.ProvisioningState.ToString()}");

            return req.CreateResponse(HttpStatusCode.OK, deployment.ProvisioningState.ToString());

            /*log.Info("C# HTTP trigger function processed a request right now.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, $"Hello  da {name}");*/

        }
    }
}
