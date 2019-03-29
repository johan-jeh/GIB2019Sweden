using System.Collections.Generic;
using Fbeltrao.AzureFunctionExtensions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace OrdersEvent
{
    public static class CosmosDBTrigger
    {
        [FunctionName("CosmosTrigger")]
        [return: EventGridOutput(TopicEndpoint = "https://orderstopic.westeurope-1.eventgrid.azure.net/api/events", SasKey = "HziWfSAiKqqrj8QFf9IZCyjycGmtaiCrfoD6nZk3WDE=")]
        public static EventGridOutput[] Run([CosmosDBTrigger(
            databaseName: "Products",
            collectionName: "Orders",
            ConnectionStringSetting = "CosmosDBConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
           
            ILogger log)
        {
            var events = new List<EventGridOutput>();
            if (input != null && input.Count > 0)
            {                
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);

                foreach (var item in input)
                {
                    events.Add(new EventGridOutput()
                    {
                        Data = item,
                        EventType = string.Format("Orders.{0}",item.GetPropertyValue<string>("status")),
                        Subject = string.Format("Orders/{0}", item.Id)
                    });
                }
            }
            return events.ToArray();
        }
    }
}
