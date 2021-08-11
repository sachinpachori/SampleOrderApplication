using System;

namespace Sample.Order.BE.Api.DTOs
{
    public class OrderSummaryResponseDTO
    {

        /// <summary>
        /// The order number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// The date the order delivery will be delivered
        /// </summary>
        public DateTime DeliveryDate { get; set; }
    }
}