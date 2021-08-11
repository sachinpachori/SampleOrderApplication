namespace Sample.Order.BE.Api.DTOs
{
    public class OrderListResponseDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public OrderResponseDTO[] Items { get; set; }

        /// <summary>
        /// The total number of orders available
        /// </summary>
        public int TotalCount { get; set; }
    }
}