using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace VideoProcessor
{
    public static class ActivityFunctions
    {
        [FunctionName(nameof(TranscodeVideo))]
        public static async Task<string> TranscodeVideo([ActivityTrigger] string inputVideo, ILogger log)
        {
            log.LogInformation($"Transcoding {inputVideo}.");
            //simulate doing activity
            await Task.Delay(5000);

            return $"{Path.GetFileNameWithoutExtension(inputVideo)}-transcoded.mp4";
        }

        [FunctionName(nameof(ExtractThumbNail))]
        public static async Task<string> ExtractThumbNail([ActivityTrigger] string inputVideo, ILogger log)
        {
            log.LogInformation($"Extracting ThumbNail {inputVideo}.");
            //simulate doing activity
            await Task.Delay(5000);

            return $"{Path.GetFileNameWithoutExtension(inputVideo)}-tumbnail.png";
        }

        [FunctionName(nameof(PrependIntro))]
        public static async Task<string> PrependIntro([ActivityTrigger] string inputVideo, ILogger log)
        {
            var introLocation = Environment.GetEnvironmentVariable("IntroLocation");

            log.LogInformation($"Prepending Intro {introLocation} to {inputVideo}.");

            //simulate doing activity
            await Task.Delay(5000);

            return $"{Path.GetFileNameWithoutExtension(inputVideo)}-withintro.mp4";
        }

        [FunctionName(nameof(Cleanup))]
        public static async Task<string> Cleanup([ActivityTrigger] string[] filesToCleanUp, ILogger log)
        {
            foreach (var file in filesToCleanUp)
            {
                log.LogInformation($"Deleting {file}");

                // simulate doing activity
                await Task.Delay(1000);
            }

            return "Cleaned up successfully";
        }
    }
}