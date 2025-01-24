using System;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GidIndustrial.Gideon.WebApi {
    /// <summary>
    /// Container for the program entry-point.
    /// </summary>
    public class Program {
        #region Methods

        /// <summary>
        /// Follows the builder pattern to create a web application host.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns><see cref="IWebHost" /> object that hosts the app and begins listening for HTTP requests.</returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
 
                // .CaptureStartupErrors(true) // uncomment these lines if there is an error in startup
                // .UseSetting("detailedErrors", "true")
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();

        /// <summary>
        /// Invokes <see cref="BuildWebHost(string[])" />.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args) {
            var host = BuildWebHost(args);

            // var host = new WebHostBuilder()
            // .UseKestrel()
            // .UseUrls("http://*:58263")
            // .UseStartup<Startup>()
            // .Build();

            host.Run();
        }

        #endregion Methods
    }
}