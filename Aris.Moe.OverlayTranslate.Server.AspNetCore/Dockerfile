FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Aris.Moe.OverlayTranslate.Server.AspNetCore/Aris.Moe.OverlayTranslate.Server.AspNetCore.csproj", "Aris.Moe.OverlayTranslate.Server.AspNetCore/"]
RUN dotnet restore "Aris.Moe.OverlayTranslate.Server.AspNetCore/Aris.Moe.OverlayTranslate.Server.AspNetCore.csproj"
COPY . .
WORKDIR "/src/Aris.Moe.OverlayTranslate.Server.AspNetCore"
RUN dotnet build "Aris.Moe.OverlayTranslate.Server.AspNetCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aris.Moe.OverlayTranslate.Server.AspNetCore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aris.Moe.OverlayTranslate.Server.AspNetCore.dll"]
