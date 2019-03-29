using System;
using System.Collections.Generic;
using System.Text;

namespace OrdersAPI
{
    public class OrderModel
    { 
        public string orderNumber { get; set; }
        public string customer { get; set; }
        public DateTime deliveryDate { get; set; }
        public string reference { get; set; }
        public string customerEmail { get; set; }
        public string status { get; set; }
        public OrderLineModel[] lines { get; set; }
    }
}
