using Sample.Order.BE.Api.Controllers;
using Sample.Order.BE.Api.DTOs;
using Sample.Order.BE.Business.Configs;
using Sample.Order.BE.Business.Enums;
using Sample.Order.BE.Business.Services;
using Sample.Order.BE.Business.Services.Interfaces;
using Sample.Order.BE.Data.HttpClients.Interfaces;
using Sample.Order.BE.Data.Models.Contentful;
using Sample.Order.BE.Data.Models.Unleashed;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Order.BE.Api.Test
{
    [TestClass]
    public class OrderControllerTest
    {
        private readonly IOrderService service;
        private readonly OrderController controller;
        readonly Mock<IUnleashedClient> mockUnleashedClient;
        private readonly Mock<IContentfulService> mockContentfulService;
        IOptions<MessagesConfig> messages;
        public OrderControllerTest()
        {
            messages = Options.Create<MessagesConfig>(new MessagesConfig()
            {
                GetCart_ProductNotFound = "not found",
                ProductOutOfStock = "out of stock"
            });

            // Instantiate Mapper
            var mapperConfig = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new AutoMapperProfile());
            });
            var mapper = mapperConfig.CreateMapper();

            mockUnleashedClient = new Mock<IUnleashedClient>();
            mockContentfulService = new Mock<IContentfulService>();

            var serviceLogger = new Mock<ILogger<OrderService>>();
            var logger = new Mock<ILogger<OrderController>>();
            service = new OrderService(mockUnleashedClient.Object, mockContentfulService.Object, messages, serviceLogger.Object);
            controller = new OrderController(logger.Object, mapper, service);
        }

        [TestMethod]
        public void SimulateOrder_Success()
        {
            var customerId = "abc123";
            var currency = "AUD";
            var payload = new OrderSimulateRequestDTO
            {
                Currency = currency,
                DeliveryDateUTC = DateTime.MaxValue,
                TaxRate = (decimal)0.2,
                ProductItems = new OrderSimulateItemRequestDTO[]
                {
                    new OrderSimulateItemRequestDTO
                    {
                        Brand = "b1",
                        ImageUrl = "i1",
                        IsInStock = true,
                        Name = "p1",
                        ProductId = "PIL-30F",
                        Quantity = 100,
                        Prices = new ProductPriceDTO[] {
                            new ProductPriceDTO  {
                            BaseUnitQty =1,
                            Name = "keg",
                            Price = new CurrencyValueDTO { Currency = currency, Value = 10 }
                            }
                        }
                    },
                    new OrderSimulateItemRequestDTO
                    {
                        Brand = "b2",
                        ImageUrl = "i2",
                        IsInStock = true,
                        Name = "p2",
                        ProductId = "PIL-30F",
                        Quantity = 50,
                        Prices = new ProductPriceDTO[] {
                            new ProductPriceDTO  {
                            BaseUnitQty =1,
                            Name = "keg",
                            Price = new CurrencyValueDTO { Currency = currency, Value = 5 }
                            }
                        }
                    }
                }
            };

            var response = controller.SimulateCustomerOrder(customerId, payload);

            var result = (OkObjectResult)response.Result;
            var content = (OrderSimulateResponseDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");

            Assert.AreEqual(2, content.ProductItems.Count(), "There should be 2 product items");

            //product 1
            Assert.AreEqual(payload.ProductItems[0].Brand, content.ProductItems[0].Brand, "Brand should match");
            Assert.AreEqual(payload.ProductItems[0].ImageUrl, content.ProductItems[0].ImageUrl, "Image url should match");
            Assert.AreEqual(payload.ProductItems[0].Name, content.ProductItems[0].Name, "Product name should match");
            Assert.AreEqual(0, content.ProductItems[0].DiscountsAndSurchargesTotal.Value, "Disocunt and surcharge total should be 0");
            Assert.AreEqual(0, content.ProductItems[0].Messages.Count(), "There should be 0 messages");
            Assert.AreEqual(payload.ProductItems[0].Prices[0].Price.Value, content.ProductItems[0].Prices[0].Price.Value, "product price should match");
            Assert.AreEqual(payload.ProductItems[0].ProductId, content.ProductItems[0].ProductId, "ProductId should match");
            Assert.AreEqual(1000, content.ProductItems[0].SubTotal.Value, "SubTotal should be qty * price of product");
            Assert.AreEqual(payload.TaxRate, content.ProductItems[0].TaxRate, "Tax rate should match customer tax rate");
            Assert.AreEqual(200, content.ProductItems[0].TaxTotal.Value, "TaxTotal should be subtotal * tax rate");

            //Product 2
            Assert.AreEqual(payload.ProductItems[1].Brand, content.ProductItems[1].Brand, "Brand should match");
            Assert.AreEqual(payload.ProductItems[1].ImageUrl, content.ProductItems[1].ImageUrl, "Image url should match");
            Assert.AreEqual(payload.ProductItems[1].Name, content.ProductItems[1].Name, "Product name should match");
            Assert.AreEqual(0, content.ProductItems[1].DiscountsAndSurchargesTotal.Value, "Disocunt and surcharge total should be 0");
            Assert.AreEqual(0, content.ProductItems[1].Messages.Count(), "There should be 0 messages");
            Assert.AreEqual(payload.ProductItems[1].Prices[0].Price.Value, content.ProductItems[1].Prices[0].Price.Value, "product price should match");
            Assert.AreEqual(payload.ProductItems[1].ProductId, content.ProductItems[1].ProductId, "ProductId should match");
            Assert.AreEqual(250, content.ProductItems[1].SubTotal.Value, "SubTotal should be qty * price of product");
            Assert.AreEqual(payload.TaxRate, content.ProductItems[1].TaxRate, "Tax rate should match customer tax rate");
            Assert.AreEqual(50, content.ProductItems[1].TaxTotal.Value, "TaxTotal should be subtotal * tax rate");

            Assert.IsFalse(content.Messages.Any(), "There shouldn't be any messages");
            Assert.AreEqual(currency, content.DiscountsAndSurchargesTotal.Currency, "DiscountsAndSurchargesTotal Currency code is incorrect");
            Assert.AreEqual(0, content.DiscountsAndSurchargesTotal.Value, "Discount and Surcharge total should be 0 for UK");
            Assert.AreEqual(currency, content.SubTotal.Currency, "Subtotal currency code is incorrect");
            Assert.AreEqual(1250, content.SubTotal.Value, "Subtotal of cart is incorrect");
            Assert.AreEqual(currency, content.TaxTotal.Currency, "Taxtotal currency code is incorrect");
            Assert.AreEqual(250, content.TaxTotal.Value, "TaxTotal of cart is incorrect");
            Assert.AreEqual(currency, content.Total.Currency, "Total currency code is incorrect");
            Assert.AreEqual(1500, content.Total.Value, "Total for cart is incorrect");
        }

        //product 1 not available - so only the product id and quantity is passed in
        [TestMethod]
        public void SimulateOrder_ProductNoLongerAvailable_Message_Success()
        {
            var customerId = "abc123";
            var currency = "AUD";
            var payload = new OrderSimulateRequestDTO
            {
                Currency = currency,
                DeliveryDateUTC = DateTime.MaxValue,
                TaxRate = (decimal)0.2,
                ProductItems = new OrderSimulateItemRequestDTO[]
                {
                    new OrderSimulateItemRequestDTO
                    {
                        ProductId = "PIL-30F",
                        Quantity = 100,
                        Brand = "b1",
                        ImageUrl = "i1",
                        IsInvalid = true
                    },
                    new OrderSimulateItemRequestDTO
                    {
                        Brand = "b2",
                        ImageUrl = "i2",
                        IsInStock = true,
                        Name = "p2",
                        ProductId = "PIL-30F",
                        Quantity = 50,
                        Prices = new ProductPriceDTO[] {
                            new ProductPriceDTO  {
                            BaseUnitQty =1,
                            Name = "keg",
                            Price = new CurrencyValueDTO { Currency = currency, Value = 5 }
                            }
                        }
                    }
                }
            };

            var response = controller.SimulateCustomerOrder(customerId, payload);

            var result = (OkObjectResult)response.Result;
            var content = (OrderSimulateResponseDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");

            Assert.AreEqual(2, content.ProductItems.Count(), "There should be 2 product items");

            //Product 1 - not found
            Assert.AreEqual(payload.ProductItems[0].Brand, content.ProductItems[0].Brand, "Brand should match even if product not found");
            Assert.AreEqual(payload.ProductItems[0].ImageUrl, content.ProductItems[0].ImageUrl, "Image url should match even if product not found");
            Assert.AreEqual(payload.ProductItems[0].Name, content.ProductItems[0].Name, "Product name should match even if prouct not found");
            Assert.AreEqual(0, content.ProductItems[0].DiscountsAndSurchargesTotal.Value, "Disocunt and surcharge total should be 0");
            Assert.IsTrue(!string.IsNullOrEmpty(content.ProductItems[0].Messages[0].Text), "Message should be product not found");
            Assert.AreEqual(messages.Value.GetCart_ProductNotFound, content.ProductItems[0].Messages[0].Text, "Message should error message type");
            Assert.AreEqual(MessageTypeEnum.Error.ToString(), content.ProductItems[0].Messages[0].MessageType, "Message should error message type");
            Assert.IsFalse(content.ProductItems[0].Prices.Any(), "product prices should be empty array if product not found");
            Assert.AreEqual(payload.ProductItems[0].ProductId, content.ProductItems[0].ProductId, "ProductId should match");
            Assert.IsNull(content.ProductItems[0].SubTotal, "SubTotal is null when product not found");
            Assert.AreEqual(payload.TaxRate, content.ProductItems[0].TaxRate, "Tax rate should match customer tax rate");
            Assert.IsNull(content.ProductItems[0].TaxTotal, "TaxTotal is null when product not found");

            //Product 2
            Assert.AreEqual(payload.ProductItems[1].Brand, content.ProductItems[1].Brand, "Brand should match");
            Assert.AreEqual(payload.ProductItems[1].ImageUrl, content.ProductItems[1].ImageUrl, "Image url should match");
            Assert.AreEqual(payload.ProductItems[1].Name, content.ProductItems[1].Name, "Product name should match");
            Assert.AreEqual(0, content.ProductItems[1].DiscountsAndSurchargesTotal.Value, "Disocunt and surcharge total should be 0");
            Assert.AreEqual(0, content.ProductItems[1].Messages.Count(), "There should be 0 messages");
            Assert.AreEqual(payload.ProductItems[1].Prices[0].Price.Value, content.ProductItems[1].Prices[0].Price.Value, "product price should match");
            Assert.AreEqual(payload.ProductItems[1].ProductId, content.ProductItems[1].ProductId, "ProductId should match");
            Assert.AreEqual(250, content.ProductItems[1].SubTotal.Value, "SubTotal should be qty * price of product");
            Assert.AreEqual(payload.TaxRate, content.ProductItems[1].TaxRate, "Tax rate should match customer tax rate");
            Assert.AreEqual(50, content.ProductItems[1].TaxTotal.Value, "TaxTotal should be subtotal * tax rate");

            Assert.IsFalse(content.Messages.Any(), "There shouldn't be any messages");
            Assert.AreEqual(currency, content.DiscountsAndSurchargesTotal.Currency, "DiscountsAndSurchargesTotal Currency code is incorrect");
            Assert.AreEqual(0, content.DiscountsAndSurchargesTotal.Value, "Discount and Surcharge total should be 0 for UK");
            Assert.AreEqual(currency, content.SubTotal.Currency, "Subtotal currency code is incorrect");
            Assert.AreEqual(250, content.SubTotal.Value, "Subtotal of cart is incorrect");
            Assert.AreEqual(currency, content.TaxTotal.Currency, "Taxtotal currency code is incorrect");
            Assert.AreEqual(50, content.TaxTotal.Value, "TaxTotal of cart is incorrect");
            Assert.AreEqual(currency, content.Total.Currency, "Total currency code is incorrect");
            Assert.AreEqual(300, content.Total.Value, "Total for cart is incorrect");
        }

        //product 2 out of stock
        [TestMethod]
        public void SimulateOrder_ProductOutOfStock_Message_Success()
        {

            var customerId = "abc123";
            var currency = "AUD";
            var payload = new OrderSimulateRequestDTO
            {
                Currency = currency,
                DeliveryDateUTC = DateTime.MaxValue,
                TaxRate = (decimal)0.2,
                ProductItems = new OrderSimulateItemRequestDTO[]
                {
                    new OrderSimulateItemRequestDTO
                    {
                        Brand = "b1",
                        ImageUrl = "i1",
                        IsInStock = true,
                        Name = "p1",
                        ProductId = "PIL-30F",
                        Quantity = 100,
                        Prices = new ProductPriceDTO[] {
                            new ProductPriceDTO  {
                            BaseUnitQty =1,
                            Name = "keg",
                            Price = new CurrencyValueDTO { Currency = currency, Value = 10 }
                            }
                        }
                    },
                    new OrderSimulateItemRequestDTO
                    {
                        Brand = "b2",
                        ImageUrl = "i2",
                        IsInStock = false,
                        Name = "p2",
                        ProductId = "PIL-30F",
                        Quantity = 50,
                        Prices = new ProductPriceDTO[] {
                            new ProductPriceDTO  {
                            BaseUnitQty =1,
                            Name = "keg",
                            Price = new CurrencyValueDTO { Currency = currency, Value = 5 }
                            }
                        }
                    }
                }
            };
            var response = controller.SimulateCustomerOrder(customerId, payload);

            var result = (OkObjectResult)response.Result;
            var content = (OrderSimulateResponseDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");

            Assert.AreEqual(2, content.ProductItems.Count(), "There should be 2 product items");

            //product 1
            Assert.AreEqual(payload.ProductItems[0].Brand, content.ProductItems[0].Brand, "Brand should match");
            Assert.AreEqual(payload.ProductItems[0].ImageUrl, content.ProductItems[0].ImageUrl, "Image url should match");
            Assert.AreEqual(payload.ProductItems[0].Name, content.ProductItems[0].Name, "Product name should match");
            Assert.AreEqual(0, content.ProductItems[0].DiscountsAndSurchargesTotal.Value, "Disocunt and surcharge total should be 0");
            Assert.AreEqual(0, content.ProductItems[0].Messages.Count(), "There should be 0 messages");
            Assert.AreEqual(payload.ProductItems[0].Prices[0].Price.Value, content.ProductItems[0].Prices[0].Price.Value, "product price should match");
            Assert.AreEqual(payload.ProductItems[0].ProductId, content.ProductItems[0].ProductId, "ProductId should match");
            Assert.AreEqual(1000, content.ProductItems[0].SubTotal.Value, "SubTotal should be qty * price of product");
            Assert.AreEqual(payload.TaxRate, content.ProductItems[0].TaxRate, "Tax rate should match customer tax rate");
            Assert.AreEqual(200, content.ProductItems[0].TaxTotal.Value, "TaxTotal should be subtotal * tax rate");

            //Product 2 - out of stock
            Assert.AreEqual(payload.ProductItems[1].Brand, content.ProductItems[1].Brand, "Brand should match");
            Assert.AreEqual(payload.ProductItems[1].ImageUrl, content.ProductItems[1].ImageUrl, "Image url should match");
            Assert.AreEqual(payload.ProductItems[1].Name, content.ProductItems[1].Name, "Product name should match");
            Assert.AreEqual(0, content.ProductItems[1].DiscountsAndSurchargesTotal.Value, "Disocunt and surcharge total should be 0");
            Assert.AreEqual(1, content.ProductItems[1].Messages.Count(), "There should be 1 message");
            Assert.IsTrue(!string.IsNullOrEmpty(content.ProductItems[1].Messages[0].Text), "There should be 1 message");
            Assert.AreEqual(MessageTypeEnum.Information.ToString(), content.ProductItems[1].Messages[0].MessageType, "Message type should be a warning");
            Assert.AreEqual(payload.ProductItems[1].Prices[0].Price.Value, content.ProductItems[1].Prices[0].Price.Value, "product price should match");
            Assert.AreEqual(payload.ProductItems[1].ProductId, content.ProductItems[1].ProductId, "ProductId should match");
            Assert.AreEqual(250, content.ProductItems[1].SubTotal.Value, "SubTotal should be qty * price of product");
            Assert.AreEqual(payload.TaxRate, content.ProductItems[1].TaxRate, "Tax rate should match customer tax rate");
            Assert.AreEqual(50, content.ProductItems[1].TaxTotal.Value, "TaxTotal should be subtotal * tax rate");

            Assert.IsFalse(content.Messages.Any(), "There shouldn't be any messages");
            Assert.AreEqual(currency, content.DiscountsAndSurchargesTotal.Currency, "DiscountsAndSurchargesTotal Currency code is incorrect");
            Assert.AreEqual(0, content.DiscountsAndSurchargesTotal.Value, "Discount and Surcharge total should be 0 for UK");
            Assert.AreEqual(currency, content.SubTotal.Currency, "Subtotal currency code is incorrect");
            Assert.AreEqual(1250, content.SubTotal.Value, "Subtotal of cart is incorrect");
            Assert.AreEqual(currency, content.TaxTotal.Currency, "Taxtotal currency code is incorrect");
            Assert.AreEqual(250, content.TaxTotal.Value, "TaxTotal of cart is incorrect");
            Assert.AreEqual(currency, content.Total.Currency, "Total currency code is incorrect");
            Assert.AreEqual(1500, content.Total.Value, "Total for cart is incorrect");
        }

        [TestMethod]
        public async Task SubmitOrder_Success()
        {
            var customerId = "Tesco";
            var payload = new OrderRequestDTO
            {
                Comments = "",
                DeliveryDateUTC = DateTime.Now,
                Reference = "",
                DeliveryAddress = new OrderAddressDTO
                {
                    AddressName = "Office",
                    Line1 = "Building",
                    Line2 = "",
                    Postcode = "E1 2XC",
                    Suburb = "",
                    Region = "",
                    City = "London",
                    Country = "UK",
                    DeliveryNote = "",
                },
                ProductItems = new OrderItemRequestDTO[]
                {
                    new OrderItemRequestDTO {
                    LineTotal = 100,
                    ProductId = "X",
                    Quantity = 10,
                    TaxTotal = 20,
                    UnitPrice = 10
                    },
                      new OrderItemRequestDTO {
                    LineTotal = 10,
                    ProductId = "Y",
                    Quantity = 1,
                    TaxTotal = 2,
                    UnitPrice = 1
                    }
                },
                SubTotal = 132,
                TaxRate = (decimal)0.2,
                TaxTotal = 22,
                Total = 132,
                OrderBy = "Full Name",
                SourceId = "App"
            };

            var salesOrderResponse = new SalesOrder
            {
                Comments = "",
                RequiredDate = DateTime.Now,
                DeliveryInstruction = "",
                CustomerRef = "",
                DeliveryName = "Office",
                DeliveryStreetAddress = "Building",
                DeliveryStreetAddress2 = "",
                DeliveryPostCode = "E1 2XC",
                DeliverySuburb = "",
                DeliveryRegion = "",
                DeliveryCity = "London",
                DeliveryCountry = "UK",
                SalesOrderLines = new SalesOrderLine[]
                {
                    new SalesOrderLine {
                    LineTotal = 100,
                    Product = new Product { ProductCode = "X" },
                    OrderQuantity = 10,
                    LineTax = 20,
                    UnitPrice = 10,
                    LineNumber = 1
                    },
                    new SalesOrderLine {
                    LineTotal = 10,
                    Product = new Product { ProductCode = "Y" },
                    OrderQuantity = 1,
                    LineTax = 2,
                    UnitPrice = 1,
                    LineNumber = 2
                    }
                },
                SubTotal = 132,
                TaxRate = (decimal)0.2,
                TaxTotal = 22,
                Total = 132,
                Customer = new Data.Models.Unleashed.Customer { CustomerCode = customerId },
                Guid = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Parked",
                OrderNumber = "ord-123",
                SourceId = "App",
                CreatedBy = "Full Name",
                CompletedDate = null,
                Currency = new Currency { CurrencyCode = "AUD" }
            };

            mockUnleashedClient.Setup(x => x.SubmitOrder(It.IsAny<SalesOrder>())).Returns(Task.FromResult(salesOrderResponse));

            var response = await controller.SubmitCustomerOrder(customerId, payload);

            var result = (OkObjectResult)response.Result;
            var orderNumber = (string)result.Value;


            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(salesOrderResponse.OrderNumber, orderNumber);
        }

        [TestMethod]
        public async Task SubmitOrder_Failure()
        {
            mockUnleashedClient.Setup(x => x.SubmitOrder(It.IsAny<SalesOrder>())).Throws(new Exception());

            var response = await controller.SubmitCustomerOrder(It.IsAny<string>(), It.IsAny<OrderRequestDTO>());

            var result = (ObjectResult)response.Result;

            // Validate status
            Assert.AreEqual(400, result.StatusCode, "Response status code should be error");

            // Validate error message
            Assert.IsNotNull(result.Value, "Response error message not found");
        }

        [TestMethod]
        public async Task GetOrderDetails_Success()
        {

            var order = new SalesOrder()
            {
                Comments = "comments",
                CompletedDate = DateTime.Now,
                CreatedBy = "full name",
                CreatedOn = DateTime.Now,
                Currency = new Currency { CurrencyCode = "AUD" },
                Customer = new Data.Models.Unleashed.Customer { CustomerCode = "Small Beer Ltd" },
                CustomerRef = "ref",
                DeliveryCity = "city",
                DeliveryCountry = "country",
                DeliveryInstruction = "instructions",
                DeliveryStreetAddress = "street",
                DeliveryName = "delivery name",
                DeliveryRegion = "region",
                DeliveryPostCode = "postcode",
                DeliveryStreetAddress2 = "street 2",
                DeliverySuburb = "sub",
                OrderDate = DateTime.Now,
                OrderNumber = "SO-234",
                RequiredDate = DateTime.Now.AddDays(1),
                OrderStatus = "Parked",
                SourceId = "App",
                Total = 110,
                TaxRate = (decimal)0.2,
                TaxTotal = 10,
                SubTotal = 100,
                LastModifiedOn = DateTime.Now,
                SalesOrderLines = new SalesOrderLine[] {new SalesOrderLine {
                    LineNumber = 1,
                    LineTax = 10,
                    LineTotal = 100,
                    OrderQuantity =1,
                    Product = new Product { ProductCode = "P1"},
                    UnitPrice = 100
                 }
               }
            };
            mockUnleashedClient.Setup(x => x.GetOrderByOrderNumber("Small Beer Ltd", "SO-234")).Returns(Task.FromResult(order));
            var response = await controller.GetCustomerOrderDetails("Small Beer Ltd", "SO-234");
            var result = (OkObjectResult)response.Result;
            var content = (OrderDetailsResponseDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");

            Assert.AreEqual(order.CustomerRef, content.Reference);
            Assert.AreEqual(order.Comments, content.Comments);
            Assert.AreEqual(order.CompletedDate, content.CompletedDate);
            Assert.AreEqual(order.CreatedBy, content.OrderBy);
            Assert.AreEqual(order.DeliveryCountry, content.DeliveryAddress.Country);
            Assert.AreEqual(order.DeliveryCity, content.DeliveryAddress.City);
            Assert.AreEqual(order.DeliveryInstruction, content.DeliveryAddress.DeliveryNote);
            Assert.AreEqual(order.DeliveryName, content.DeliveryAddress.AddressName);
            Assert.AreEqual(order.DeliveryStreetAddress, content.DeliveryAddress.Line1);
            Assert.AreEqual(order.DeliveryStreetAddress2, content.DeliveryAddress.Line2);
            Assert.AreEqual(order.DeliverySuburb, content.DeliveryAddress.Suburb);
            Assert.AreEqual(order.DeliveryRegion, content.DeliveryAddress.Region);
            Assert.AreEqual(order.DeliveryPostCode, content.DeliveryAddress.Postcode);
            Assert.AreEqual(order.LastModifiedOn, content.LastModifiedDate);
            Assert.AreEqual(order.OrderDate, content.OrderDate);
            Assert.AreEqual(order.OrderNumber, content.OrderNumber);
            Assert.AreEqual(order.OrderStatus, content.Status);
            Assert.AreEqual(order.RequiredDate, content.DeliveryDate);
            Assert.AreEqual(order.SourceId, content.OrderSource);
            Assert.AreEqual(order.Currency.CurrencyCode, content.SubTotal.Currency);
            Assert.AreEqual(order.SubTotal, content.SubTotal.Value);

            //     Assert.AreEqual(order.SubTotal, content.DiscountsAndSurchargesTotal);
            Assert.AreEqual(order.Currency.CurrencyCode, content.Total.Currency);
            Assert.AreEqual(order.Total, content.Total.Value);
            Assert.AreEqual(order.SalesOrderLines[0].LineTax, content.ProductItems[0].TaxTotal.Value);
            Assert.AreEqual(order.TaxRate, content.ProductItems[0].TaxRate);
            //     Assert.AreEqual(order.TaxRate, content.ProductItems[0].DiscountsAndSurchargesTotal);
            Assert.AreEqual(order.SalesOrderLines[0].Product.ProductDescription, content.ProductItems[0].Name);
            Assert.AreEqual(order.SalesOrderLines[0].LineTotal / order.SalesOrderLines[0].OrderQuantity, content.ProductItems[0].LandedUnitCost.Value);
            Assert.AreEqual(order.SalesOrderLines[0].Product.ProductCode, content.ProductItems[0].ProductId);
            Assert.AreEqual(order.SalesOrderLines[0].Product.ProductDescription, content.ProductItems[0].Name);
            Assert.AreEqual(order.SalesOrderLines[0].OrderQuantity, content.ProductItems[0].Quantity);
            Assert.AreEqual(order.SalesOrderLines[0].LineTotal, content.ProductItems[0].SubTotal.Value);
            Assert.AreEqual(order.SalesOrderLines[0].LineTax, content.ProductItems[0].TaxTotal.Value);
            Assert.AreEqual(order.SalesOrderLines[0].UnitPrice, content.ProductItems[0].Price.Value);
        }

        [TestMethod]
        public async Task GetOrderDetails_Failure()
        {
            mockUnleashedClient.Setup(x => x.GetOrderByOrderNumber(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            var response = await controller.GetCustomerOrderDetails(It.IsAny<string>(), It.IsAny<string>());

            var result = (ObjectResult)response.Result;

            // Validate status
            Assert.AreEqual(400, result.StatusCode, "Response status code should be error");

            // Validate error message
            Assert.IsNotNull(result.Value, "Response error message not found");
        }

        [TestMethod]
        public async Task GetOrderDetails_Failure_NotFound()
        {
            // Setup Mock Unleashed Client
            mockUnleashedClient.Setup(x => x.GetOrderByOrderNumber(It.IsAny<string>(), It.IsAny<string>())).Throws(new ArgumentException());

            // Call api
            var response = await controller.GetCustomerOrderDetails(It.IsAny<string>(), It.IsAny<string>());
            var result = (ObjectResult)response.Result;

            // Validate status
            Assert.AreEqual(400, result.StatusCode, "Response status code should be 400");
        }

        [TestMethod]
        public async Task GetOrders_Filter_All_Success()
        {
            var customerId = "ABC123";
            var orderList = new SalesOrderResult
            {
                ErrorDescription = null,
                Pagination = new Pagination { NumberOfItems = 10, PageSize = 1, PageNumber = 1, NumberOfPages = 10 },
                SalesOrders = new System.Collections.Generic.List<SalesOrder> {
                new SalesOrder
                {
                       Comments = "comments",
                CompletedDate = DateTime.Now,
                CreatedBy = "full name",
                CreatedOn = DateTime.Now,
                Currency = new Currency { CurrencyCode = "AUD" },
                Customer = new Data.Models.Unleashed.Customer { CustomerCode =customerId},
                CustomerRef = "ref",
                DeliveryCity = "city",
                DeliveryCountry = "country",
                DeliveryInstruction = "instructions",
                DeliveryStreetAddress = "street",
                DeliveryName = "delivery name",
                DeliveryRegion = "region",
                DeliveryPostCode = "postcode",
                DeliveryStreetAddress2 = "street 2",
                DeliverySuburb = "sub",
                OrderDate = DateTime.Now,
                OrderNumber = "SO-234",
                RequiredDate = DateTime.Now.AddDays(1),
                OrderStatus = "Parked",
                SourceId = "App",
                Total = 110,
                TaxRate = (decimal)0.2,
                TaxTotal = 10,
                SubTotal = 100,
                LastModifiedOn = DateTime.Now,
                SalesOrderLines = new SalesOrderLine[] {new SalesOrderLine {
                    LineNumber = 1,
                    LineTax = 10,
                    LineTotal = 100,
                    OrderQuantity =1,
                    Product = new Product { ProductCode = "P1"},
                    UnitPrice = 100
                 }
               }
                }
            }
            };
            mockUnleashedClient.Setup(x => x.GetOrders(customerId, 1, 2, "")).Returns(Task.FromResult(orderList));
            var response = await controller.GetCustomerOrderList(customerId, 1, 2, OrderStatusEnum.All);

            var result = (OkObjectResult)response.Result;
            var content = (OrderListResponseDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");

            Assert.AreEqual(orderList.Pagination.NumberOfItems, content.TotalCount);
            Assert.AreEqual(orderList.SalesOrders[0].CompletedDate, content.Items[0].CompletedDate, "Completed date should match");
            Assert.AreEqual(orderList.SalesOrders[0].RequiredDate, content.Items[0].DeliveryDate);
            Assert.AreEqual(orderList.SalesOrders[0].SalesOrderLines.Count(), content.Items[0].NumberOfItems);
            Assert.AreEqual(orderList.SalesOrders[0].OrderDate, content.Items[0].OrderDate);
            Assert.AreEqual(orderList.SalesOrders[0].OrderNumber, content.Items[0].OrderNumber);
            Assert.AreEqual(orderList.SalesOrders[0].SourceId, content.Items[0].OrderSource);
            Assert.AreEqual(orderList.SalesOrders[0].CustomerRef, content.Items[0].Reference);
            Assert.AreEqual(orderList.SalesOrders[0].OrderStatus, content.Items[0].Status);
            Assert.AreEqual(orderList.SalesOrders[0].Currency.CurrencyCode, content.Items[0].Total.Currency);
            Assert.AreEqual(orderList.SalesOrders[0].Total, content.Items[0].Total.Value);
            CollectionAssert.AreEqual(new[] { "P1" }, content.Items[0].ItemProductIds);
        }

        [TestMethod]
        public async Task GetOrders_Filter_Open_Success()
        {
            var customerId = "ABC123";
            var orderList = new SalesOrderResult
            {
                ErrorDescription = null,
                Pagination = new Pagination { NumberOfItems = 10, PageSize = 1, PageNumber = 1, NumberOfPages = 10 },
                SalesOrders = new System.Collections.Generic.List<SalesOrder> {
                new SalesOrder
                {
                       Comments = "comments",
                CompletedDate = null,
                CreatedBy = "full name",
                CreatedOn = DateTime.Now,
                Currency = new Currency { CurrencyCode = "AUD" },
                Customer = new Data.Models.Unleashed.Customer { CustomerCode =customerId},
                CustomerRef = "ref",
                DeliveryCity = "city",
                DeliveryCountry = "country",
                DeliveryInstruction = "instructions",
                DeliveryStreetAddress = "street",
                DeliveryName = "delivery name",
                DeliveryRegion = "region",
                DeliveryPostCode = "postcode",
                DeliveryStreetAddress2 = "street 2",
                DeliverySuburb = "sub",
                OrderDate = DateTime.Now,
                OrderNumber = "SO-234",
                RequiredDate = DateTime.Now.AddDays(1),
                OrderStatus = "Parked",
                SourceId = "App",
                Total = 110,
                TaxRate = (decimal)0.2,
                TaxTotal = 10,
                SubTotal = 100,
                LastModifiedOn = DateTime.Now,
                SalesOrderLines = new SalesOrderLine[] {new SalesOrderLine {
                    LineNumber = 1,
                    LineTax = 10,
                    LineTotal = 100,
                    OrderQuantity =1,
                    Product = new Product { ProductCode = "P1"},
                    UnitPrice = 100
                 }
               }
                }
            }
            };
            mockUnleashedClient.Setup(x => x.GetOrders(customerId, 1, 2, "Parked,Placed")).Returns(Task.FromResult(orderList));
            var response = await controller.GetCustomerOrderList(customerId, 1, 2, OrderStatusEnum.Open);

            var result = (OkObjectResult)response.Result;
            var content = (OrderListResponseDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");

            Assert.AreEqual(orderList.Pagination.NumberOfItems, content.TotalCount);
            Assert.IsNull(content.Items[0].CompletedDate, "Completed date should be null");
            Assert.AreEqual(orderList.SalesOrders[0].RequiredDate, content.Items[0].DeliveryDate);
            Assert.AreEqual(orderList.SalesOrders[0].SalesOrderLines.Count(), content.Items[0].NumberOfItems);
            Assert.AreEqual(orderList.SalesOrders[0].OrderDate, content.Items[0].OrderDate);
            Assert.AreEqual(orderList.SalesOrders[0].OrderNumber, content.Items[0].OrderNumber);
            Assert.AreEqual(orderList.SalesOrders[0].SourceId, content.Items[0].OrderSource);
            Assert.AreEqual(orderList.SalesOrders[0].CustomerRef, content.Items[0].Reference);
            Assert.AreEqual(orderList.SalesOrders[0].OrderStatus, content.Items[0].Status);
            Assert.AreEqual(orderList.SalesOrders[0].Currency.CurrencyCode, content.Items[0].Total.Currency);
            Assert.AreEqual(orderList.SalesOrders[0].Total, content.Items[0].Total.Value);
            CollectionAssert.AreEqual(new[] {"P1" }, content.Items[0].ItemProductIds);
        }

        [TestMethod]
        public async Task GetOrders_NoResults_Success()
        {

            var customerId = "ABC123";
            var orderList = new SalesOrderResult
            {
                ErrorDescription = null,
                Pagination = new Pagination { NumberOfItems = 0, NumberOfPages = 0, PageNumber = 0, PageSize = 2 },
                SalesOrders = new System.Collections.Generic.List<SalesOrder>() { }
            };

            mockUnleashedClient.Setup(x => x.GetOrders(customerId, 1, 2, OrderStatusEnum.Backordered.ToString())).Returns(Task.FromResult(orderList));
            var response = await controller.GetCustomerOrderList(customerId, 1, 2, OrderStatusEnum.Backordered);

            var result = (OkObjectResult)response.Result;
            var content = (OrderListResponseDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");

            Assert.AreEqual(0, content.TotalCount);
            Assert.AreEqual(0, content.Items.Count());
        }

        [TestMethod]
        public async Task GetOrders_Failure()
        {
            mockUnleashedClient.Setup(x => x.GetOrders(It.IsAny<string>(), 1, 2, "")).Throws(new Exception());

            var response = await controller.GetCustomerOrderList(It.IsAny<string>(), 1, 2, OrderStatusEnum.All);

            var result = (ObjectResult)response.Result;

            // Validate status
            Assert.AreEqual(400, result.StatusCode, "Response status code should be error");

            // Validate error message
            Assert.IsNotNull(result.Value, "Response error message not found");
        }

        [TestMethod]
        public async Task GetLastOrder_Success()
        {
            var order = new SalesOrder()
            {
                Comments = "comments",
                CompletedDate = DateTime.Now,
                CreatedBy = "full name",
                CreatedOn = DateTime.Now,
                Currency = new Currency { CurrencyCode = "AUD" },
                Customer = new Data.Models.Unleashed.Customer { CustomerCode = "Small Beer Ltd" },
                CustomerRef = "ref",
                DeliveryCity = "city",
                DeliveryCountry = "country",
                DeliveryInstruction = "instructions",
                DeliveryStreetAddress = "street",
                DeliveryName = "delivery name",
                DeliveryRegion = "region",
                DeliveryPostCode = "postcode",
                DeliveryStreetAddress2 = "street 2",
                DeliverySuburb = "sub",
                OrderDate = DateTime.Now,
                OrderNumber = "SO-234",
                RequiredDate = DateTime.Now.AddDays(1),
                OrderStatus = "Parked",
                SourceId = "App",
                Total = 110,
                TaxRate = (decimal)0.2,
                TaxTotal = 10,
                SubTotal = 100,
                LastModifiedOn = DateTime.Now,
                SalesOrderLines = new SalesOrderLine[] {new SalesOrderLine {
                    LineNumber = 1,
                    LineTax = 10,
                    LineTotal = 100,
                    OrderQuantity =1,
                    Product = new Product { ProductCode = "P1"},
                    UnitPrice = 100
                 }
               }
            };
            var salesOrderResult = new SalesOrderResult
            {
                ErrorDescription = null,
                Pagination = new Pagination { NumberOfItems = 10, PageNumber = 1, NumberOfPages = 1, PageSize = 10 },
                SalesOrders = new System.Collections.Generic.List<SalesOrder> { order }
            };

            mockUnleashedClient.Setup(x => x.GetOrders("Small Beer Ltd", 1, int.MaxValue, "")).Returns(Task.FromResult(salesOrderResult));
            var response = await controller.GetLastOrder("Small Beer Ltd");
            var result = (OkObjectResult)response.Result;
            var content = (OrderDetailsResponseDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");

            Assert.AreEqual(order.CustomerRef, content.Reference);
            Assert.AreEqual(order.Comments, content.Comments);
            Assert.AreEqual(order.CompletedDate, content.CompletedDate);
            Assert.AreEqual(order.CreatedBy, content.OrderBy);
            Assert.AreEqual(order.DeliveryCountry, content.DeliveryAddress.Country);
            Assert.AreEqual(order.DeliveryCity, content.DeliveryAddress.City);
            Assert.AreEqual(order.DeliveryInstruction, content.DeliveryAddress.DeliveryNote);
            Assert.AreEqual(order.DeliveryName, content.DeliveryAddress.AddressName);
            Assert.AreEqual(order.DeliveryStreetAddress, content.DeliveryAddress.Line1);
            Assert.AreEqual(order.DeliveryStreetAddress2, content.DeliveryAddress.Line2);
            Assert.AreEqual(order.DeliverySuburb, content.DeliveryAddress.Suburb);
            Assert.AreEqual(order.DeliveryRegion, content.DeliveryAddress.Region);
            Assert.AreEqual(order.DeliveryPostCode, content.DeliveryAddress.Postcode);
            Assert.AreEqual(order.LastModifiedOn, content.LastModifiedDate);
            Assert.AreEqual(order.OrderDate, content.OrderDate);
            Assert.AreEqual(order.OrderNumber, content.OrderNumber);
            Assert.AreEqual(order.OrderStatus, content.Status);
            Assert.AreEqual(order.RequiredDate, content.DeliveryDate);
            Assert.AreEqual(order.SourceId, content.OrderSource);
            Assert.AreEqual(order.Currency.CurrencyCode, content.SubTotal.Currency);
            Assert.AreEqual(order.SubTotal, content.SubTotal.Value);
            Assert.AreEqual(0, content.DiscountsAndSurchargesTotal.Value);
            Assert.AreEqual(order.Currency.CurrencyCode, content.Total.Currency);
            Assert.AreEqual(order.Total, content.Total.Value);
            Assert.AreEqual(order.SalesOrderLines[0].LineTax, content.ProductItems[0].TaxTotal.Value);
            Assert.AreEqual(order.TaxRate, content.ProductItems[0].TaxRate);
            Assert.AreEqual(0, content.ProductItems[0].DiscountsAndSurchargesTotal.Value);

            Assert.AreEqual(order.SalesOrderLines[0].Product.ProductDescription, content.ProductItems[0].Name);
            Assert.AreEqual(order.SalesOrderLines[0].LineTotal / order.SalesOrderLines[0].OrderQuantity, content.ProductItems[0].LandedUnitCost.Value);
            Assert.AreEqual(order.SalesOrderLines[0].Product.ProductCode, content.ProductItems[0].ProductId);
            Assert.AreEqual(order.SalesOrderLines[0].Product.ProductDescription, content.ProductItems[0].Name);
            Assert.AreEqual(order.SalesOrderLines[0].OrderQuantity, content.ProductItems[0].Quantity);
            Assert.AreEqual(order.SalesOrderLines[0].LineTotal, content.ProductItems[0].SubTotal.Value);
            Assert.AreEqual(order.SalesOrderLines[0].LineTax, content.ProductItems[0].TaxTotal.Value);
            Assert.AreEqual(order.SalesOrderLines[0].UnitPrice, content.ProductItems[0].Price.Value);
        }

        [TestMethod]
        public async Task GetLastOrder_Failure()
        {
            var customerId = "abcq223";
            mockUnleashedClient.Setup(x => x.GetOrders(customerId, 1, 10, "")).Throws(new Exception());

            var response = await controller.GetLastOrder(customerId);

            var result = (ObjectResult)response.Result;

            // Validate status
            Assert.AreEqual(400, result.StatusCode, "Response status code should be error");

            // Validate error message
            Assert.IsNotNull(result.Value, "Response error message not found");
        }

        [TestMethod]
        public async Task GetLastOrder_None_Success()
        {
            var salesOrderResult = new SalesOrderResult
            {
                ErrorDescription = null,
                Pagination = new Pagination { NumberOfItems = 0, PageNumber = 1, NumberOfPages = 1, PageSize = 10 },
                SalesOrders = new System.Collections.Generic.List<SalesOrder> { }
            };
            mockUnleashedClient.Setup(x => x.GetOrders("Small Beer Ltd", 1, int.MaxValue, "")).Returns(Task.FromResult(salesOrderResult));
         
            var response = await controller.GetLastOrder("Small Beer Ltd");
            var result = (OkObjectResult)response.Result;
            var content = (OrderDetailsResponseDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");
            Assert.IsNull(content);
        }

        [TestMethod]
        public async Task GetNextUndeliveredOrder_Success()
        {
            var orders = new List<SalesOrderSummary>
            {
                new SalesOrderSummary { OrderNumber = "1", RequiredDate = DateTime.UtcNow.AddDays(1) },
                new SalesOrderSummary { OrderNumber = "1", RequiredDate = DateTime.UtcNow.AddDays(-1) }
            };

            var orderResult = new SalesOrderSummaryResult
            {
                ErrorDescription = null,
                Pagination = new Pagination { NumberOfItems = orders.Count, PageNumber = 1, NumberOfPages = 1, PageSize = orders.Count },
                SalesOrders = orders
            };

            mockUnleashedClient.Setup(x => x.GetOrderSummaries("Small Beer Ltd", 1, int.MaxValue, "Placed,Completed")).Returns(Task.FromResult(orderResult));
            var response = await controller.GetNextUndeliveredOrder("Small Beer Ltd");
            var result = (OkObjectResult)response.Result;
            var content = (List<OrderSummaryResponseDTO>)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");


            var order = orders.Where(x => x.RequiredDate.Date >= DateTime.UtcNow.Date).OrderBy(x => x.RequiredDate).First();
            Assert.AreEqual(1, content.Count);
            Assert.AreEqual(orders[0].OrderNumber, content[0].OrderNumber);
            Assert.AreEqual(orders[0].RequiredDate, content[0].DeliveryDate);
        }

        [TestMethod]
        public async Task GetNextUndeliveredOrder_Success_Today()
        {
            var orders = new List<SalesOrderSummary>
            {
                new SalesOrderSummary { OrderNumber = "1", RequiredDate = DateTime.UtcNow.AddDays(1) },
                new SalesOrderSummary { OrderNumber = "1", RequiredDate = DateTime.UtcNow },
                new SalesOrderSummary { OrderNumber = "1", RequiredDate = DateTime.UtcNow.AddDays(-1) }
            };

            var orderResult = new SalesOrderSummaryResult
            {
                ErrorDescription = null,
                Pagination = new Pagination { NumberOfItems = orders.Count, PageNumber = 1, NumberOfPages = 1, PageSize = orders.Count },
                SalesOrders = orders
            };

            mockUnleashedClient.Setup(x => x.GetOrderSummaries("Small Beer Ltd", 1, int.MaxValue, "Placed,Completed")).Returns(Task.FromResult(orderResult));
            var response = await controller.GetNextUndeliveredOrder("Small Beer Ltd");
            var result = (OkObjectResult)response.Result;
            var content = (List<OrderSummaryResponseDTO>)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");

            var order = orders.Where(x => x.RequiredDate.Date >= DateTime.UtcNow.Date).OrderBy(x => x.RequiredDate).First();
            Assert.AreEqual(1, content.Count);
            Assert.AreEqual(orders[1].OrderNumber, content[0].OrderNumber);
            Assert.AreEqual(orders[1].RequiredDate, content[0].DeliveryDate);
        }

        [TestMethod]
        public async Task GetNextUndeliveredOrder_Success_NoCurrentDeliveries()
        {
            var orders = new List<SalesOrderSummary>
            {
                new SalesOrderSummary { OrderNumber = "1", RequiredDate = DateTime.UtcNow.AddDays(-1) }
            };

            var orderResult = new SalesOrderSummaryResult
            {
                ErrorDescription = null,
                Pagination = new Pagination { NumberOfItems = orders.Count, PageNumber = 1, NumberOfPages = 1, PageSize = orders.Count },
                SalesOrders = orders
            };

            mockUnleashedClient.Setup(x => x.GetOrderSummaries("Small Beer Ltd", 1, int.MaxValue, "Placed,Completed")).Returns(Task.FromResult(orderResult));
            var response = await controller.GetNextUndeliveredOrder("Small Beer Ltd");
            var result = (OkObjectResult)response.Result;
            var content = (List<OrderSummaryResponseDTO>)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");
            Assert.AreEqual(0, content.Count, "Response content has orders");
        }

        [TestMethod]
        public async Task GetNextUndeliveredOrder_Success_None()
        {
            var orders = new List<SalesOrderSummary>();
            var orderResult = new SalesOrderSummaryResult
            {
                ErrorDescription = null,
                Pagination = new Pagination { NumberOfItems = orders.Count, PageNumber = 1, NumberOfPages = 1, PageSize = orders.Count },
                SalesOrders = orders
            };

            mockUnleashedClient.Setup(x => x.GetOrderSummaries("Small Beer Ltd", 1, int.MaxValue, "Placed,Completed")).Returns(Task.FromResult(orderResult));
            var response = await controller.GetNextUndeliveredOrder("Small Beer Ltd");
            var result = (OkObjectResult)response.Result;
            var content = (List<OrderSummaryResponseDTO>)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");
            Assert.AreEqual(0, content.Count, "Response content has orders");
        }

        [TestMethod]
        public async Task GetNextUndeliveredOrder_Failure()
        {
            var customerId = "abcq223";
            mockUnleashedClient.Setup(x => x.GetOrderSummaries(customerId, 1, 10, "Placed,Completed")).Throws(new Exception());
            var response = await controller.GetNextUndeliveredOrder(customerId);
            var result = (ObjectResult)response.Result;

            // Validate status
            Assert.AreEqual(400, result.StatusCode, "Response status code should be error");

            // Validate error message
            Assert.IsNotNull(result.Value, "Response error message not found");
        }


        [TestMethod]
        public async Task GetDeliveryInformation_Success()
        {
           
            var info = new DeliveryInformation
            {
               BodyText1 = "body text 1",
                BodyText2 = "body text 2",
                BodyText3 = "body text 3",
                Link = "https:www.google.co.uk",
                LinkText = "Google link",
                RowArray1 = new DeliveryInformationKeyValuePair[] { new DeliveryInformationKeyValuePair { Label = "Monday", Value = "London" }, new DeliveryInformationKeyValuePair { Label = "Tues", Value = "Essex" } },
                RowArray2 = new DeliveryInformationKeyValuePair[] { new DeliveryInformationKeyValuePair { Label = "Wed", Value = "Hastings" } }
            };

            mockContentfulService.Setup(x => x.GetDeliveryInformation()).Returns(Task.FromResult(info));
            var response = await controller.GetDeliveryInformation();
            var result = (OkObjectResult)response.Result;
            var content = (DeliveryInformationDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");
            Assert.AreEqual(info.BodyText1, content.BodyText1);
            Assert.AreEqual(info.BodyText2, content.BodyText2);
            Assert.AreEqual(info.BodyText3, content.BodyText3);
            Assert.AreEqual(info.Link, content.Link);
            Assert.AreEqual(info.LinkText, content.LinkText);
            Assert.AreEqual(info.RowArray1[0].Label, content.RowArray1[0].Key);
            Assert.AreEqual(info.RowArray1[1].Label, content.RowArray1[1].Key);
            Assert.AreEqual(info.RowArray2[0].Label, content.RowArray2[0].Key);

            Assert.AreEqual(info.RowArray1[0].Value, content.RowArray1[0].Value);
            Assert.AreEqual(info.RowArray1[1].Value, content.RowArray1[1].Value);
            Assert.AreEqual(info.RowArray2[0].Value, content.RowArray2[0].Value);

            Assert.AreEqual(2, content.RowArray1.Length);
            Assert.AreEqual(1, content.RowArray2.Length);
        }

        [TestMethod]
        public async Task GetDeliveryInformation_Success_EmptyValues()
        {

            var info = new DeliveryInformation
            {
                BodyText1 = "body text 1",
                BodyText2 = null,
                BodyText3 = null,
                Link = null,
                LinkText = null,
                RowArray1 =null ,
                RowArray2 =null
            };

            mockContentfulService.Setup(x => x.GetDeliveryInformation()).Returns(Task.FromResult(info));
            var response = await controller.GetDeliveryInformation();
            var result = (OkObjectResult)response.Result;
            var content = (DeliveryInformationDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");
            Assert.AreEqual(info.BodyText1, content.BodyText1);
            Assert.AreEqual("", content.BodyText2);
            Assert.AreEqual("", content.BodyText3);
            Assert.AreEqual("", content.Link);
            Assert.AreEqual("", content.LinkText);

            Assert.AreEqual(0, content.RowArray1.Length);
            Assert.AreEqual(0, content.RowArray2.Length);
        }

        [TestMethod]
        public async Task GetDeliveryInformation_Success_Null()
        {

            DeliveryInformation info = null;

            mockContentfulService.Setup(x => x.GetDeliveryInformation()).Returns(Task.FromResult(info));
            var response = await controller.GetDeliveryInformation();
            var result = (OkObjectResult)response.Result;
            var content = (DeliveryInformationDTO)result.Value;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Status code should be 200 ok");
            Assert.IsNull(content);
            
        }

        [TestMethod]
        public async Task GetDeliveryInformation_Failure()
        {
            mockContentfulService.Setup(x => x.GetDeliveryInformation()).Throws(new Exception());
            var response = await controller.GetDeliveryInformation();
            var result = (ObjectResult)response.Result;

            // Validate status
            Assert.AreEqual(400, result.StatusCode, "Response status code should be error");

            // Validate error message
            Assert.IsNotNull(result.Value, "Response error message not found");
        }
    }

}
