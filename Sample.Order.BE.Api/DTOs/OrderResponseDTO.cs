using System;

namespace Sample.Order.BE.Api.DTOs
{
    public class OrderResponseDTO
    {
        /// <summary>
        /// The order number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// The status of the order
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The date the order was created
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// The date the order will be delivered
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// The order total
        /// </summary>
        public CurrencyValueDTO Total { get; set; }

        /// <summary>
        /// Number of items ordered
        /// </summary>
        public int NumberOfItems { get; set; }

        /// <summary>
        /// The application the order placed
        /// </summary>
        public string OrderSource { get; set; }

        /// <summary>
        /// The order reference
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// The date the order was completed
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// Array of product ids in the order
        /// </summary>
        public string[] ItemProductIds { get; set; }
    }
}