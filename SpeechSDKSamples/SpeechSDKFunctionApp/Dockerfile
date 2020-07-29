#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:3.0-appservice AS base
RUN apt-get update
RUN apt-get -y install build-essential libasound2
WORKDIR /home/site/wwwroot
ENV LD_LIBRARY_PATH=/home/site/wwwroot/ \
    LD_DEBUG=libs
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SpeechSDKFunctionApp/SpeechSDKFunctionApp.csproj", "SpeechSDKFunctionApp/"]
COPY ["SpeechSDKCommon/SpeechSDKCommon.csproj", "SpeechSDKCommon/"]
RUN dotnet restore "SpeechSDKFunctionApp/SpeechSDKFunctionApp.csproj"
COPY . .
COPY /bin/runtimes/linux-x64/native/libMicrosoft.CognitiveServices.Speech.core.so ./libMicrosoft.CognitiveServices.Speech.core.dll.so
WORKDIR "/src/SpeechSDKFunctionApp"
RUN dotnet build "SpeechSDKFunctionApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpeechSDKFunctionApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
COPY --from=publish /app/publish/bin/runtimes/linux-x64/libMicrosoft.CognitiveServices.Speech.core.so ./libMicrosoft.CognitiveServices.Speech.core.dll.so
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true