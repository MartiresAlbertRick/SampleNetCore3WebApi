using AD.CAAPS.API.Controllers;
using AD.CAAPS.API.Models;
using AD.CAAPS.Entities;
using AD.CAAPS.Entities.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NLog.Web;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using AD.CAAPS.Services;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace AD.CAAPS.API
{
    public class Startup
    {
        // https://devblogs.microsoft.com/dotnet/configureawait-faq/


        internal static IConfiguration Configuration { get; private set; }
        internal static bool IsDevelopment { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container. The method cannot be "static"
#pragma warning disable CA1822 // Mark members as static
        public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // Mark members as static
        {
            // add singleton to allow injecting IConfiguration into Controllers/Services
            services.AddSingleton<IConfiguration>(Configuration);

            BaseController.ConfigureControllers(Configuration);
            BaseServices.ConfigureService(Configuration);

            // add compression
            services.Configure<GzipCompressionProviderOptions>(options => { options.Level = System.IO.Compression.CompressionLevel.Optimal; })
                    .Configure<BrotliCompressionProviderOptions>(options => { options.Level = System.IO.Compression.CompressionLevel.Optimal; })
                    .AddResponseCompression(options =>
                    {
                        options.EnableForHttps = true;
                        options.Providers.Add<GzipCompressionProvider>();
                        options.Providers.Add<BrotliCompressionProvider>();
                    });

            // add validators
            services.AddTransient<IValidator<Vendor>, VendorValidator>()
                    .AddTransient<IValidator<GoodsReceipt>, GoodsReceiptValidator>()
                    .AddTransient<IValidator<PurchaseOrder>, PurchaseOrderValidator>()
                    .AddTransient<IValidator<ImportConfirmation>, ImportConfirmationValidator>()
                    .AddTransient<IValidator<Payment>, PaymentValidator>()
                    .AddTransient<IValidator<ValidAdditionalCharges>, ValidAdditionalChargeValidator>()
                    .AddTransient<IValidator<Entity>, EntityValidator>()
                    .AddTransient<IValidator<UnitOfMeasure>, UnitOfMeasureValidator>()
                    .AddTransient<IValidator<RoutingCodes>, RoutingCodeValidator>()
                    .AddTransient<IValidator<Currency>, CurrencyValidator>()
                    .AddTransient<IValidator<NonPoVendor>, NonPoVendorValidator>()
                    .AddTransient<IValidator<ApDocument>, ApDocumentValidator>()
                    .AddTransient<IValidator<GLCodeLine>, GLCodedLineValidator>()
                    .AddTransient<IValidator<LineItem>, LineItemValidator>()
                    .AddTransient<IValidator<GLCodeDetails>, GLCodeValidator>()
                    .AddTransient<IValidator<TaxCodeDetails>, TaxCodeValidator>()
                    .AddTransient<IValidator<Product>, ProductValidator>()
                    .AddTransient<IValidator<ClosedPurchaseOrder>, ClosedPurchaseOrderValidator>()
                    .AddTransient<IValidator<PaymentTerms>, PaymentTermsValidator>();

            // add cors
            services.AddCors(options =>
            {
                /*
                  Removed: corsPolicyBuilder.AllowCredentials()
                  Reason: The CORS protocol does not allow specifying a wildcard (any) origin and credentials at the same time. 
                  Configure the CORS policy by listing individual origins if credentials needs to be supported
                */
                options.AddPolicy("CORS", corsPolicyBuilder =>
                      corsPolicyBuilder.AllowAnyOrigin()
                                       .AllowAnyMethod()
                                       .AllowAnyHeader()
                                       );
            });

            // odata query settings
            services.Configure<ODataQuerySettings>(options => { options.PageSize = Convert.ToInt32(Configuration.GetSection("AppSettings:QuerySettings:PageCount").Value); });

            // response caching
            services.Configure<CustomCacheSettings>(Configuration.GetSection("AppSettings:CacheSettings"))
                    .AddScoped<CustomEnableQueryAttribute>();

            // add swashbuckle
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CAAPS API",
                    Version = "v1",
                    Description = "Acumen Data CAAPS API",
                    Contact = new OpenApiContact
                    {
                        Name = Configuration.GetValue<string>("AppSettings:API:ContactName"),
                        Email = Configuration.GetValue<string>("AppSettings:API:ContactEmail")
                    },
                });
                c.CustomOperationIds(e => e.ActionDescriptor.RouteValues["Action"]);
                // c.OperationFilter<SwaggerPreventDuplicateConsumeFilter>();
            });

            services.AddHealthChecks();

