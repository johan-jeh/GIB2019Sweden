using System;
using System.Collections.Generic;
using System.Text;

namespace OrdersAPI
{
    public class OrderUpdateModel : OrderModel
    {
        public string id
        {
            get
            {
                return orderNumber;
            }
        }
    }
}
