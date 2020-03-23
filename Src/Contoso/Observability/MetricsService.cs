// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using Microsoft.ApplicationInsights;
    using Prometheus;

    /// <summary>
    /// Prometheus Histograms to collect query performance data.
    /// </summary>
    public class MetricsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsService"/> class.
        /// Constructor for mocking.
        /// </summary>
        public MetricsService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsService"/> class.
        /// </summary>
        /// <param name="telemetryClient">The ApplicationInsights telemetry client (null if AppInsights is off).</param>
        public MetricsService(TelemetryClient? telemetryClient)
        {
            var name = "sum_computation_api_call_duration_s";
            var help = "Duration in seconds of calls to the SumComputationAPI.";
            this.SumComputationAPICallDuration = new Histogram(
                Prometheus.Metrics.CreateHistogram(name, help, new HistogramConfiguration
                {
                    Buckets = Prometheus.Histogram.ExponentialBuckets(start: 0.001, factor: 2, count: 15),
                }),
                name,
                help,
                telemetryClient?.GetMetric(name));
        }

        /// <summary>
        /// Gets the metric for the duration in seconds of calls to the SumComputationAPI.
        /// </summary>
        public Histogram? SumComputationAPICallDuration { get; }
    }
}
