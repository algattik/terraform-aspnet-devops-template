// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Sample controller.
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class SampleController : ControllerBase, ISumComputationAPI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleController"/> class.
        /// </summary>
        /// <param name="sampleService">Service to add numbers.</param>
        /// <param name="logger">Logger.</param>
        public SampleController(ISampleService sampleService, ILogger<SampleController> logger)
        {
            this.Logger = logger;
            this.SampleService = sampleService;
        }

        private ISampleService SampleService { get; set; }

        private ILogger Logger { get; set; }

        /// <summary>
        /// Add two numbers and return their sum.
        /// </summary>
        /// <param name="value">Number to add values up to.</param>
        /// <returns>Sum of integer numbers from 0 to value.</returns>
        [HttpGet]
        public Task<int> SumNumbersUpTo(int value)
        {
            var response = this.SampleService.SumNumbersUpToAsync(value);

            return response;
        }
    }
}
