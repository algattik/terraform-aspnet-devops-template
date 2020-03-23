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
        public async void AddTwoNumbers()
        {
            var controller = Substitute.For<ISumComputationAPI>();
            controller.SumNumbersUpTo(2).Returns(3);
            var service = new SampleService(
                controller,
                Substitute.For<ILogger<SampleService>>(),
                Substitute.For<MetricsService>());
            var returnedString = await service.SumNumbersUpToAsync(3);
            Assert.Equal(6, returnedString);
        }
    }
}
