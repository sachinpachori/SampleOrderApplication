using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sample.Order.BE.Data.Models.Unleashed
{
   
    public class SalesOrderResult : PaginationResult
    {
        [JsonProperty("Items")]
        public List<SalesOrder> SalesOrders { get; set; }
    }
}
