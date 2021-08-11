using System;

namespace Sample.Order.BE.Api.DTOs
{
    public class OrderDetailsProductItemResponseDTO
    {
        /// <summary>
        /// The product id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// The product display name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The product order quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The product unit price
        /// </summary>
        [Obsolete]
        public CurrencyValueDTO UnitPrice { get; set; }

        /// <summary>
        /// The product base price
        /// </summary>
        public CurrencyValueDTO Price { get; set; }

        /// <summary>
        /// The product landed unit cost based on quantity
        /// </summary>
        public CurrencyValueDTO LandedUnitCost { get; set; }

        /// <summary>
        /// The product item subtotal amount
        /// </summary>
        public CurrencyValueDTO SubTotal { get; set; }

        /// <summary>
        /// The product item total discounts and surcharges
        /// </summary>
        public CurrencyValueDTO DiscountsAndSurchargesTotal { get; set; }

        /// <summary>
        /// The product item tax rate
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// The product item total tax
        /// </summary>
        public CurrencyValueDTO TaxTotal { get; set; }
    }
}