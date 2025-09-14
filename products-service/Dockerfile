
            # Dockerfile for products-service
            FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
            WORKDIR /app
            EXPOSE 80

            FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
            WORKDIR /src
            COPY ["products-service/products-service.csproj", "products-service/"]
            RUN dotnet restore "products-service/products-service.csproj"
            COPY . .
            WORKDIR "/src/products-service"
            RUN dotnet build "products-service.csproj" -c Release -o /app/build

            FROM build AS publish
            RUN dotnet publish "products-service.csproj" -c Release -o /app/publish

            FROM base AS final
            WORKDIR /app
            COPY --from=publish /app/publish .
            ENTRYPOINT ["dotnet", "products-service.dll"]
            