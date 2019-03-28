using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace OrdersEvent
{
    public static class CosmosDBTrigger
    {
        [FunctionName("CosmosTrigger")]
        public static void Run([CosmosDBTrigger(
            databaseName: "Products",
            collectionName: "Orders",
            ConnectionStringSetting = "CosmosDBConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);

            }
        }
    }
}

// [EventGridOutput(TopicEndpoint = "https://mlogdberg-product.westeurope-1.eventgrid.azure.net/api/events", SasKey = "9ocn4PKNH2Z6mLDwNYE7aI1Y+MZ+TSuivBtUtdJmI4A=")] out EventGridOutput outputEvent,
//               outputEvent = new EventGridOutput(input, "Orders.Update", $"product/update/count/{input.Count}");
//log.LogInformation("EventGrid MLogdberg.Products.Update event sent");