using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VideoProcessor
{
    public static class OrchestratorFunctions
    {
        [FunctionName(nameof(ProcessVideoOrchestrator))]
        public static async Task<object> ProcessVideoOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            string videoLocation = null;
            string withIntroLocation = null;
            string thumbnailLocation = null;
            string transcodedLocation = null;

            try
            {

                log = context.CreateReplaySafeLogger(log);
                videoLocation = context.GetInput<string>();

                //Retry
                log.LogInformation("about to call transcode video activity");
                transcodedLocation = await context.CallActivityWithRetryAsync<string>(
                    "TranscodeVideo",
                    new RetryOptions(TimeSpan.FromSeconds(5), 4) { Handle = ex => ex is InvalidCastException },
                    videoLocation);

                log.LogInformation("about to call thumbnail video activity");
                thumbnailLocation = await context.CallActivityAsync<string>("ExtractThumbnail", transcodedLocation);

                log.LogInformation("about to call prepend video activity");
                withIntroLocation = await context.CallActivityAsync<string>("PrependIntro", transcodedLocation);

            }
            catch (Exception e)
            {
                log.LogError($"Caught an error from an activity: {e.Message}");

                await context.CallActivityAsync<string>(
                    "Cleanup",
                    new object[] { transcodedLocation, thumbnailLocation, withIntroLocation });
            }

            return new
            {
                Transcoded = transcodedLocation,
                Thumbnail = thumbnailLocation,
                WithIntro = withIntroLocation
            };
        }
    }
}