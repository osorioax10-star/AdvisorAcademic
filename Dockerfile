FROM ://microsoft.com AS build
WORKDIR /app

# Copiar archivos para restaurar dependencias
COPY *.sln .
COPY SmartAcademicAdvisor.Web/*.csproj ./SmartAcademicAdvisor.Web/
RUN dotnet restore

# Copiar todo el código y publicar
COPY . .
WORKDIR /app/SmartAcademicAdvisor.Web
RUN dotnet publish -c Release -o /out

# Etapa de ejecución
FROM ://microsoft.com AS runtime
WORKDIR /app
COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartAcademicAdvisor.Web.dll"]