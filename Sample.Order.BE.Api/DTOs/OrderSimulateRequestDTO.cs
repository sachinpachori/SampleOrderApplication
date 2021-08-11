using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Order.BE.Api.DTOs
{
    public class OrderSimulateRequestDTO
    {
        /// <summary>
        /// The order delivery date
        /// </summary>
        public DateTime DeliveryDateUTC { get; set; }

        /// <summary>
        /// The order items
        /// </summary>
        public OrderSimulateItemRequestDTO[] ProductItems { get; set; }

        //new fields
        public decimal TaxRate { get; set; }

        public string Currency { get; set; }
    }
}
