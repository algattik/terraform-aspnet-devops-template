// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso.UnitTests
{
    using System.Threading.Tasks;
    using Contoso;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class SampleServiceTests
    {
        [Fact]
        public async void AddTwoNumbers()
        {
            var controller = new Mock<ISumComputationAPI>();

            controller.Setup(foo => foo.SumNumbersUpTo(2)).Returns(Task.FromResult(3));

            var service = new SampleService(
                controller.Object,
                new Mock<ILogger<SampleService>>().Object,
                new Mock<MetricsService>(null).Object,
                new Mock<ICosmosDBService>().Object);
            var returnedString = await service.SumNumbersUpToAsync(3);
            Assert.Equal(6, returnedString);
        }
    }
}
