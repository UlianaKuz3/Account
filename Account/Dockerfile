FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY . .
RUN dotnet restore "Account.csproj"
RUN dotnet publish "Account.csproj" -c Release -o /app/publish 

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Account.dll"]