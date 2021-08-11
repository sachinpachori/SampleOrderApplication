namespace Sample.Order.BE.Business.Models
{
    public class OrderSimulateItemResponseModel {

        public string ProductId { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string ImageUrl { get; set; }

        public int Quantity { get; set; }

        public ProductPriceModel[] Prices { get; set; }

        public CurrencyValueModel LandedUnitPrice { get; set; }

        public CurrencyValueModel SubTotal { get; set; }

        public CurrencyValueModel DiscountsAndSurchargesTotal { get; set; }

        public decimal TaxRate { get; set; }

        public CurrencyValueModel TaxTotal { get; set; }

        public MessageModel[] Messages { get; set; }
    
    }
}
