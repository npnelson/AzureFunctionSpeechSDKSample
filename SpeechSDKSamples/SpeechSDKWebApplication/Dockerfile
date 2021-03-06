#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update
RUN apt-get -y install build-essential libasound2
ENV LD_DEBUG=libs

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SpeechSDKWebApplication/SpeechSDKWebApplication.csproj", "SpeechSDKWebApplication/"]
RUN dotnet restore "SpeechSDKWebApplication/SpeechSDKWebApplication.csproj"
COPY . .
WORKDIR "/src/SpeechSDKWebApplication"
RUN dotnet build "SpeechSDKWebApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpeechSDKWebApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpeechSDKWebApplication.dll"]