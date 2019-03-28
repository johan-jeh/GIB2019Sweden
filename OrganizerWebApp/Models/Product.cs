using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrganizerWebApp.Models
{
    public class Product
    {
        public string id { get; set; }
        public int productNumber { get; set; }
        public string category { get; set; }
        public string productName { get; set; }
        public string description { get; set; }
    }
}
