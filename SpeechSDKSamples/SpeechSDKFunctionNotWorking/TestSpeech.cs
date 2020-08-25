using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SpeechSDKCommon;
using System.Threading.Tasks;

namespace SpeechSDKFunctionNotWorking
{
    public static class TestSpeech
    {
        [FunctionName("TestSpeech")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var speech = await SpeechWrapper.RecognizeSpeechAsync();
                return new OkObjectResult(speech);
            }
            catch (System.Exception ex)
            {
                log.LogError(ex, "Exception while attempting to recognize speech");
                return new OkObjectResult(ex);
            }

        }
    }
}
