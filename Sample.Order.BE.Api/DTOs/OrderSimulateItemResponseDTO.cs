namespace Sample.Order.BE.Api.DTOs
{
    public class OrderSimulateItemResponseDTO
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
        /// The product brand name
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// The product image url
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// The product format prices
        /// </summary>
        public ProductPriceDTO[] Prices { get; set; }

        /// <summary>
        ///  The product landed unit price based on quantity
        /// </summary>
        public CurrencyValueDTO LandedUnitPrice { get; set; }

        /// <summary>
        /// The order item subtotal amount
        /// </summary>
        public CurrencyValueDTO SubTotal { get; set; }

        /// <summary>
        /// The order item total discounts and surcharges
        /// </summary>
        public CurrencyValueDTO DiscountsAndSurchargesTotal { get; set; }

        /// <summary>
        /// The order tax rate
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// The order item total tax
        /// </summary>
        public CurrencyValueDTO TaxTotal { get; set; }

        /// <summary>
        /// Messages related to order simulation
        /// </summary>
        public MessageDTO[] Messages { get; set; }

    }
}