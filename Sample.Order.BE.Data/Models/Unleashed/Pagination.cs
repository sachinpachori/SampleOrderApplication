﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Order.BE.Data.Models.Unleashed
{
    public class Pagination
    {
        public int NumberOfItems { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }
}
