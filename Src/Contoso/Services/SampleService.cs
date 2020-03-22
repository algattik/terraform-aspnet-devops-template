// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using Microsoft.Extensions.Logging;

    public class SampleService
    {
        public SampleService(ILogger<SampleService> logger)
        {
            this.Logger = logger;
        }

        private ILogger Logger { get; set; }

        public static int AddTwoNumbers(int x, int y)
        {
            return x + y;
        }

        public string DoSomething(string indexName)
        {
            try
            {
                return indexName;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error while executing DoSomething.");
                throw;
            }
        }
    }
}
