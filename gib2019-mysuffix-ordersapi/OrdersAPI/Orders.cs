using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace OrdersAPI
{
    public static class Orders
    {

        private static OrderModel[] mockdata = new OrderModel[] {
            new OrderModel()
            {
                customer = "mockCustomer",
                customerEmail = "mocked@email.se",
                deliveryDate = DateTime.Today,
                orderNumber = "11111",
                reference = "mocked",
                lines  = new OrderLineModel[]
                {
                    new OrderLineModel()
                    {
                        lineId = "01",
                        productNumber = "1",
                        quantity = 1,
                        comment = "mocked"
                    }
                }
            }
            };

        [FunctionName("GetOrders")]
        public static async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders")] HttpRequest req,
            [CosmosDB("Products", "Orders", ConnectionStringSetting = "CosmosDBConnectionString", SqlQuery = "select * from orders")] IEnumerable<OrderModel> item,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request. requesting for orders");

            return (ActionResult)new OkObjectResult(item);
        }

        [FunctionName("GetOrdersById")]
        public static async Task<IActionResult> GetByOrderNumber(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{orderid}")] HttpRequest req,
           [CosmosDB("Products", "Orders", ConnectionStringSetting = "CosmosDBConnectionString", SqlQuery = "select * from orders c where c.id = {orderid}")] IEnumerable<OrderModel> item,
           ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request. requesting for orders");

            var enumerator = item.GetEnumerator();
            if (enumerator.MoveNext())
                return (ActionResult)new OkObjectResult(enumerator.Current);
            else
                return new NoContentResult();
        }

        [FunctionName("CreateOrder")]
        public static IActionResult CreateOrder(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequest req,
           [CosmosDB(
                databaseName: "Products",
                collectionName: "Orders",
                ConnectionStringSetting = "CosmosDBConnectionString")]out OrderModel document,
           ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request. Create orders");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            document = JsonConvert.DeserializeObject<OrderModel>(requestBody);
            document.status = "Created";
            log.LogInformation($"Order Created in Cosmos: {(string)document.orderNumber}");
            return new OkObjectResult(document);
        }

        [FunctionName("UpdateOrder")]
        public static IActionResult UpdateOrder(
           [HttpTrigger(AuthorizationLevel.Function, "put", Route = "orders/{orderid}")] HttpRequest req,
           [CosmosDB(
                databaseName: "Products",
                collectionName: "Orders",
                ConnectionStringSetting = "CosmosDBConnectionString")]out OrderUpdateModel document,
           [CosmosDB("Products", "Orders", ConnectionStringSetting = "CosmosDBConnectionString", SqlQuery = "select * from orders c where c.id = {orderid}")] IEnumerable<OrderUpdateModel> item,
           ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request. Update orders");



            string requestBody = new StreamReader(req.Body).ReadToEnd();
            document = JsonConvert.DeserializeObject<OrderUpdateModel>(requestBody);
            document.status = "Updated";

            var enumerator = item.GetEnumerator();
            if (enumerator.MoveNext())
            {
                document.orderNumber = enumerator.Current.orderNumber;

            }
            else
            {
                document = null;
                return new EmptyResult();
            }
            log.LogInformation($"Order updated in Cosmos: {(string)document.orderNumber}");
            return new OkObjectResult(document);
        }
    }
}
