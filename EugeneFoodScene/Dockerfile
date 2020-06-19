#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mvpstudio/dotnet:v01 as base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Server/EugeneFoodScene.Server.csproj", "Server/"]
COPY ["Client/EugeneFoodScene.Client.csproj", "Client/"]
COPY ["Shared/EugeneFoodScene.Shared.csproj", "Shared/"]
RUN dotnet restore "Server/EugeneFoodScene.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "EugeneFoodScene.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EugeneFoodScene.Server.csproj" -c Release -o /app/publish

FROM base AS final
USER mvp
WORKDIR /home/mvp/app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EugeneFoodScene.Server.dll"]
