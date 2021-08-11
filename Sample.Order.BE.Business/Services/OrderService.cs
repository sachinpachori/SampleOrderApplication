using Sample.Order.BE.Business.Configs;
using Sample.Order.BE.Business.Enums;
using Sample.Order.BE.Business.Models;
using Sample.Order.BE.Business.Services.Interfaces;
using Sample.Order.BE.Data.HttpClients.Interfaces;
using Sample.Order.BE.Data.Models.Unleashed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Order.BE.Business.Services
{
    public class OrderService : BaseService, IOrderService, IDisposable
    {
        #region IDisposable Code
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Dispose unmanaged resources.

                disposed = true;
            }
        }





        ~OrderService()
        {
            Dispose(false);
        }
        #endregion

        private readonly MessagesConfig messagesConfig;
        private readonly IUnleashedClient unleashedClient;
        private readonly IContentfulService contentfulService;
        private const string OrderStatus = "Parked";
        private const string UndeliverdOrderStatus = "Placed,Completed";

        public OrderService(IUnleashedClient unleashedClient, IContentfulService contentfulService, IOptions<MessagesConfig> messagesConfig, ILogger<OrderService> logger) : base(logger)
        {
            this.messagesConfig = messagesConfig.Value;
            this.unleashedClient = unleashedClient;
            this.contentfulService = contentfulService;
        }

        public OrderSimulateResponseModel SimulateOrder(string customerId, OrderSimulateRequestModel model)
        {
            var runningSubTotal = Decimal.Zero;
            var runningTaxTotal = Decimal.Zero;
            var runningDiscountsSurchargesTotal = Decimal.Zero;

            var cartItems = new List<OrderSimulateItemResponseModel>();
            foreach (var item in model.ProductItems)
            {
                var productItem = new OrderSimulateItemResponseModel()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    DiscountsAndSurchargesTotal = new Models.CurrencyValueModel { Value = 0, Currency = model.Currency },
                    TaxRate = model.TaxRate,
                };

                productItem.Brand = item.Brand;
                productItem.ImageUrl = item.ImageUrl;
                productItem.Name = item.Name;

                if (item.IsInvalid)
                {
                    var messages = new MessageModel { MessageType = MessageTypeEnum.Error, Text = messagesConfig.GetCart_ProductNotFound };
                    productItem.Messages = new MessageModel[] { messages };
                }
                else
                {
                    var smallestBaseUnitPrice = item.Prices.Min(x => x.BaseUnitQty); //Get the lowest baseUnitQty to get the landedUnitPriceValue. (UK only has price / one baseUnityQty)
                    var landedUnitPriceValue = item.Prices.Where(x => x.BaseUnitQty == smallestBaseUnitPrice).Select(x => x.Price.Value).FirstOrDefault();
                    var currencyCode = item.Prices.First().Price.Currency;
                    var subtotal = landedUnitPriceValue * item.Quantity;
                    var taxTotal = model.TaxRate * subtotal;

                    runningSubTotal += subtotal;
                    runningTaxTotal += taxTotal;

                    productItem.Prices = item.Prices; // UK only has one price
                    productItem.LandedUnitPrice = new CurrencyValueModel { Value = landedUnitPriceValue, Currency = currencyCode };
                    productItem.SubTotal = new CurrencyValueModel { Value = subtotal, Currency = currencyCode };
                    productItem.TaxTotal = new CurrencyValueModel { Value = taxTotal, Currency = currencyCode };
                    productItem.Messages = new MessageModel[] { };

                    if (!item.IsInStock)
                    {
                        var messages = new MessageModel { MessageType = MessageTypeEnum.Information, Text = messagesConfig.ProductOutOfStock };
                        productItem.Messages = new MessageModel[] { messages };
                    }
                }

                cartItems.Add(productItem);
            }
            var response = new OrderSimulateResponseModel
            {
                ProductItems = cartItems.ToArray(),
                DiscountsAndSurchargesTotal = new CurrencyValueModel { Value = runningDiscountsSurchargesTotal, Currency = model.Currency },
                SubTotal = new CurrencyValueModel { Currency = model.Currency, Value = runningSubTotal },
                TaxTotal = new CurrencyValueModel
                {
                    Currency = model.Currency,
                    Value = runningTaxTotal
                },
                Total = new CurrencyValueModel
                {
                    Currency = model.Currency,
                    Value = runningSubTotal + runningTaxTotal + runningDiscountsSurchargesTotal
                },
                Messages = new MessageModel[] { }
            };

            return response;
        }

        public async Task<string> SubmitCustomerOrder(string customerId, OrderRequestModel order)
        {

            var salesOrderLines = new List<SalesOrderLine>();

            for (int i = 0; i < order.ProductItems.Length; i++)
            {
                salesOrderLines.Add(new SalesOrderLine
                {
                    LineNumber = i + 1,
                    LineTax = order.ProductItems[i].TaxTotal,
                    OrderQuantity = order.ProductItems[i].Quantity,
                    UnitPrice = order.ProductItems[i].UnitPrice,
                    LineTotal = order.ProductItems[i].LineTotal,
                    Product = new Product { ProductCode = order.ProductItems[i].ProductId }
                });
            }

            var orderRequest = new SalesOrder
            {
                Comments = order.Comments,
                Customer = new Data.Models.Unleashed.Customer { CustomerCode = customerId },
                CustomerRef = order.Reference,
                DeliveryCity = order.DeliveryAddress.City,
                DeliveryCountry = order.DeliveryAddress.Country,
                DeliveryInstruction = order.DeliveryAddress.DeliveryNote,
                DeliveryName = order.DeliveryAddress.AddressName,
                DeliveryPostCode = order.DeliveryAddress.Postcode,
                DeliveryRegion = order.DeliveryAddress.Region,
                DeliveryStreetAddress = order.DeliveryAddress.Line1,
                DeliveryStreetAddress2 = order.DeliveryAddress.Line2,
                DeliverySuburb = order.DeliveryAddress.Suburb,
                Guid = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                OrderStatus = OrderStatus,
                SubTotal = order.SubTotal,
                TaxRate = order.TaxRate,
                TaxTotal = order.TaxTotal,
                Total = order.Total,
                RequiredDate = order.DeliveryDateUTC,
                SalesOrderLines = salesOrderLines.ToArray(),
                SourceId = order.SourceId,
                CreatedBy = order.OrderBy,
                SalesPerson = string.IsNullOrEmpty(order.SalesPersonId) ? null : new SalesPerson { Guid = order.SalesPersonId }
            };

            var response = await unleashedClient.SubmitOrder(orderRequest);
            if (response.OrderNumber == null)
            {
                if (response.Items != null && response.Items.Length > 0)
                {
                    string error = $"Order submit failed with {response.Items.Length} errors.";
                    foreach (var errorItem in response.Items)
                    {
                        error += $" Field: {errorItem.Field}, Desc: {errorItem.Description}";
                    }
                    throw new Exception(error);
                }
                else
                {
                    throw new Exception("Order submit failed and did not return order number");
                }
            }

            return response.OrderNumber;
        }

        public async Task<OrderDetailsResponseModel> GetOrderDetails(string customerId, string orderNumber)
        {
            var salesOrder = await unleashedClient.GetOrderByOrderNumber(customerId, orderNumber);

            var model = new OrderDetailsResponseModel
            {
                DeliveryDate = salesOrder.RequiredDate,
                Id = salesOrder.Guid,
                LastModifiedDate = salesOrder.LastModifiedOn,
                DiscountsAndSurchargesTotal = new CurrencyValueModel { Value = 0, Currency = salesOrder.Currency.CurrencyCode },
                OrderBy = salesOrder.CreatedBy,
                OrderDate = salesOrder.OrderDate,
                OrderNumber = salesOrder.OrderNumber,
                Status = salesOrder.OrderStatus,
                SubTotal = new CurrencyValueModel { Value = salesOrder.SubTotal, Currency = salesOrder.Currency.CurrencyCode },
                TaxTotal = new CurrencyValueModel { Value = salesOrder.TaxTotal, Currency = salesOrder.Currency.CurrencyCode },
                Total = new CurrencyValueModel { Value = salesOrder.Total, Currency = salesOrder.Currency.CurrencyCode },
                OrderSource = salesOrder.SourceId,
                Comments = salesOrder.Comments,
                CompletedDate = salesOrder.CompletedDate,
                Reference = salesOrder.CustomerRef,
                ProductItems = salesOrder.SalesOrderLines.Select(x => new OrderDetailsProductItemResponseModel
                {
                    DiscountsAndSurchargesTotal = new CurrencyValueModel { Value = 0, Currency = salesOrder.Currency.CurrencyCode },
                    Name = x.Product.ProductDescription,
                    ProductId = x.Product.ProductCode,
                    Quantity = Convert.ToInt32(x.OrderQuantity),
                   // UnitPrice = new CurrencyValueModel { Value = x.UnitPrice, Currency = salesOrder.Currency.CurrencyCode }, //Obsolete
                    Price = new CurrencyValueModel { Value = x.UnitPrice, Currency = salesOrder.Currency.CurrencyCode }, //use Price instead
                    LandedUnitCost = new CurrencyValueModel { Value = x.LineTotal / x.OrderQuantity, Currency = salesOrder.Currency.CurrencyCode },
                    TaxRate = salesOrder.TaxRate,
                    SubTotal = new CurrencyValueModel { Value = x.LineTotal, Currency = salesOrder.Currency.CurrencyCode },
                    TaxTotal = new CurrencyValueModel { Value = x.LineTax, Currency = salesOrder.Currency.CurrencyCode },

                }).ToArray(),

                DeliveryAddress = new OrderAddressModel
                {
                    AddressName = salesOrder.DeliveryName,
                    City = salesOrder.DeliveryCity,
                    Country = salesOrder.DeliveryCountry,
                    DeliveryNote = salesOrder.DeliveryInstruction,
                    Line1 = salesOrder.DeliveryStreetAddress,
                    Line2 = salesOrder.DeliveryStreetAddress2,
                    Postcode = salesOrder.DeliveryPostCode,
                    Region = salesOrder.DeliveryRegion,
                    Suburb = salesOrder.DeliverySuburb
                },
                NumberOfItems = salesOrder.SalesOrderLines.Count()
            };

            return model;
        }

        public async Task<OrderListResponseModel> GetOrders(string customerId, int pageNo, int pageLimit, OrderStatusEnum filterByStatus)
        {
            string status = filterByStatus.ToString();

            if (filterByStatus == OrderStatusEnum.Open)
            {
                status = "Parked,Placed";
            }
            else if (filterByStatus == OrderStatusEnum.All)
            {
                status = "";
            }

            var pagedOrders = await unleashedClient.GetOrders(customerId, pageNo, pageLimit, status);

            var model = new OrderListResponseModel
            {
                TotalCount = pagedOrders.Pagination.NumberOfItems,
                Items = pagedOrders.SalesOrders.Select(x => new OrderResponseModel
                {
                    DeliveryDate = x.RequiredDate,
                    NumberOfItems = x.SalesOrderLines.Count(),
                    OrderDate = x.OrderDate,
                    OrderNumber = x.OrderNumber,
                    OrderSource = x.SourceId,
                    Reference = x.CustomerRef,
                    Status = x.OrderStatus,
                    CompletedDate = x.CompletedDate,
                    Total = new CurrencyValueModel { Value = x.Total, Currency = x.Currency.CurrencyCode },
                    ItemProductIds  = x.SalesOrderLines.Where(y=>y.Product.ProductCode != null).Select(y=>y.Product.ProductCode).ToArray()
                }).ToArray()
            };

            return model;
        }

        //Note: There will be a performance issue later with passing in max page size when customers have many orders.
        // cannot call unleashed atm to to sort by order number or order date, by default sort by last modified
        //so getting all orders by int.maxValue (so it's just one call).
        //Unleashed will return page size of 1000
        public async Task<OrderDetailsResponseModel> GetLastOrder(string customerId)
        {
            var allOrders = await unleashedClient.GetOrders(customerId, 1, int.MaxValue, "");

            var lastOrder = allOrders.SalesOrders.OrderByDescending(x => x.CreatedOn).FirstOrDefault();

            if (lastOrder == null)
            {
                return null;
            }

            var model = new OrderDetailsResponseModel()
            {
                Comments = lastOrder.Comments,
                CompletedDate = lastOrder.CompletedDate,
                DeliveryAddress = new OrderAddressModel
                {
                    AddressName = lastOrder.DeliveryName,
                    City = lastOrder.DeliveryCity,
                    Country = lastOrder.DeliveryCountry,
                    DeliveryNote = lastOrder.DeliveryInstruction,
                    Line1 = lastOrder.DeliveryStreetAddress,
                    Line2 = lastOrder.DeliveryStreetAddress2,
                    Postcode = lastOrder.DeliveryPostCode,
                    Region = lastOrder.DeliveryRegion,
                    Suburb = lastOrder.DeliverySuburb
                },
                DeliveryDate = lastOrder.RequiredDate,
                OrderDate = lastOrder.OrderDate,
                OrderNumber = lastOrder.OrderNumber,
                OrderBy = lastOrder.CreatedBy,
                LastModifiedDate = lastOrder.LastModifiedOn,
                OrderSource = lastOrder.SourceId,
                Reference = lastOrder.CustomerRef,
                Status = lastOrder.OrderStatus,
                DiscountsAndSurchargesTotal = new CurrencyValueModel { Value = 0, Currency = lastOrder.Currency.CurrencyCode },
                Id = lastOrder.Guid,
                SubTotal = new CurrencyValueModel { Value = lastOrder.SubTotal, Currency = lastOrder.Currency.CurrencyCode },
                TaxTotal = new CurrencyValueModel { Value = lastOrder.TaxTotal, Currency = lastOrder.Currency.CurrencyCode },
                Total = new CurrencyValueModel { Value = lastOrder.Total, Currency = lastOrder.Currency.CurrencyCode },
                NumberOfItems = lastOrder.SalesOrderLines.Count(),
                ProductItems = lastOrder.SalesOrderLines.Select(x => new OrderDetailsProductItemResponseModel
                {
                    DiscountsAndSurchargesTotal = new CurrencyValueModel { Value = 0, Currency = lastOrder.Currency.CurrencyCode },
                    Name = x.Product.ProductDescription,
                    ProductId = x.Product.ProductCode,
                    Quantity = Convert.ToInt32(x.OrderQuantity),
                    TaxRate = lastOrder.TaxRate,
                    //UnitPrice = new CurrencyValueModel { Value = x.UnitPrice, Currency = lastOrder.Currency.CurrencyCode }, //Deprecating use price instead to be inline with cart model
                    Price = new CurrencyValueModel { Value = x.UnitPrice, Currency = lastOrder.Currency.CurrencyCode },
                    SubTotal = new CurrencyValueModel { Value = x.LineTotal, Currency = lastOrder.Currency.CurrencyCode },
                    LandedUnitCost = new CurrencyValueModel { Value = (x.LineTotal / x.OrderQuantity), Currency = lastOrder.Currency.CurrencyCode },
                    TaxTotal = new CurrencyValueModel { Value = (x.LineTax), Currency = lastOrder.Currency.CurrencyCode },
                }).ToArray()
            };

            return model;

        }

        // Note: There will be a performance issue later with passing in max page size when customers have many orders.
        // Unleashed cannot sort by required date
        // so getting all orders by int.maxValue (so it's just one call).
        // Unleashed will return page size of 1000 so this isn't a viable longterm solution
        public async Task<List<OrderSummaryResponseModel>> GetNextUndeliveredOrder(string customerId)
        {
            var orders = await unleashedClient.GetOrderSummaries(customerId, 1, int.MaxValue, UndeliverdOrderStatus);
            var nextUndeliveredOrder = orders.SalesOrders.Where(x => x.RequiredDate.Date >= DateTime.UtcNow.Date).OrderBy(x => x.RequiredDate).FirstOrDefault();

            if (nextUndeliveredOrder == null)
            {
                return new List<OrderSummaryResponseModel>();
            }

            var model = new List<OrderSummaryResponseModel>
            {
                new OrderSummaryResponseModel
                {
                    OrderNumber = nextUndeliveredOrder.OrderNumber,
                    DeliveryDate = nextUndeliveredOrder.RequiredDate
                }
            };

            return model;
        }

        public async Task<DeliveryInformationResponseModel> GetDeliveryInformation()
        {
            var info = await contentfulService.GetDeliveryInformation();

            if (info == null) return null;

            var model = new DeliveryInformationResponseModel
            {
                BodyText1 = info.BodyText1,
                BodyText2 = info.BodyText2 ?? "",
                BodyText3 = info.BodyText3 ?? "",
                Link = info.Link ?? "",
                LinkText = info.LinkText ?? info.Link ?? "",
                RowArray1 = info.RowArray1== null ? new DeliveryInfoLabelValueModel[] { } : info.RowArray1.Select(x=> new DeliveryInfoLabelValueModel { Key = x.Label, Value = x.Value }).ToArray(),
                RowArray2 = info.RowArray1 == null ? new DeliveryInfoLabelValueModel[] { } : info.RowArray2.Select(x => new DeliveryInfoLabelValueModel { Key = x.Label,Value = x.Value }).ToArray(),
            };

            return model;
        }
    }
}
