// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Sample controller.
    /// </summary>
    [Route("[controller]/[action]")]
    public class SampleController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleController"/> class.
        /// </summary>
        /// <param name="sampleService">Service to add numbers.</param>
        /// <param name="logger">Logger.</param>
        public SampleController(SampleService sampleService, ILogger<SampleController> logger)
        {
            this.Logger = logger;
            this.SampleService = sampleService;
        }

        private SampleService SampleService { get; set; }

        private ILogger Logger { get; set; }

        /// <summary>
        /// Action for adding numbers.
        /// </summary>
        /// <returns>A dummy response.</returns>
        [HttpGet]
        public IActionResult Process()
        {
            var response = this.SampleService.AddTwoNumbers(1, 2);

            return this.Ok(response);
        }
    }
}
