#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG ASPNET_VERSION=3.1

FROM mcr.microsoft.com/dotnet/core/sdk:$ASPNET_VERSION AS build

ARG VersionPrefix=0.0.0

WORKDIR /app

# copy csproj and restore as distinct layers
COPY Src/*.sln .
COPY Src/Contoso/*.csproj ./Contoso/
COPY Src/Contoso.UnitTests/*.csproj ./Contoso.UnitTests/
COPY Src/Contoso.LoadTests/*.csproj ./Contoso.LoadTests/
RUN dotnet restore

# copy everything else and build app
COPY Src/. .
WORKDIR /app/Contoso
RUN dotnet build -c Release /p:VersionPrefix=${VersionPrefix} /p:TreatWarningsAsErrors=true -warnaserror


FROM build AS unittestrunner
WORKDIR /app/Contoso.UnitTests
ENTRYPOINT ["dotnet", "test", "--logger:trx", "--collect:XPlat Code Coverage"]

FROM build AS unittest
WORKDIR /app/Contoso.UnitTests
RUN dotnet test --collect:"XPlat Code Coverage"

FROM build AS loadtestrunner
WORKDIR /app/Contoso.LoadTests
ENTRYPOINT ["dotnet", "test", "--logger:trx", "--results-directory", "/loadTestResults"]

FROM build AS loadtest
WORKDIR /app/Contoso.LoadTests
RUN dotnet test

FROM build AS publish
WORKDIR /app/Contoso
RUN dotnet publish -c Release -o /out


FROM mcr.microsoft.com/dotnet/core/aspnet:$ASPNET_VERSION AS runtime

WORKDIR /app
COPY --from=publish /out .
ENTRYPOINT ["dotnet", "Contoso.dll"]
