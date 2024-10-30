# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory for the build
WORKDIR /app

# Copy the csproj file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire project and build it
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Set the working directory for the runtime
WORKDIR /app

# Copy the published output from the build stage to the runtime stage
COPY --from=build /app/publish .

# Expose port 80 for the Blazor app
EXPOSE 80

# Set the entry point to run the Blazor Server app
ENTRYPOINT ["dotnet", "Chrip.Razor.dll"]
