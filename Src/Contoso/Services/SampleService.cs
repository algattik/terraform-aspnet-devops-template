// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Sample service.
    /// </summary>
    public class SampleService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleService"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public SampleService(ILogger<SampleService> logger)
        {
            this.Logger = logger;
        }

        private ILogger Logger { get; set; }

        /// <summary>
        /// Add two numbers and return their sum.
        /// </summary>
        /// <param name="x">First number to add.</param>
        /// <param name="y">Second number to add.</param>
        /// <returns>Sum of x and y.</returns>
        public int AddTwoNumbers(int x, int y)
        {
            this.Logger.LogInformation("Adding numbers");
            return x + y;
        }
    }
}
