using Sample.Order.BE.Data.Config;
using Sample.Order.BE.Data.Helper;
using Sample.Order.BE.Data.HttpClients.Interfaces;
using Sample.Order.BE.Data.Models.Unleashed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sample.Order.BE.Data.HttpClients
{
    public class UnleashedClient : HttpClientBase, IUnleashedClient
    {
        private readonly UnleashedConfig unleashedConfig;
        private readonly string SalesOrderPath = "SalesOrders/{0}";
        private readonly string SalesOrderByOrderNumberPath = "SalesOrders?customerCode={0}&orderNumber={1}";
        private readonly string SalesOrderListPath = "SalesOrders/{0}?customerCode={1}&pageSize={2}&orderStatus={3}";
        private const string DateStringFormat = "yyyy-MM-ddTHH:mm";

        public UnleashedClient(IOptions<UnleashedConfig> unleashedConfig, HttpClient client, ILogger<UnleashedClient> logger) : base(client, logger)
        {
            this.unleashedConfig = unleashedConfig.Value;
        }

        private string GetQuerystring(string url)
        {
            if (url.IndexOf('?') > 0)
            {
                return url.Split("?")[1];
            }
            return "";
        }

        private void UpdateApiAuthSignature(string url)
        {
            var queryString = GetQuerystring(url);
            var signature = GetSignature(queryString);
            client.DefaultRequestHeaders.Remove(UnleashedConfig.ApiAuthSignature);
            client.DefaultRequestHeaders.Add(UnleashedConfig.ApiAuthSignature, signature);
        }

        private string GetSignature(string querystring)
        {
            var encoding = new UTF8Encoding();
            byte[] key = encoding.GetBytes(unleashedConfig.ApiKey);
            var myhmacsha256 = new HMACSHA256(key);
            byte[] hashValue = myhmacsha256.ComputeHash(encoding.GetBytes(querystring));
            string hmac64 = Convert.ToBase64String(hashValue);
            myhmacsha256.Clear();
            return hmac64;
        }

        public async Task<SalesOrder> SubmitOrder(SalesOrder order)
        {
            var url = string.Format(SalesOrderPath, order.Guid);
            UpdateApiAuthSignature(url);

            var serializationOptions = new JsonSerializerOptions
            {
                Converters = { new UnleashedDateTimeConverter() }
            };
            var request = JsonSerializer.Serialize(order, serializationOptions);
            var salesOrder = await PostAsync<SalesOrder>(url, new StringContent(request, Encoding.UTF8, "application/json"));

            return salesOrder;
        }

        public async Task<SalesOrder> GetOrderByOrderNumber(string customerId, string orderNumber)
        {
            var url = string.Format(SalesOrderByOrderNumberPath, customerId, orderNumber);
            UpdateApiAuthSignature(url);

            var result = await GetAsync<SalesOrderResult>(url);
            if (result.HasError)
            {
                throw new Exception($"Unleashed error occurred: { result.ErrorDescription }");
            }
            else
            {
                if (result != null && result.SalesOrders.Count > 0)
                {
                    return result.SalesOrders.First();
                }
                else
                {
                    throw new ArgumentException("Sales order not found");
                }
            }
        }

        public async Task<SalesOrderResult> GetOrders(string customerId, int pageNo, int pageLimit, string filterByStatus)
        {
            var url = string.Format(SalesOrderListPath, pageNo, customerId, pageLimit, filterByStatus);
            UpdateApiAuthSignature(url);

            var results = await GetAsync<SalesOrderResult>(url);

            if (results.HasError)
            {
                throw new Exception($"Unleashed error occurred: { results.ErrorDescription }");
            }
            else
            {
                return results;
            }
        }

        public async Task<SalesOrderSummaryResult> GetOrderSummaries(string customerId, int pageNo, int pageLimit, string filterByStatus)
        {
            var url = string.Format(SalesOrderListPath, pageNo, customerId, pageLimit, filterByStatus);
            UpdateApiAuthSignature(url);

            var results = await GetAsync<SalesOrderSummaryResult>(url);

            if (results.HasError)
            {
                throw new Exception($"Unleashed error occurred: { results.ErrorDescription }");
            }
            else
            {
                return results;
            }
        }
    }
}
