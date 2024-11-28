#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UnistreamDemo.WebApi/UnistreamDemo.WebApi.csproj", "UnistreamDemo.WebApi/"]
RUN dotnet restore "UnistreamDemo.WebApi/UnistreamDemo.WebApi.csproj"
COPY . .
WORKDIR "/src/UnistreamDemo.WebApi"
RUN dotnet build "UnistreamDemo.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UnistreamDemo.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UnistreamDemo.WebApi.dll"]