using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using DinkToPdf.Contracts;
using DinkToPdf;
using WebApi.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using GidIndustrial.Gideon.WebApi.Libraries;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights.Extensibility;
using GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet;
using GidIndustrial.Gideon.WebApi.Controllers;
using System;
using System.Linq;
using LeadAutomation.Firefly.Exchange.Helpers;

namespace GidIndustrial.Gideon.WebApi {
    /// <summary>
    /// Configures services and the app's request pipeline.
    /// </summary>
    public class Startup {

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">Application configuration properties.</param>
        public Startup(IHostingEnvironment env, IConfiguration config) {
            Environment = env;
            Configuration = config;
        }


        /// <summary>
        /// /// Gets application configuration properties.
        /// </summary>
        IHostingEnvironment Environment { get; set; }
        IConfiguration Configuration { get; set; }


        /// <summary>
        /// Defines the middleware for the request pipeline.
        /// </summary>
        /// <param name="app">Application request pipeline builder.</param>
        /// <param name="env">Information about the web hosting environment of the application.</param>
        /// <remarks>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</remarks>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            // if (env.IsDevelopment()) {
            // }
            app.UseCors("CorsPolicy");
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
            app.UseSession();
            // https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?tabs=aspnetcore2x
            app.UseResponseCompression();

            app.UseStaticFiles();

            app.UseAuthentication();

            // app.UseMiddleware<LogRequestsMiddleware>();

            // app.UseIdentity();
            //Defaults to {Controller="Home"}/{Action="Index"}/{id?}
            app.UseMvcWithDefaultRoute();

            loggerFactory.AddAzureWebAppDiagnostics(new AzureAppServicesDiagnosticsSettings {
                OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss zzz} [{Level}] {RequestId}-{SourceContext}: {Message}{NewLine}{Exception}"
            });

        }

        /// <summary>
        /// Defines the services used by the app.
        /// </summary>
        /// <param name="services">Contract for service descriptors.</param>
        /// <remarks>This method gets called by the runtime. Use this method to add services to the container.</remarks>
        public void ConfigureServices(IServiceCollection services) {
            // https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?tabs=aspnetcore2x
            services.AddResponseCompression(options => {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            }).Configure<GzipCompressionProviderOptions>(options => {
                options.Level = CompressionLevel.Fastest;
            });
            services.AddSession();
            services.AddSingleton<ITelemetryInitializer, RequestBodyInitializer>();


#if DEBUG
            //In development need to load the 64 bit libwkhtmltox binary for pdf generation
            if (System.Environment.Is64BitOperatingSystem) {
                // Add converter to DI
                CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
                context.LoadUnmanagedLibrary(Environment.ContentRootPath + @"\Libraries\libwkhtmltox64bit\libwkhtmltox.dll");
            }
#endif
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<ViewRender, ViewRender>();


            services.AddAuthentication(options => {
                // the scheme name has to match the value we're going to use in AuthenticationBuilder.AddScheme(...)
                options.DefaultAuthenticateScheme = "Custom Scheme";
                options.DefaultChallengeScheme = "Custom Scheme";
            })
            .AddCustomAuth(o => { });
            AuthenticationController.SetTokenLifetimeMinutes(60);

            //Very important!
            //This makes sure that only logged in users can access data by default
            //You can use [AllowAnonymous] to allow anonymous access later on
            services.AddMvc(config => {
                var authPolicy = new AuthorizationPolicyBuilder().RequireAssertion(authContext => {
                    if (authContext.Resource is Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext) {
                        //Get the MVC authorization context
                        var authContextResource = (Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)authContext.Resource;
                        var apiKey = Configuration.GetValue<string>("API-Key");
                        if (apiKey.Length > 0 && authContextResource.HttpContext.Request.Headers["API-Key"] == apiKey) {
                            return true;
                        }
                    }
                    return authContext.User.Identity.IsAuthenticated;
                }).Build();
                config.Filters.Add(new AuthorizeFilter(authPolicy));
            })
            .AddControllersAsServices()
            .AddJsonOptions(options => {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });


            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(
                            "https://gideon.gidindustrial.com",
                            "https://localhost:7024",
                            "http://localhost:5142",
                            "http://localhost:59908",
                            "http://localhost:59945",
                            "http://localhost:58263"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();  // Ensure credentials are allowed
                });
            });

            services.Configure<MvcOptions>(options => {
                options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
            });

            //put configuration variables into classes that need them
            MicrosoftGraphApiConnector.configuration = Configuration;
            EmailGenerator.configuration = Configuration;
            // services.AddScoped<EmailGenerator>();
            // QuickBooksApiInfo.configuration = Configuration;
            Encryption.KeyString = Configuration.GetValue<string>("EKey");

            services.AddSingleton<QuickBooksConnector>();

            services.Configure<QuickBooksOptions>(Configuration.GetSection("QuickBooks"));

            if (Environment.IsDevelopment()) {
                services.AddDbContext<AppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("GideonLocalDB")));
                //services.AddDbContext<AppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ProductionDB")));

                AuthorizeNetApiRequestor.SetLiveMode(); // Temp set live mode
            } else {
                services.AddDbContext<AppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ProductionDB")));
                AuthorizeNetApiRequestor.SetLiveMode();
            }

            //Init stuff that requires database or other services
            using (var scope = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope()) {
                var provider = scope.ServiceProvider;
                using (var dbContext = provider.GetRequiredService<AppDBContext>()) {
                    // Run migrations
                    try{
                        //dbContext.Database.Migrate(); // This line should only be run once. After first execution, user should comment it out to avoid errors in DB migration.
                    }catch(Exception ex){
                        Console.WriteLine("ERROR MIGRATING DB!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }

                    SeedDB.Seed(dbContext); // Uncommented this line first to run seed DB
                    // ExchangeRates.Initialize();
                    // var salesOrders = dbContext.SalesOrders.ToList();
                    // var i = 0;
                    // foreach (var salesOrder in salesOrders)
                    // {
                    //     var profitTask = salesOrder.GetNetProfit(dbContext);
                    //     profitTask.Wait();
                    //     Console.WriteLine("Done " + i++);
                    // }

                    // Sync users with AD - Get any that haven't been copied over yet
                    User.GetAllUsers(dbContext);

                    //set up user groups - sync them from AD
                    UserGroup.SetupUserGroups(dbContext);

                    var quickBooksConnector = provider.GetRequiredService<QuickBooksConnector>();
                    quickBooksConnector.FetchQuickBooksApiInfo(dbContext);

                    // Quote.SetTotalForAllQuotes(dbContext).Wait();
                    // SalesOrder.SetBalanceForAllSalesOrders(dbContext, services).Wait();


                    // GidLocationOption.ConvertSubLocationOptions(dbContext);

                    // quickBooksConnector.SyncQB(dbContext).Wait();
                    // throw new Exception("All done");

                    if (!Environment.IsDevelopment()) {
                        //sync quickbooks terms
                        PaymentTerm.GetAllFromQuickBooks(quickBooksConnector, dbContext).Wait();

                        //sync sales order payment methods
                        SalesOrderPaymentMethod.GetAllFromQuickBooks(quickBooksConnector, dbContext).Wait();
                    }
                }
            }
        }
    }
}

//#2 