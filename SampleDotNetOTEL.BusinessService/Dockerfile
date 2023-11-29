FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SampleDotNetOTEL.BusinessService/SampleDotNetOTEL.BusinessService.csproj", "SampleDotNetOTEL.BusinessService/"]
RUN dotnet restore "SampleDotNetOTEL.BusinessService/SampleDotNetOTEL.BusinessService.csproj"
COPY . .
WORKDIR "/src/SampleDotNetOTEL.BusinessService"
RUN dotnet build "SampleDotNetOTEL.BusinessService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SampleDotNetOTEL.BusinessService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleDotNetOTEL.BusinessService.dll"]
