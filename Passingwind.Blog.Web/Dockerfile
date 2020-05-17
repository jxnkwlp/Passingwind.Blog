#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Passingwind.Blog.Web/Passingwind.Blog.Web.csproj", "Passingwind.Blog.Web/"]
COPY ["Passingwind.Blog.Widgets/Passingwind.Blog.Widgets.csproj", "Passingwind.Blog.Widgets/"]
COPY ["Passingwind.Blog.Web.Core/Passingwind.Blog.Web.Core.csproj", "Passingwind.Blog.Web.Core/"]
COPY ["Passingwind.Blog.Core/Passingwind.Blog.Core.csproj", "Passingwind.Blog.Core/"]
COPY ["Passingwind.BlogML/Passingwind.BlogML.csproj", "Passingwind.BlogML/"]
RUN dotnet restore "Passingwind.Blog.Web/Passingwind.Blog.Web.csproj"
COPY . .
WORKDIR "/src/Passingwind.Blog.Web"
RUN dotnet build "Passingwind.Blog.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Passingwind.Blog.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Passingwind.Blog.Web.dll"]
