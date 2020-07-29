using Microsoft.CognitiveServices.Speech;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpeechSDKCommon
{
    public static class SpeechWrapper
    {
        public async static Task<string> RecognizeSpeechAsync()
        {
            var sb = new StringBuilder();
            var config = SpeechConfig.FromSubscription("405941c5988c4378bcf4ab407571bda8", "eastus2");
            // var config = SpeechConfig.FromSubscription("", "");
            var stopRecognition = new TaskCompletionSource<int>();
            var assembly = Assembly.GetAssembly(typeof(SpeechWrapper));
            var audioStream = assembly.GetManifestResourceStream("SpeechSDKCommon.test.wav");
            // Create an audio stream from a wav file.
            // Replace with your own audio file name.
            using (var audioInput = AzureSpeechHelpers.OpenWavFile(new BinaryReader(audioStream)))
            {
                // Creates a speech recognizer using audio stream input.
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {


                    recognizer.Recognized += (s, e) =>
                    {
                        if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            sb.Append(e.Result.Text);
                        }
                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            //  _logger.LogInformation("Speech could not be recognized");
                        }
                    };

                    recognizer.Canceled += (s, e) =>
                    {
                        //  _logger.LogInformation($"CANCELED: Reason={e.Reason}");

                        if (e.Reason == CancellationReason.Error)
                        {
                            //   _logger.LogInformation($"CANCELED:  ErrorCode={e.ErrorCode} ErrorDetails={e.ErrorDetails}");

                        }

                        stopRecognition.TrySetResult(0);
                    };

                    recognizer.SessionStopped += (s, e) =>
                    {
                        // _logger.LogInformation("Speech Recognition Session Stopped");
                        stopRecognition.TrySetResult(0);
                    };

                    // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    // Waits for completion.
                    // Use Task.WaitAny to keep the task rooted.
                    Task.WaitAny(new[] { stopRecognition.Task });  //should this be await Task.WhenAny??

                    // Stops recognition.
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                    return sb.ToString();
                }
            }
        }
    }
}
