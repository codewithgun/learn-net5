FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["learn-net5-webapi.csproj", "./"]
RUN dotnet restore "learn-net5-webapi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "learn-net5-webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "learn-net5-webapi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "learn-net5-webapi.dll"]
