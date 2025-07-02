// Program.cs
using Microsoft.EntityFrameworkCore;
using MicroServicioTurismo.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS para desarrollo: permitir cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configurar cadena de conexión desde Docker Compose (DefaultConnection)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar controladores
builder.Services.AddControllers();

// (Opcional) OpenAPI si lo deseas
builder.Services.AddOpenApi();

var app = builder.Build();

// Usar CORS en desarrollo
app.UseCors("DevPolicy");

// Inicializar base de datos y datos de prueba
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Garantiza que la BD existe (si no hay migraciones, esto crea todo)
    dbContext.Database.EnsureCreated();

    // Aplica migraciones si existen
    dbContext.Database.Migrate();

    // Llama al inicializador de datos
    DbInitializer.Initialize(dbContext);
}

// Mapear controladores
app.MapControllers();

app.Run();
