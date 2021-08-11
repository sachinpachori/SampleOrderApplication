namespace Sample.Order.BE.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyValueDTO
    {
        /// <summary>
        /// The currency value
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// The currency type
        /// </summary>
        public string Currency { get; set; }
    }
}