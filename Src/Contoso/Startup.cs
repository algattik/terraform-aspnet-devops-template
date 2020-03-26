// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Prometheus;
    using Serilog;

    /// <summary>
    /// Startup ASP.NET Core class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string HealthCheckRoute = "/health";

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration for the app.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        private ServerTelemetryChannel? TelemetryChannel { get; set; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">Host environment.</param>
        /// <param name="lt">Application lifetime.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lt)
        {
            lt?.ApplicationStopping.Register(this.OnShutdown);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // detailed request logging
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseHeaderPropagation();

            // Expose HTTP Metrics to Prometheus:
            // Number of HTTP requests in progress.
            // Total number of received HTTP requests.
            // Duration of HTTP requests.
            app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                // Starts a Prometheus metrics exporter using endpoint routing.
                // Using The default URL: /metrics.
                endpoints.MapMetrics();

                endpoints.MapControllers();

                // Enable middleware to serve from health endpoint
                endpoints.MapHealthChecks(HealthCheckRoute);
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="services">Service collection to be configured.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            this.ConfigureTelemetryServices(services);

            services.AddSingleton(
                s => new AzureService(
                    this.Configuration,
                    s.GetService<ILogger<AzureService>>()));

            services.AddSingleton(
                s => new CosmosDBService(
                    this.Configuration,
                    s.GetService<AzureService>(),
                    s.GetService<ILogger<CosmosDBService>>()));

            services.AddControllers();

            services.AddTransient<ISampleService>(
                s => new SampleService(
                    s.GetRequiredService<ISumComputationAPI>(),
                    s.GetRequiredService<ILogger<SampleService>>(),
                    s.GetRequiredService<MetricsService>(),
                    s.GetRequiredService<CosmosDBService>()));

            services.AddHeaderPropagation(options =>
            {
                options.Headers.Add("X-TraceId");
            });

            // use this http client factory to issue requests to the compute service
            services.AddHttpClient("ComputeServiceClient", c =>
            {
                var computeServiceAddress = this.Configuration["computeServiceAddress"];
                c.BaseAddress = new Uri(computeServiceAddress);
            })
            .AddHeaderPropagation()
            .AddTypedClient(c => Refit.RestService.For<ISumComputationAPI>(c));

            // Add a health/liveness service
            services.AddHealthChecks();
        }

        private void OnShutdown()
        {
            if (this.TelemetryChannel == null)
            {
                return;
            }

            Log.Information("***** Flushing telemetry *****");

            this.TelemetryChannel?.Flush();

            // Wait while the data is flushed
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Configures all telemetry services - internal (like Prometheus endpoint) and external (Application Insights).
        /// </summary>
        private void ConfigureTelemetryServices(IServiceCollection services)
        {
            // using GetService since TelemetryClient won't exist if AppInsights is turned off.
            services.AddSingleton(
                s => new MetricsService(s.GetService<TelemetryClient>()));

            // complete the config for AppInsights.
            this.ConfigureApplicationInsights(services);
        }

        /// <summary>
        /// Configures the ApplicationInsights telemetry.
        /// </summary>
        private void ConfigureApplicationInsights(IServiceCollection services)
        {
            // verify we got a valid instrumentation key, if we didn't, we just skip AppInsights
            // we do not log this, as at this point we still don't have a logger
            var hasKey = Guid.TryParse(this.Configuration["instrumentationKey"], out Guid instrumentationKey);
            if (hasKey)
            {
                var telemetryAppId = this.Configuration["instrumentationAppId"] ?? "App";
                Log.Information("***** Configuring Application Insights telemetry with Id {identifier} *****", telemetryAppId);

                // This sets up ServerTelemetryChannel with StorageFolder set to a custom location.
                this.TelemetryChannel = new ServerTelemetryChannel() { StorageFolder = this.Configuration["appInsightsStorageFolder"] };
                services.AddSingleton(typeof(ITelemetryChannel), this.TelemetryChannel);

                services.AddApplicationInsightsTelemetry(instrumentationKey.ToString());

                services.AddSingleton<ITelemetryInitializer>(s =>
                    new TelemetryInitializer(s.GetRequiredService<IHttpContextAccessor>(), telemetryAppId, HealthCheckRoute));
            }
            else
            {
                Log.Information("***** Not configuring Application Insights telemetry *****");
            }
        }
    }
}
