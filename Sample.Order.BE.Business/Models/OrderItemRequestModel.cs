namespace Sample.Order.BE.Business.Models
{
    public class OrderItemRequestModel
    {
        public string ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
     
    }

}
