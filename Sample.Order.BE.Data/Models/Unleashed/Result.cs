using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Customer.BE.Data.Models.Unleashed
{
    public class Result
    {
        [JsonProperty("description")]
        public string ErrorDescription { get; set; }
        public bool HasError { get { return !string.IsNullOrEmpty(ErrorDescription); } }
    }
}
