using System;

namespace Sample.Order.BE.Business.Models
{
    public class OrderDetailsResponseModel
    {
        public Guid Id { get; set; }
       
        public string OrderNumber { get; set; }
       
        public string Status { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderBy { get; set; }

        public DateTime DeliveryDate { get; set; }

        public OrderAddressModel DeliveryAddress { get; set; }

        public OrderDetailsProductItemResponseModel[] ProductItems { get; set; }

       
        public CurrencyValueModel SubTotal { get; set; }

      
        public CurrencyValueModel TaxTotal { get; set; }

       
        public CurrencyValueModel DiscountsAndSurchargesTotal { get; set; }

       
        public CurrencyValueModel Total { get; set; }

        
        public DateTime LastModifiedDate { get; set; }

        public string OrderSource { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string Reference { get; set; }

        public string Comments { get; set; }

        public int NumberOfItems { get; set; }
    }
}
