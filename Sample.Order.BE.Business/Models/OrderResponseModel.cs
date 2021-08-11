using System;

namespace Sample.Order.BE.Business.Models
{
    public class OrderResponseModel
    {
        public string OrderNumber { get; set; }

        public string Status { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime DeliveryDate { get; set; }

        public CurrencyValueModel Total { get; set; }

        public int NumberOfItems { get; set; }

        public string OrderSource { get; set; }

        public string Reference { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string[] ItemProductIds { get; set; }

    }
}
