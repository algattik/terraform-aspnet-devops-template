// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Interface to Azure management operations.
    /// </summary>
    public class AzureService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureService"/> class.
        /// </summary>
        /// <param name="configuration">Configuration for service.</param>
        public AzureService(IConfiguration configuration)
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

            this.Azure = Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .Authenticate(credentials)
                .WithDefaultSubscription();
        }

        /// <summary>
        ///  Gets Azure interface.
        /// </summary>
        public IAzure Azure { get; }
    }
}