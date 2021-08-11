using System;

namespace Sample.Order.BE.Business.Models
{
    public class OrderRequestModel
    {
        public DateTime DeliveryDateUTC { get; set; }
       
        public OrderAddressModel DeliveryAddress { get; set; }
        
        public string Reference { get; set; }
        
        public string Comments { get; set; }

        public OrderItemRequestModel[] ProductItems { get; set; }
        
        public decimal SubTotal { get; set; }
        
        public decimal TaxRate { get; set; }
        
        public decimal TaxTotal { get; set; }
        
        public decimal Total { get; set; }
        
        public string SourceId { get; set; }
        
        public string OrderBy { get; set; }
        
        public string SalesPersonId { get; set; }
    }
}
