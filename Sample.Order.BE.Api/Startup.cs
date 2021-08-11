using Sample.Order.BE.Business.Configs;
using Sample.Order.BE.Business.Services;
using Sample.Order.BE.Business.Services.Interfaces;
using Sample.Order.BE.Data.Config;
using Sample.Order.BE.Data.HttpClients;
using Sample.Order.BE.Data.HttpClients.Interfaces;
using AutoMapper;
using Contentful.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace Sample.Order.BE.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public static IConfiguration Configuration { get; private set; }

        public IWebHostEnvironment Environment { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                  .AddJsonOptions(options =>
                  {
                      options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                  });

            // Load settings
            services.AddOptions();
            services.Configure<MessagesConfig>(Configuration.GetSection(MessagesConfig.Messages));
            services.Configure<UnleashedConfig>(Configuration.GetSection(UnleashedConfig.Unleashed));

            // Configure mappers
            var mapperConfig = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new AutoMapperProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton<IMapper>(mapper);

            // Add Http Context Accessor
            services.AddHttpContextAccessor();

            // Add transient for Services.
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IContentfulService, ContentfulService>();

            services.AddHttpClient<IUnleashedClient, UnleashedClient>(client =>
            {
                client.BaseAddress = new Uri(Configuration["Unleashed:ApiBase"]);
                client.DefaultRequestHeaders.Add("api-auth-id", Configuration["Unleashed:ApiId"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddContentful(Configuration);

            // Add Swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample Order Service Api", Version = "v1", Description = "API to support the Sample Order operations." });

                // Set the comments path for the Swagger JSON and UI.
                if (Environment.IsDevelopment())
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, "swagger.xml");
                    c.IncludeXmlComments(xmlPath);
                }
                else
                {
                    c.IncludeXmlComments("swagger.xml");
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Sample Order Service Api");
                c.RoutePrefix = "swagger";
            });
        }
    }
}
