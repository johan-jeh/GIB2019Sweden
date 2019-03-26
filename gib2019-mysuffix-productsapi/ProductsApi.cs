using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Collections.Generic;

namespace gib2019_mysuffix_productsapi
{
    public class Product
    {
        public string id { get; set; }
        public int productNumber { get; set; }
        public string category { get; set; }
        public string productName { get; set; }
        public string description { get; set; }
    }
    
    public static class ProductsApi
    {
        private static string EndpointUrl = System.Environment.GetEnvironmentVariable("EndpointUrl");
        private static string PrimaryKey = System.Environment.GetEnvironmentVariable("PrimaryKey"); 
        private static string DatabaseName = System.Environment.GetEnvironmentVariable("DatabaseName");
        private static string CollectionName = System.Environment.GetEnvironmentVariable("CollectionName");

        private static DocumentClient client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

        [FunctionName("GetProducts")]
        public static async Task<Product[]> GetAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetProducts function processed a request.");

            int count = 0;
            string continuation = string.Empty;
            List<Product> products = new List<Product>();
            do
            {
                // Read the feed 10 items at a time until there are no more items to read
                FeedResponse<dynamic> response = await client.ReadDocumentFeedAsync(
                    UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                    new FeedOptions
                    {
                        MaxItemCount = 10,
                        RequestContinuation = continuation
                    });

                // Append the item count
                count += response.Count;

                // Get the continuation so that we know when to stop.
                continuation = response.ResponseContinuation;

                // Loop through documents returned, convert to Product and add to list
                foreach (var document in response)
                {
                    products.Add((Product)document);
                }

            } while (!string.IsNullOrEmpty(continuation));

            log.LogInformation($"Found {count} products.");

            return products.ToArray();
        }

        [FunctionName("GetProduct")]
        public static async Task<Product> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{category}/{id}")] HttpRequest req,
            string category, 
            string id,
            ILogger log)
        {
            log.LogInformation("GetProduct function processed a request.");
            Product product = await client.ReadDocumentAsync<Product>(
                UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id), 
                new RequestOptions { PartitionKey = new PartitionKey(category) });
            return product;
        }

        [FunctionName("CreateProduct")]
        public static async Task<CreatedResult> Create(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] Product product,
            ILogger log)
        {
            log.LogInformation("CreateProduct function processed a request.");
            await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), product);
            return new CreatedResult($"products/{product.id}", product); 
        }

        [FunctionName("DeleteProduct")]
        public static async Task<OkResult> Delete(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{category}/{id}")] HttpRequest req,
            string category,
            string id,
            ILogger log)
        {
            log.LogInformation("CreateProduct function processed a request.");
            Document doc = await client.ReadDocumentAsync(
                UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id),
                new RequestOptions { PartitionKey = new PartitionKey(category) });
            await client.DeleteDocumentAsync(doc.SelfLink,
                new RequestOptions { PartitionKey = new PartitionKey(category) });
            return new OkResult();
        }
    }
}
