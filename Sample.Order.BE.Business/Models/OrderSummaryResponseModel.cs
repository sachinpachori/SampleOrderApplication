using System;

namespace Sample.Order.BE.Business.Models
{
    public class OrderSummaryResponseModel
    {
        public string OrderNumber { get; set; }
       
        public DateTime DeliveryDate { get; set; }
    }
}
