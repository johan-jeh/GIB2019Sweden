using System;
using System.Collections.Generic;
using System.Text;

namespace OrdersAPI
{
    public class OrderLineModel
    {
        public string lineId { get; set; }
        public string productNumber { get; set; }
        public string comment { get; set; }
        public int quantity { get; set; }

    }
}
