using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using OrganizerWebApp.Models;
using OrganizerWebApp.Helpers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OrganizerWebApp.Controllers
{
    public class SupplierController : Controller
    {
        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var items = await DocumentDBRepository<Supplier>.GetItemsAsync(d => true);
            return View(items.OrderBy(s => s.RegistrationTime));
        }

        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Name","ApiBaseUrl", "ApiSubscriptionKey", "StorageBaseUrl")] Supplier item)
        {
            item.Id = System.Guid.NewGuid().ToString();
            // I know this is a hack, but it just needs to work on 2019-03-30
            item.RegistrationTime = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");

            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Supplier>.CreateItemAsync(item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Id", "Name", "ApiBaseUrl", "ApiSubscriptionKey", "StorageBaseUrl", "RegistrationTime")] Supplier item)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Supplier>.UpdateItemAsync(item.Id, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Supplier item = await DocumentDBRepository<Supplier>.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [ActionName("Products")]
        public async Task<ActionResult> ProductsAsync(string id)
        {
            try
            {
                Supplier item = await DocumentDBRepository<Supplier>.GetItemAsync(id);

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", item.ApiSubscriptionKey);

                var serializer = new DataContractJsonSerializer(typeof(List<Product>));
                var streamTask = client.GetStreamAsync(new Uri(new Uri(
                    item.ApiBaseUrl.EndsWith('/') ? item.ApiBaseUrl : item.ApiBaseUrl + "/"), "products"));
                var products = serializer.ReadObject(await streamTask) as List<Product>;

                if (!string.IsNullOrEmpty(item.StorageBaseUrl))
                {
                    foreach (var product in products)
                    {
                        product.imageUrl = new Uri(new Uri(item.StorageBaseUrl), $"thumbnails/{product.productNumber}.jpg").ToString();
                    }
                }

                ViewBag.ERROR = string.Empty;
                return View(products.OrderBy(p => p.productNumber));
            }
            catch (Exception ex)
            {
                ViewBag.ERROR = ex.Message;
                string url = string.Empty;
                try
                {
                    Supplier item = await DocumentDBRepository<Supplier>.GetItemAsync(id);
                    url = new Uri(new Uri(item.ApiBaseUrl), "products").ToString();
                    ViewBag.URL = url;
                }
                catch (Exception ex2) { }
                return View();
            }
        }
    }
}