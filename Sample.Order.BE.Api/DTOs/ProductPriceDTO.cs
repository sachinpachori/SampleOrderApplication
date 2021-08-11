namespace Sample.Order.BE.Api.DTOs
{
    public class ProductPriceDTO
    {
        /// <summary>
        /// The product price format name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The number of base units for the format
        /// </summary>
        public int BaseUnitQty { get; set; }

        /// <summary>
        /// The product price amount
        /// </summary>
        public CurrencyValueDTO Price { get; set; }

        /// <summary>
        /// The product previous price amount
        /// </summary>
        public CurrencyValueDTO PreviousPrice { get; set; }
    }
}