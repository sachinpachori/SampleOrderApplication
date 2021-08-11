using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Order.BE.Business.Models
{

    public class OrderSimulateRequestModel
    {
        public DateTime DeliveryDateUTC { get; set; }
       
        public OrderSimulateItemModel[] ProductItems { get; set; }
       
        public decimal TaxRate { get; set; }

        public string Currency { get; set; }
    }

}
