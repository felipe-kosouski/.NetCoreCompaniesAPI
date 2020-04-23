FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

ENV SECRET=CodeMazeSecretKey

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["CompanyEmployees.csproj", "./"]
RUN dotnet restore "./CompanyEmployees.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CompanyEmployees.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CompanyEmployees.csproj" -c Release -o /app/publish
CMD ASPNETCORE_URLS=http://*:$PORT dotnet out/CompanyEmployees.dll


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CompanyEmployees.dll"]
