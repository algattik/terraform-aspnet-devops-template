using NBomber.Contracts;
using NBomber.CSharp;
using RestSharp;
using System;
using System.Net.Http;
using System.Runtime.Versioning;
using Xunit;
using Xunit.Abstractions;

namespace Contoso.LoadTests
{
    public class LoadTest
    {

        static HttpClient client = new HttpClient();
        static string _stepName = "CheckIfOK";
        private readonly Uri _path;
        private readonly int _concurrency;
        private readonly int _duration;
        private readonly int _warmuUp;
        private readonly ITestOutputHelper _output;

        public LoadTest(ITestOutputHelper output)
        {
            static Uri getUriFromEnvironment(string name)
            {
                var env = Environment.GetEnvironmentVariable(name);
                if (string.IsNullOrWhiteSpace(env))
                {
                    throw new NotSupportedException($"Missing environment variable {name}");
                }
                return new Uri(env);
            };

            _path = getUriFromEnvironment("SERVICE_URL");
            _concurrency = 10;
            _duration = 10;
            _warmuUp = 5;
        }

        ///<summary>
        /// Executes a basic load test using NBomber
        ///</summary>
        [Fact]
        public void RunLoadTest()
        {
            // Load test settings
            var warmUpPhase = TimeSpan.FromSeconds(_warmuUp); // execution time of warm-up before start bombing
            var testDuration = TimeSpan.FromSeconds(_duration); // execution time of Scenario 
            var concurrentCopies = _concurrency; // specify how many copies of current Scenario to run in parallel

            // Assertions parameters
            var acceptedFailNumber = 0;
            var minRPS = 5;

            // Execution step for the load scenario
            var step = Step.Create(_stepName, async context =>
            {
                var response = await client.GetAsync(_path + "/sample/sumNumbersUpTo?value=100");
                var success = response.IsSuccessStatusCode;
                return success ? Response.Ok() : Response.Fail();
            });

            // List of assertions to be respected by the scenario for each step
            var successAssertion = Assertion.ForStep(stepName: _stepName,
                assertion: statistics => statistics.FailCount == acceptedFailNumber);
            var rpsAssertion = Assertion.ForStep(stepName: _stepName,
                assertion: statistics => statistics.RPS >= minRPS);
        }
    }
}
