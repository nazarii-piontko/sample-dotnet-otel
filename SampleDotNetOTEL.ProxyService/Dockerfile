FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SampleDotNetOTEL.ProxyService/SampleDotNetOTEL.ProxyService.csproj", "SampleDotNetOTEL.ProxyService/"]
RUN dotnet restore "SampleDotNetOTEL.ProxyService/SampleDotNetOTEL.ProxyService.csproj"
COPY . .
WORKDIR "/src/SampleDotNetOTEL.ProxyService"
RUN dotnet build "SampleDotNetOTEL.ProxyService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SampleDotNetOTEL.ProxyService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleDotNetOTEL.ProxyService.dll"]
