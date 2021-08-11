using System.Runtime.InteropServices;

namespace Sample.Order.BE.Api.DTOs
{
    public class OrderSimulateResponseDTO {

        /// <summary>
        /// The order product items
        /// </summary>
        public OrderSimulateItemResponseDTO[] ProductItems { get; set; }

        /// <summary>
        ///  The order total discounts and surcharges
        /// </summary>
        public CurrencyValueDTO DiscountsAndSurchargesTotal { get; set; }

        /// <summary>
        /// The order sub total
        /// </summary>
        public CurrencyValueDTO SubTotal { get; set; }

        /// <summary>
        /// The order tax total
        /// </summary>
        public CurrencyValueDTO TaxTotal { get; set; }

        /// <summary>
        /// The order total
        /// </summary>
        public CurrencyValueDTO Total { get; set; }

        /// <summary>
        /// The order messages
        /// </summary>
        public MessageDTO[] Messages { get; set; }

       
    }
}
