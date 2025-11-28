# Etapa de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY ["EmployeeManagementAPI.csproj", "./"]
RUN dotnet restore "EmployeeManagementAPI.csproj"

# Copiar todo el código y compilar
COPY . .
RUN dotnet build "EmployeeManagementAPI.csproj" -c Release -o /app/build

# Publicar la aplicación
RUN dotnet publish "EmployeeManagementAPI.csproj" -c Release -o /app/publish

# Etapa de Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar archivos publicados
COPY --from=build /app/publish .

# Exponer puerto
EXPOSE 5000

# Punto de entrada
ENTRYPOINT ["dotnet", "EmployeeManagementAPI.dll"]