using SmartAcademicAdvisor.Core.Interfaces;
using SmartAcademicAdvisor.Core.Models;
using SmartAcademicAdvisor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Register repositories
builder.Services.AddSingleton<IRepository<Alumno>>(sp => 
    new JsonRepository<Alumno>(sp.GetRequiredService<IWebHostEnvironment>(), "alumnos.json"));
builder.Services.AddSingleton<IRepository<Materia>>(sp => 
    new JsonRepository<Materia>(sp.GetRequiredService<IWebHostEnvironment>(), "materias.json"));
builder.Services.AddSingleton<IRepository<Especialidad>>(sp => 
    new JsonRepository<Especialidad>(sp.GetRequiredService<IWebHostEnvironment>(), "especialidades.json"));
builder.Services.AddSingleton<IRepository<Recomendacion>>(sp => 
    new JsonRepository<Recomendacion>(sp.GetRequiredService<IWebHostEnvironment>(), "recomendaciones.json"));

// Register services
builder.Services.AddScoped<AlumnoService>();
builder.Services.AddScoped<MateriaService>();
builder.Services.AddScoped<CertificacionService>();
builder.Services.AddScoped<RecommendationService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Index}/{id?}");

app.Run();
