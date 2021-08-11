using Sample.Customer.BE.Data.Models.Unleashed;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Order.BE.Data.Models.Unleashed
{
    public class PaginationResult : Result
    {
        public Pagination Pagination { get; set; }
    }
}
