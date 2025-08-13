#!/bin/sh
set -e

echo "Running EF Core migrations..."
dotnet ef database update --project /src/MoneyTransfer/MoneyTransfer.csproj --no-build

