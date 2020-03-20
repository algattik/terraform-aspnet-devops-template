#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG ASPNET_VERSION=3.1

FROM mcr.microsoft.com/dotnet/core/aspnet:$ASPNET_VERSION AS base

WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:$ASPNET_VERSION AS build
WORKDIR /src
COPY ["Contoso/Contoso.csproj", "Contoso/"]
RUN dotnet restore "Contoso/Contoso.csproj"
COPY . .
WORKDIR "/src/Contoso"
RUN dotnet build "Contoso.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Contoso.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contoso.dll"]
