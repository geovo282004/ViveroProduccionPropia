using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ViveroWebApi.Data;
using ViveroWebApi.Excepciones;
using ViveroWebApi.Mappers;
using ViveroWebApi.Repositorios;
using ViveroWebApi.Servicios;

namespace ViveroWebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // JsonStringEnumConverter: los enum se ven como texto en Swagger ("Listo") y no como número (1).
        builder.Services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // ---------- Manejo global de errores (sin try/catch en los controladores) ----------
        builder.Services.AddExceptionHandler<ManejadorGlobalExcepciones>();
        builder.Services.AddProblemDetails();

        // ---------- Base de datos (SQLite con Entity Framework Core) ----------
        // La cadena de conexión NO va quemada en el código: se lee de appsettings.json.
        builder.Services.AddDbContext<ViveroDbContext>(opciones =>
            opciones.UseSqlite(builder.Configuration.GetConnectionString("ViveroDb")));

        // ---------- Inyección de dependencias ----------
        // Aquí se le dice a .NET: "cuando alguien pida esta INTERFAZ, dale esta CLASE".
        // Los repositorios ahora son de SQLite. Son Scoped (uno por petición) igual que el DbContext.
        // Para volver a la memoria bastaría cambiar ProductoSqliteRepositorio por ProductoEnMemoriaRepositorio.

        // Productos (insumos, herramientas y especies comparten el mismo repositorio)
        builder.Services.AddScoped<ProductoSqliteRepositorio>();
        builder.Services.AddScoped<IProductoRepositorio>(sp => sp.GetRequiredService<ProductoSqliteRepositorio>());
        builder.Services.AddScoped<IProductoLecturaRepositorio>(sp => sp.GetRequiredService<ProductoSqliteRepositorio>());

        // Persona 1: insumos y herramientas
        builder.Services.AddScoped<IInsumoMapper, InsumoMapper>();
        builder.Services.AddScoped<IInsumoServicio, InsumoServicio>();
        builder.Services.AddScoped<IHerramientaMapper, HerramientaMapper>();
        builder.Services.AddScoped<IHerramientaServicio, HerramientaServicio>();

        // Persona 2: especies, lotes y labores
        builder.Services.AddScoped<IEspecieMapper, EspecieMapper>();
        builder.Services.AddScoped<IEspecieServicio, EspecieServicio>();

        // Persona 3: clientes y pedidos
        builder.Services.AddScoped<ClienteSqliteRepositorio>();
        builder.Services.AddScoped<IClienteRepositorio>(sp => sp.GetRequiredService<ClienteSqliteRepositorio>());
        builder.Services.AddScoped<IClienteLecturaRepositorio>(sp => sp.GetRequiredService<ClienteSqliteRepositorio>());
        builder.Services.AddScoped<IClienteMapper, ClienteMapper>();
        builder.Services.AddScoped<IClienteServicio, ClienteServicio>();

        builder.Services.AddScoped<IPedidoRepositorio, PedidoSqliteRepositorio>();
        builder.Services.AddScoped<IPedidoMapper, PedidoMapper>();
        builder.Services.AddScoped<IPedidoServicio, PedidoServicio>();

        var app = builder.Build();

        // Crea el archivo vivero.db con sus tablas la primera vez que se ejecuta.
        using (var scope = app.Services.CreateScope())
        {
            ViveroDbContext db = scope.ServiceProvider.GetRequiredService<ViveroDbContext>();
            db.Database.EnsureCreated();
        }

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
