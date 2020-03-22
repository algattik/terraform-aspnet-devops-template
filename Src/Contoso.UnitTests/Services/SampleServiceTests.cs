using Contoso;
using Xunit;

namespace Tests
{
    public class SampleServiceTests
    {

        [Fact]
        public void AddTwoNumbers()
        {
            var returnedString = SampleService.AddTwoNumbers(2, 3);
            Assert.Equal(5, returnedString);
        }
    }
}
