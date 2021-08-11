using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sample.Order.BE.Data.Models.Unleashed
{
   
    public class SalesOrderSummaryResult : PaginationResult
    {
        [JsonProperty("Items")]
        public List<SalesOrderSummary> SalesOrders { get; set; }
    }
}
