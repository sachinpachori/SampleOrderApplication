using Sample.Order.BE.Data.Models.Unleashed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Order.BE.Data.HttpClients.Interfaces
{
    public interface IUnleashedClient
    {
        Task<SalesOrder> SubmitOrder(SalesOrder order);
        Task<SalesOrder> GetOrderByOrderNumber(string customerId, string orderNumber);
        Task<SalesOrderResult> GetOrders(string customerId, int pageNo, int pageLimit, string filterByStatus);
        Task<SalesOrderSummaryResult> GetOrderSummaries(string customerId, int pageNo, int pageLimit, string filterByStatus);
    }
}