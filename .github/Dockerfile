
# Dockerfile for .github
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY [".github.csproj", "./"]
RUN dotnet restore ".github.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build ".github.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ".github.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", ".github.dll"]
