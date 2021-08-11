
using Sample.Order.BE.Data.Helper;
using Sample.Order.BE.Data.HttpClients.Interfaces;
using Sample.Order.BE.Data.Models.Contentful;
using Contentful.Core;
using Contentful.Core.Search;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Sample.Order.BE.Data.HttpClients
{
    public class ContentfulService : IContentfulService
    {
        private readonly IContentfulClient contentfulClient;
        private readonly ILogger<ContentfulService> logger;

        public ContentfulService(IContentfulClient contentfulClient, ILogger<ContentfulService> logger)
        {
            this.contentfulClient = contentfulClient;
            this.logger = logger;
        }

        public async Task<DeliveryInformation> GetDeliveryInformation()
        {
            var builder = QueryBuilder<DeliveryInformation>.New.ContentTypeIs(ContentfulContentTypes.DeliveryInformation).OrderBy("sys.createdAt");
            var info = (await contentfulClient.GetEntries(builder)).FirstOrDefault();

            return info;
        }

    }
}
