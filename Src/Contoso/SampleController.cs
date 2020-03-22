namespace Contoso
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

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
        public IActionResult Process(string name)
        {
            var response = SampleService.DoSomething(name);

            return Ok(response);
        }
    }
}
