// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso.UnitTests
{
    using Contoso;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Xunit;

    public class SampleServiceTests
    {
        [Fact]
        public void AddTwoNumbers()
        {
            var service = new SampleService(
                Substitute.For<ILogger<SampleService>>());
            var returnedString = service.AddTwoNumbers(2, 3);
            Assert.Equal(5, returnedString);
        }
    }
}
