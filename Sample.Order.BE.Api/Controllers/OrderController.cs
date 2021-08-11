using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Order.BE.Api.DTOs;
using Sample.Order.BE.Business.Enums;
using Sample.Order.BE.Business.Models;
using Sample.Order.BE.Business.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Sample.Order.BE.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    public class OrderController : BaseController<OrderController>
    {
        private readonly IOrderService service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public OrderController(ILogger<OrderController> logger, IMapper mapper, IOrderService service)
            : base(logger, mapper)
        {
            this.service = service;
        }


        /// <summary>
        /// Perform order simulation
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="payload">The order to simulate</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Simulate")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderSimulateResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public ActionResult<OrderSimulateResponseDTO> SimulateCustomerOrder([FromQuery][Required] string customerId, [Required][FromBody] OrderSimulateRequestDTO payload)
        {
            try
            {
                var model = _mapper.Map<OrderSimulateRequestDTO, OrderSimulateRequestModel>(payload);
                var response = service.SimulateOrder(customerId, model);
                var responseDTO = _mapper.Map<OrderSimulateResponseModel, OrderSimulateResponseDTO>(response);
                return Ok(responseDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception SimulateCustomerOrder CustomerId {customerId}", customerId);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Submit order
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="payload">The order to submit</param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<ActionResult<string>> SubmitCustomerOrder([FromQuery][Required] string customerId, [Required][FromBody] OrderRequestDTO payload)
        {
            try
            {
                var order = _mapper.Map<OrderRequestDTO, OrderRequestModel>(payload);
                var orderId = await service.SubmitCustomerOrder(customerId, order);

                return Ok(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception SubmitCustomerOrder CustomerId {customerId}", customerId);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get the order details
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="orderNumber"></param>
        /// <returns>Returns an OrderDetailsResponseDTO due to successful operation</returns>
        [HttpGet]
        [Route("{orderNumber}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderDetailsResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<ActionResult<OrderDetailsResponseDTO>> GetCustomerOrderDetails([FromQuery][Required] string customerId, [Required][FromRoute] string orderNumber)
        {
            try
            {
                var orderDetailsResponse = await service.GetOrderDetails(customerId, orderNumber);
                var dto = _mapper.Map<OrderDetailsResponseModel, OrderDetailsResponseDTO>(orderDetailsResponse);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception GetCustomerOrderDetails CustomerId {customerId}", customerId);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Get the paged list of orders
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageLimit"></param>
        /// <param name="filterByStatus"></param>
        /// <returns></returns>
        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderListResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<ActionResult<OrderListResponseDTO>> GetCustomerOrderList([FromQuery][Required] string customerId, [Required][FromQuery] int pageNo, [Required][FromQuery] int pageLimit, [FromQuery]OrderStatusEnum filterByStatus = OrderStatusEnum.All )
        {
            try
            {
                var orderList = await service.GetOrders(customerId, pageNo, pageLimit, filterByStatus);
                var dto = _mapper.Map<OrderListResponseModel, OrderListResponseDTO>(orderList);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception GetCustomerOrderList CustomerId {customerId}", customerId);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get the order details of the last order
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("lastOrder")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderDetailsResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<ActionResult<OrderDetailsResponseDTO>> GetLastOrder([FromQuery][Required] string customerId)
        {
            try
            {

                var orderList = await service.GetLastOrder(customerId);
                var dto = _mapper.Map<OrderDetailsResponseModel, OrderDetailsResponseDTO>(orderList);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception GetLastOrder CustomerId {customerId}", customerId);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get the order summaries of next undelivered orders
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("nextUndeliveredOrder")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<OrderSummaryResponseDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<ActionResult<List<OrderSummaryResponseDTO>>> GetNextUndeliveredOrder([FromQuery][Required] string customerId)
        {
            try
            {
                var order = await service.GetNextUndeliveredOrder(customerId);
                var dto = _mapper.Map<List<OrderSummaryResponseModel>, List<OrderSummaryResponseDTO>>(order);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception GetNextUndeliveredOrder CustomerId {customerId}", customerId);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets delivery information
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("deliveryInformation")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DeliveryInformationDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<ActionResult<OrderSummaryResponseDTO>> GetDeliveryInformation()
        {
            try
            {
                var order = await service.GetDeliveryInformation();
                var dto = _mapper.Map<DeliveryInformationResponseModel, DeliveryInformationDTO>(order);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception GetDeliveryInformation");
                return BadRequest(ex.Message);
            }
        }
    }
}
