using System;

namespace Sample.Order.BE.Business.Models
{
    public class OrderDetailsProductItemResponseModel
    {
        public string ProductId { get; set; }
       
        public string Name { get; set; }
       
        public int Quantity { get; set; }

        [Obsolete]
        public CurrencyValueModel UnitPrice { get; set; }

        public CurrencyValueModel Price { get; set; }

        public CurrencyValueModel LandedUnitCost { get; set; }

        public CurrencyValueModel SubTotal { get; set; }
        
        public CurrencyValueModel DiscountsAndSurchargesTotal { get; set; }
       
        public decimal TaxRate { get; set; }
       
        public CurrencyValueModel TaxTotal { get; set; }
    }
}
