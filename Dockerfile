FROM ://microsoft.com AS build
WORKDIR /app

# 1. Copiar todos los archivos de la raiz para no perder ninguna referencia
COPY . .

# 2. Restaurar dependencias usando la solucion completa (.sln)
RUN dotnet restore

# 3. Compilar y publicar la solucion entera
RUN dotnet publish -c Release -o /out

# 4. Etapa de ejecucion con el runtime oficial
FROM ://microsoft.com AS runtime
WORKDIR /app
COPY --from=build /out .

# Configurar puerto requerido por Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartAcademicAdvisor.Web.dll"]]