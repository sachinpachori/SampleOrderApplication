using System;

namespace Sample.Order.BE.Api.DTOs
{

    public class OrderRequestDTO
    {
        /// <summary>
        /// The order delivery date
        /// </summary>
        public DateTime DeliveryDateUTC { get; set; }

        /// <summary>
        /// The order delivery address
        /// </summary>
        public OrderAddressDTO DeliveryAddress { get; set; }

        /// <summary>
        /// The order reference
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// The order comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// The order items
        /// </summary>
        public OrderItemRequestDTO[] ProductItems { get; set; }

        /// <summary>
        /// The subtotal for the order
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// The tax rate for the order
        /// </summary>
        public decimal TaxRate { get; set; }
        
        /// <summary>
        /// The tax total for the order 
        /// </summary>
        public decimal TaxTotal { get; set; }
        
        /// <summary>
        /// The total for the order
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Source application where order submited
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// User's full name who placed the order
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// The sales person id for the customer
        /// </summary>
        public string SalesPersonId { get; set; }
    }
}