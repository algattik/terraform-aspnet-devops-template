namespace Contoso
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Prometheus;
    using Serilog;

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
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureTelemetryServices(services);

            services.AddControllers();

            services.AddTransient<SampleService>(
                s => new SampleService(
                    s.GetRequiredService<ILogger<SampleService>>()));

            // use this http client factory to issue requests to the metadata elastic instance
            services.AddHttpClient("ForwardingClient", (svcProvider, httpClient) =>
            {
                var computeServiceURL = Configuration["computeServiceURL"];
                httpClient.BaseAddress = new Uri(computeServiceURL);
            }).AddHeaderPropagation();

            services.AddHeaderPropagation(options =>
            {
                options.Headers.Add("X-TraceId");
            });

            // Add a health/liveness service
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // detailed request logging
            app.UseSerilogRequestLogging();

            app.UseRouting();

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
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
        /// <summary>
        /// Configures all telemetry services - internal (like Prometheus endpoint) and external (Application Insights).
        /// </summary>
        private void ConfigureTelemetryServices(IServiceCollection services)
        {
            // using GetService since TelemetryClient won't exist if AppInsights is turned off.
            services.AddSingleton(
                s => Metrics.Create(s.GetService<TelemetryClient>()));

            // complete the config for AppInsights.
            ConfigureApplicationInsights(services);
        }

        /// <summary>
        /// Configures the ApplicationInsights telemetry.
        /// </summary>
        private void ConfigureApplicationInsights(IServiceCollection services)
        {
            // Only if explicitly declared we are collecting telemetry
            var hasCollectBool =
                bool.TryParse(
                    Configuration["collectTelemetry"],
                    out bool isCollect);

            if (!hasCollectBool || !isCollect)
            {
                return;
            }

            var adxUrl = Configuration["adxClusterUrl"];

            // verify we got a valid instrumentation key, if we didn't, we just skip AppInsights
            // we do not log this, as at this point we still don't have a logger
            var hasGuid = Guid.TryParse(Configuration["instrumentationKey"], out Guid instrumentationKey);
            if (hasGuid)
            {
                services.AddApplicationInsightsTelemetry(instrumentationKey.ToString());
                var telemetryIdentifier = "Contoso";

                services.AddSingleton<ITelemetryInitializer>(s =>
                    new TelemetryInitializer(s.GetRequiredService<IHttpContextAccessor>(), telemetryIdentifier, HealthCheckRoute));
            }
        }
    }
}
