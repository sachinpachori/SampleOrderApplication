using Sample.Order.BE.Data.Models.Contentful;
using System.Threading.Tasks;

namespace Sample.Order.BE.Data.HttpClients.Interfaces
{
    public interface IContentfulService
    {
        Task<DeliveryInformation> GetDeliveryInformation();
    }
}