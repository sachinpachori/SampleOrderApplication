using System;

namespace Sample.Order.BE.Api.DTOs
{
    public class OrderDetailsResponseDTO {

        /// <summary>
        /// The order id (GUID)
        /// </summary>
        public string  Id { get; set; }

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
        /// The user that created the order
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// The date the order delivery will be delivered
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// The order delivery address
        /// </summary>
        public OrderAddressDTO DeliveryAddress { get; set; }

        /// <summary>
        /// The order items
        /// </summary>
        public OrderDetailsProductItemResponseDTO[] ProductItems { get; set; }

        /// <summary>
        /// The order total (exclusive of tax)
        /// </summary>
        public CurrencyValueDTO SubTotal { get; set; }

        /// <summary>
        /// The order tax total
        /// </summary>
        public CurrencyValueDTO TaxTotal { get; set; }

        /// <summary>
        /// The order discounts and surcharges total
        /// </summary>
        public CurrencyValueDTO DiscountsAndSurchargesTotal { get; set; }

        /// <summary>
        /// The order total (inclusive of tax)
        /// </summary>
        public CurrencyValueDTO Total { get; set; }

        /// <summary>
        /// The date the order was last modified
        /// </summary>
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Source where order submitted
        /// </summary>
        public string OrderSource { get; set; }

        /// <summary>
        /// Date the order was completed
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// The order reference
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// The order comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// The number of items in the order
        /// </summary>
        public int NumberOfItems { get; set; }
    }
}