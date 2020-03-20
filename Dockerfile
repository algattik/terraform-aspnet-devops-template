#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG ASPNET_VERSION=3.1


FROM mcr.microsoft.com/dotnet/core/sdk:$ASPNET_VERSION AS build

WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Contoso/*.csproj ./Contoso/
COPY Contoso.UnitTests/*.csproj ./Contoso.UnitTests/
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /app/Contoso
RUN dotnet build


FROM build AS testrunner
WORKDIR /app/Contoso.UnitTests
ENTRYPOINT ["dotnet", "test", "--logger:trx"]


FROM build AS test
WORKDIR /app/Contoso.UnitTests
RUN dotnet test

FROM build AS publish
WORKDIR /app/Contoso
RUN dotnet publish -c Release -o /out


FROM mcr.microsoft.com/dotnet/core/aspnet:$ASPNET_VERSION AS runtime

WORKDIR /app
COPY --from=publish /out .
ENTRYPOINT ["dotnet", "Contoso.dll"]
