using Sample.Order.BE.Business.Enums;
using Sample.Order.BE.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Order.BE.Business.Services.Interfaces
{
    public interface IOrderService
    {
        OrderSimulateResponseModel SimulateOrder(string customerId, OrderSimulateRequestModel model);
        Task<string> SubmitCustomerOrder(string customerId, OrderRequestModel order);
        Task<OrderDetailsResponseModel> GetOrderDetails(string customerId, string orderNumber);
        Task<OrderListResponseModel> GetOrders(string customerId, int pageNo, int pageLimit, OrderStatusEnum filterByStatus);
        Task<OrderDetailsResponseModel> GetLastOrder(string customerId);
        Task<List<OrderSummaryResponseModel>> GetNextUndeliveredOrder(string customerId);
        Task<DeliveryInformationResponseModel> GetDeliveryInformation();
    }
}
