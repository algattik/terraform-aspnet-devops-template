namespace Contoso
{
    using System;
    using Microsoft.Extensions.Logging;

    public class SampleService
    {
        public SampleService(ILogger<SampleService> logger)
        {
            Logger = logger;
        }

        private ILogger Logger { get; set; }

        public string DoSomething(string indexName)
        {
            try
            {
                return indexName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error while executing DoSoomething.");
                throw;
            }
        }

    }
}