/*
            services.AddSignalR(hubOptions =>
            {
                hubOptions.MaximumReceiveMessageSize = 32768;
            });
*/
            // Add odata
            services.AddOData();

            //ServiceProvider serviceProvider = services.BuildServiceProvider();
            // CustomCacheSettings cacheOptions = serviceProvider.GetRequiredService<IOptions<CustomCacheSettings>>().Value;
            services.AddResponseCaching();

            // CustomCacheSettings cacheOptions = serviceProvider.GetRequiredService<IOptions<CustomCacheSettings>>().Value;
            CustomCacheSettings cacheOptions = Configuration.Get<CustomCacheSettings>();

            // AddMvc replaced by AddControllers in latest .AspNetCore
            // services.AddMvc()
            var mvcBuilder = services.AddControllers(options =>
            {
                options.EnableEndpointRouting = false;

                if (cacheOptions != null && cacheOptions.DurationSeconds > 0)
                {
                    options.CacheProfiles.Add("DefaultCacheProfile",
                    new CacheProfile()
                    {
                        Duration = cacheOptions.DurationSeconds,
                        VaryByHeader = cacheOptions.VaryByHeader,
                        VaryByQueryKeys = cacheOptions.VaryByQueryKeys.ToArray<string>()
                    });
                }
                else
                {
                    options.CacheProfiles.Add("DefaultCacheProfile",
                    new CacheProfile()
                    {
                        NoStore = true
                    });
                }

                foreach (var formatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(it => !it.SupportedMediaTypes.Any()))
                {
                    System.Diagnostics.Trace.WriteLine($"Adding {formatter.BaseAddressFactory?.Method?.Name} output SupportedMediaTypes - \"application/prs.mock-odata\"");
                    formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.mock-odata"));
                }
                foreach (var formatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(it => !it.SupportedMediaTypes.Any()))
                {
                    System.Diagnostics.Trace.WriteLine($"Adding {formatter.BaseAddressFactory?.Method?.Name} input SupportedMediaTypes - \"application/prs.mock-odata\"");
                    formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.mock-odata"));
                }
            });

            // SetCompatibilityVersion is obsolete in latest .AspNetCore
            // .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            // .SetCompatibilityVersion(CompatibilityVersion.Latest)

            mvcBuilder.AddFluentValidation();

            // use NewtonsoftJson serializer? 
            mvcBuilder.AddNewtonsoftJson(options => {
                if (Configuration.GetValue<bool>("AppSettings:API:PrettifyJSON"))
                    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            // Use Text.Json serializer? 
            /*
            mvcBuilder.AddJsonOptions(options =>
            {
                if (Configuration.GetValue<bool>("AppSettings:API:PrettifyJSON"))
                {
                    options.JsonSerializerOptions.WriteIndented = true; //.Formatting = Newtonsoft.Json.Formatting.Indented;
                };
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // .ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            });
            */

            /*
            mvcBuilder.AddJsonOptions(options => { 
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; 
            });
            */

            // 
            // services.AddControllersWithViews(options => { });
            // services.AddRazorPages(options => { });

            // services.AddMvc((MvcOptions options) =>{ });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. The method cannot be "static"
#pragma warning disable CA1822 // Mark members as static
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore CA1822 // Mark members as static
        {
            
            if (env.IsDevelopment())
            {
                Startup.IsDevelopment = true;

                // https://stackoverflow.com/questions/58184170/no-usedatabaseerrorpage-extension-method-in-net-core-3-0
                // .Net Core 3.1 removes dependency: Microsoft.AspNetCore.App
                // .Net Core 3.1 added dependency: Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore

                app.UseDatabaseErrorPage();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                Startup.IsDevelopment = false;
                app.UseHsts();
                // app.UseDatabaseErrorPage();
                // app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CAAPS API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseCors("CORS");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                // endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions() {  });
                // endpoints.MapHub<ChatHub>("/echo");
                endpoints.MapControllers();
                // endpoints.MapDefaultControllerRoute();
                // endpoints.MapControllerRoute("default", "{controller=API}/{action=ECHO}/{id?}");

                /*
                MapControllers adds support for attribute - routed controllers.
                MapAreaControllerRoute adds a conventional route for controllers in an area.
                MapControllerRoute adds a conventional route for controllers.

                endpoints.MapControllers();
                endpoints.MapAreaControllerRoute(
                    "admin",
                    "admin",
                    "Admin/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    "default", "{controller=Home}/{action=Index}/{id?}");
                */    
            });

            app.UseResponseCaching();
            app.UseResponseCompression();

            int maxTopValue = Configuration.GetSection("AppSettings:QuerySettings:TopLimit").Get<int>();

            // odata
            app.UseMvc((IRouteBuilder routeBuilder) =>
            {
                // $expand
                // $select
                // $count
                // $orderby
                // $filter
                // $top
                // $skip
                routeBuilder.SkipToken().Expand().Select().Count().OrderBy().Filter().MaxTop(maxTopValue);
                routeBuilder.EnableDependencyInjection();
            });
        }
    }

    /* REMOVED/OBSOLETE FUNCTIONALITY
     * 
     * https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1296
    public class SwaggerPreventDuplicateConsumeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            operation.Consumes = operation.Consumes.Distinct().ToList();
        }
    }
    */
}