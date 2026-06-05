FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY BankRESTApi/BankRESTApi.csproj BankRESTApi/
RUN dotnet restore BankRESTApi/BankRESTApi.csproj
COPY BankRESTApi/ BankRESTApi/
RUN dotnet publish BankRESTApi/BankRESTApi.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BankRESTApi.dll"]
