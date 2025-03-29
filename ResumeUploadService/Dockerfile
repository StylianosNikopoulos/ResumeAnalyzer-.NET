# Use base ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ResumeUploadService/ResumeUploadService.csproj", "ResumeUploadService/"]
RUN dotnet restore "ResumeUploadService/ResumeUploadService.csproj"
COPY . .
WORKDIR "/src/ResumeUploadService"
RUN dotnet build "ResumeUploadService.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "ResumeUploadService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResumeUploadService.dll"]
