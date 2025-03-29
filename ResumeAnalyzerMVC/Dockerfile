# Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ResumeAnalyzerMVC/ResumeAnalyzerMVC.csproj", "ResumeAnalyzerMVC/"]
COPY ["AuthService/AuthService.csproj", "AuthService/"]
COPY ["UserService/UserService.csproj", "UserService/"]
COPY ["ResumeUploadService/ResumeUploadService.csproj", "ResumeUploadService/"]
RUN dotnet restore "ResumeAnalyzerMVC/ResumeAnalyzerMVC.csproj"

# Copy all source files
COPY . .
WORKDIR "/src/ResumeAnalyzerMVC"

# Build the project
RUN dotnet build "ResumeAnalyzerMVC.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ResumeAnalyzerMVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResumeAnalyzerMVC.dll"]
