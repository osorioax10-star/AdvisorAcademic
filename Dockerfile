FROM ://microsoft.com AS build
WORKDIR /src

# Copiar todo el contenido del proyecto
COPY . .

# Restaurar y publicar la aplicación web
RUN dotnet restore
RUN dotnet publish "SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj" -c Release -o /app/publish

# Etapa final: Servidor de ejecución
FROM ://microsoft.com AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Configuración del puerto para Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartAcademicAdvisor.Web.dll"]