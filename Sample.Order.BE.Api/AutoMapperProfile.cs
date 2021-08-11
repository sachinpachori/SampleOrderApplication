using Sample.Order.BE.Api.DTOs;
using Sample.Order.BE.Business.Models;
using AutoMapper;

namespace Sample.Order.BE.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public AutoMapperProfile()
        {
            // Business model => response DTO
            CreateMap<OrderSimulateResponseModel, OrderSimulateResponseDTO>();
            CreateMap<CurrencyValueModel, CurrencyValueDTO>();
            CreateMap<ProductPriceModel, ProductPriceDTO>();
            CreateMap<OrderSimulateItemResponseModel, OrderSimulateItemResponseDTO>();
            CreateMap<MessageModel, MessageDTO>();

            CreateMap<OrderDetailsResponseModel, OrderDetailsResponseDTO>();
            CreateMap<OrderDetailsProductItemResponseModel, OrderDetailsProductItemResponseDTO>();
            CreateMap<OrderAddressModel, OrderAddressDTO>();

            CreateMap<OrderListResponseModel, OrderListResponseDTO>();
            CreateMap<OrderResponseModel, OrderResponseDTO>();

            CreateMap<OrderSummaryResponseModel, OrderSummaryResponseDTO>();

            CreateMap<DeliveryInformationResponseModel, DeliveryInformationDTO>();
            CreateMap<DeliveryInfoLabelValueModel, DeliveryInfoLabelValueDTO>();

            //DTO => model
            CreateMap<OrderSimulateRequestDTO, OrderSimulateRequestModel>();
            CreateMap<OrderSimulateItemRequestDTO, OrderSimulateItemModel>();
            CreateMap<ProductPriceDTO, ProductPriceModel>();
            CreateMap<CurrencyValueDTO, CurrencyValueModel>();

            CreateMap<OrderRequestDTO, OrderRequestModel>();

            CreateMap<OrderRequestDTO, OrderRequestModel>();
            CreateMap<OrderItemRequestDTO, OrderItemRequestModel>();
            CreateMap<OrderAddressDTO, OrderAddressModel>();
        }
    }
}
