namespace Contoso
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("[controller]/[action]")]
    public class SampleController : ControllerBase
    {
        public SampleController(SampleService sampleService, ILogger<SampleController> logger)
        {
            Logger = logger;
            SampleService = sampleService;
        }

        private SampleService SampleService { get; set; }

        private ILogger Logger { get; set; }

        [HttpGet]
        public IActionResult Process()
        {
            var response = SampleService.DoSomething("Hello");

            return Ok(response);
        }
    }
}
