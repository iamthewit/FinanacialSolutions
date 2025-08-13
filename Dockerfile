# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY MoneyTransfer/*.csproj ./MoneyTransfer/
RUN dotnet restore MoneyTransfer/MoneyTransfer.csproj
COPY MoneyTransfer/. ./MoneyTransfer/
WORKDIR /src/MoneyTransfer
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MoneyTransfer.dll"]


