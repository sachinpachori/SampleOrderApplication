namespace Sample.Order.BE.Business.Models
{
    public class OrderSimulateResponseModel
    {
        public OrderSimulateItemResponseModel[] ProductItems { get; set; }
      
        public CurrencyValueModel DiscountsAndSurchargesTotal { get; set; }
       
        public CurrencyValueModel SubTotal { get; set; }
     
        public CurrencyValueModel TaxTotal { get; set; }

        
        public CurrencyValueModel Total { get; set; }

        public MessageModel[] Messages { get; set; }
    }
}
