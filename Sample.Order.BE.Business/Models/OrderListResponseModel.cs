namespace Sample.Order.BE.Business.Models
{
    public class OrderListResponseModel
    {
        public OrderResponseModel[] Items { get; set; }

        public int TotalCount { get; set; }
    }
}
