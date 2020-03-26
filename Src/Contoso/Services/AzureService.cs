// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Interface to Azure management operations.
    /// </summary>
    public class AzureService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureService"/> class.
        /// </summary>
        /// <param name="configuration">Configuration for service.</param>
        /// <param name="logger">Logger.</param>
        public AzureService(IConfiguration configuration, ILogger<AzureService> logger)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var clientId = configuration["aadClientId"];
            var clientSecret = configuration["aadClientSecret"];
            var tenantId = configuration["aadTenantId"];

            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(
                clientId,
                clientSecret,
                tenantId,
                AzureEnvironment.AzureGlobalCloud);

            try
            {
                this.Azure = Microsoft.Azure.Management.Fluent.Azure
                    .Configure()
                    .Authenticate(credentials)
                    .WithDefaultSubscription();
            }
            catch (Exception e)
            {
                logger.LogCritical(
                    e,
                    "Couldn't authenticate to Azure with client {clientId} in tenant {tenantId}",
                    clientId,
                    tenantId);
                throw;
            }
        }

        /// <summary>
        ///  Gets Azure interface.
        /// </summary>
        public IAzure Azure { get; }
    }
}