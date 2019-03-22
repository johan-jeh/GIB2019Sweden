using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        static Product[] sampleProducts = new Product[] { new Product { id = "1", productNumber = 1, category = "bikes", productName = "speeder", description = "pretty" } };

        [FunctionName("GetProducts")]
        public static async Task<Product[]> GetAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetProducts function processed a request.");
            return sampleProducts;
        }

        [FunctionName("GetProduct")]
        public static async Task<Product> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("GetProduct function processed a request.");
            return sampleProducts[int.Parse(id)-1];
        }

        [FunctionName("CreateProduct")]
        public static async Task<CreatedResult> Create(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] Product product,
            ILogger log)
        {
            log.LogInformation("CreateProduct function processed a request.");

            
            return new CreatedResult($"products/{product.id}", product); 
        }

        [FunctionName("DeleteProduct")]
        public static async Task<OkResult> Delete(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id}")] Product product,
            string id,
            ILogger log)
        {
            log.LogInformation("CreateProduct function processed a request.");
            return new OkResult();
        }


    }
}
