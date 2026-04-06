FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY UCAA.HRMS.Domain/UCAA.HRMS.Domain.csproj UCAA.HRMS.Domain/
COPY UCAA.HRMS.Application/UCAA.HRMS.Application.csproj UCAA.HRMS.Application/
COPY UCAA.HRMS.Infrastructure/UCAA.HRMS.Infrastructure.csproj UCAA.HRMS.Infrastructure/
COPY UCAA.HRMS.API/UCAA.HRMS.API.csproj UCAA.HRMS.API/
RUN dotnet restore UCAA.HRMS.API/UCAA.HRMS.API.csproj

COPY . .
RUN dotnet publish UCAA.HRMS.API/UCAA.HRMS.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "UCAA.HRMS.API.dll"]
