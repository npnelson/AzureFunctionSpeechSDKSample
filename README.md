# AzureFunctionSpeechSDKSample

This sample demonstrates what I believe to be a bug, possibily with the way Microsoft.NET.Sdk.Functions 3.0.9 handles the copying of native dependencies. I say possibly because it might also be a problem with the way Azure Functions Host resolves native dependencies.

This sample uses Microsoft.CognitiveServices.Speech to recognize some speech in the embedded test.wav file. For convenience, it includes hardcoded credentials to a Free tier speech service in Azure. If trolls start to hit it, it will be shut down and this readme updated.     

The problem can be demonstrated by opening the solution in Visual Studio 2019 (16.6.5). Launch the SpeechSDKFunctionNotWorking in **Release** and **Docker** and then attempt to hit `http://<yourhost:port>/api/TestSpeech`. 

**DO NOT RUN IN IT IN DEBUG/DOCKER, that might expose other issues such as** https://github.com/Azure/azure-functions-host/issues/5950#issuecomment-635521230

The sample code catches the exception thrown and returns it to the client. In this case, I see a `Unable to load shared library 'libMicrosoft.CognitiveServices.Speech.core.so error'`

If you run it locally on Windows, it runs fine and returns `Good test of speech recognition.`, which is what the embedded test.wav file contains speech of.

Looking at the container logs (with LD_DEBUG = library) you can see:

<pre><code>
Executing 'TestSpeech' (Reason='This function was programmatically called via the host APIs.', Id=55942fd7-5752-40b3-88d0-90a421f4599e)
        23:	find library=libMicrosoft.CognitiveServices.Speech.core.so.so [0]; searching
        23:	 search cache=/etc/ld.so.cache
        23:	 search path=/lib/x86_64-linux-gnu:/usr/lib/x86_64-linux-gnu:/lib:/usr/lib		(system search path)
        23:	  trying file=/lib/x86_64-linux-gnu/libMicrosoft.CognitiveServices.Speech.core.so.so
        23:	  trying file=/usr/lib/x86_64-linux-gnu/libMicrosoft.CognitiveServices.Speech.core.so.so
        23:	  trying file=/lib/libMicrosoft.CognitiveServices.Speech.core.so.so
        23:	  trying file=/usr/lib/libMicrosoft.CognitiveServices.Speech.core.so.so
        23:	
        23:	find library=liblibMicrosoft.CognitiveServices.Speech.core.so.so [0]; searching
        23:	 search cache=/etc/ld.so.cache
        23:	 search path=/lib/x86_64-linux-gnu:/usr/lib/x86_64-linux-gnu:/lib:/usr/lib		(system search path)
        23:	  trying file=/lib/x86_64-linux-gnu/liblibMicrosoft.CognitiveServices.Speech.core.so.so
        23:	  trying file=/usr/lib/x86_64-linux-gnu/liblibMicrosoft.CognitiveServices.Speech.core.so.so
        23:	  trying file=/lib/liblibMicrosoft.CognitiveServices.Speech.core.so.so
        23:	  trying file=/usr/lib/liblibMicrosoft.CognitiveServices.Speech.core.so.so
        23:	       
        23:	find library= <b>libMicrosoft.CognitiveServices.Speech.core.so</b> [0]; searching
        23:	 search cache=/etc/ld.so.cache
        23:	 search path=/lib/x86_64-linux-gnu:/usr/lib/x86_64-linux-gnu:/lib:/usr/lib		(system search path)
        23:	  trying file=/lib/x86_64-linux-gnu/libMicrosoft.CognitiveServices.Speech.core.so
        23:	  trying file=/usr/lib/x86_64-linux-gnu/libMicrosoft.CognitiveServices.Speech.core.so
        23:	  trying file=/lib/libMicrosoft.CognitiveServices.Speech.core.so
        23:	  trying file=/usr/lib/libMicrosoft.CognitiveServices.Speech.core.so
        23:	
        23:	find library=liblibMicrosoft.CognitiveServices.Speech.core.so [0]; searching
        23:	 search cache=/etc/ld.so.cache
        23:	 search path=/lib/x86_64-linux-gnu:/usr/lib/x86_64-linux-gnu:/lib:/usr/lib		(system search path)
        23:	  trying file=/lib/x86_64-linux-gnu/liblibMicrosoft.CognitiveServices.Speech.core.so
        23:	  trying file=/usr/lib/x86_64-linux-gnu/liblibMicrosoft.CognitiveServices.Speech.core.so
        23:	  trying file=/lib/liblibMicrosoft.CognitiveServices.Speech.core.so
        23:	  trying file=/usr/lib/liblibMicrosoft.CognitiveServices.Speech.core.so
</pre>

**It cannot find the libMicrosoft.CognitiveServices.Speech.core.so file that is buried in the /home/site/wwwroot/bin/runtimes/linux64/native directory.**

Perhaps these ".so" files should be copied to the bin directory alongside Microsoft.CognitiveServices.Speech.csharp.dll?

Also included is an ASP.NET Web Application (SpeechSDKWebApplication) that runs fine in Visual Studio with docker/release mode. In that sample, all you have to do is launch the app and hit it with a browser and you can see how it recognizes the speech properly.
