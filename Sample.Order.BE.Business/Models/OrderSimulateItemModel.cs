namespace Sample.Order.BE.Business.Models
{
    public class OrderSimulateItemModel : OrderItemRequestModel
    {
        public string Brand { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public ProductPriceModel[] Prices { get; set; }

        public bool IsInStock { get;  set; }

        public bool IsInvalid { get; set; }
    }

}
