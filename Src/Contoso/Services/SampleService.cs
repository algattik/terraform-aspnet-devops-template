// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Sample service.
    /// </summary>
    public class SampleService : ISampleService
    {
        private readonly ISampleController client;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleService"/> class.
        /// </summary>
        /// <param name="client">Remote controller.</param>
        /// <param name="logger">Logger.</param>
        public SampleService(ISampleController client, ILogger<SampleService> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        /// <summary>
        /// Add two numbers and return their sum.
        /// </summary>
        /// <param name="value">Number to add values up to.</param>
        /// <returns>Sum of integer numbers from 0 to value.</returns>
        public async Task<int> SumNumbersUpToAsync(int value)
        {
            if (value <= 1)
            {
                return value;
            }

            var r1 = await this.client.SumNumbersUpTo(value - 1);
            var sum1 = r1;
            return value + sum1;
        }
    }
}
