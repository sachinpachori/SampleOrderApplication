using System.ComponentModel.DataAnnotations;

namespace Sample.Order.BE.Api.DTOs
{
    public class OrderSimulateItemRequestDTO 
    {
        /// <summary>
        /// The product id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// The product order quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The product brand name
        /// </summary>
        public string Brand { get;  set; }
        
        /// <summary>
        /// The product image url
        /// </summary>
        public string ImageUrl { get;  set; }
        
        /// <summary>
        /// The product display name
        /// </summary>
        public string Name { get;  set; }

        /// <summary>
        /// The volume prices
        /// </summary>
        public ProductPriceDTO[] Prices { get;  set; }

        /// <summary>
        /// The product is in stock
        /// </summary>
        public bool IsInStock { get; set; }

        /// <summary>
        /// The product is invalid / not found
        /// </summary>
        public bool IsInvalid { get; set; }
    }
}