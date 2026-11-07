using System.Text.Json.Serialization;
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

        // ---------- Inyección de dependencias ----------
        // Aquí se le dice a .NET: "cuando alguien pida esta INTERFAZ, dale esta CLASE".
        // Los repositorios son Singleton (una sola copia para toda la app) para que
        // los datos en memoria no se pierdan entre una petición y otra.

        // Productos (insumos, herramientas y especies comparten el mismo repositorio)
        builder.Services.AddSingleton<ProductoEnMemoriaRepositorio>();
        builder.Services.AddSingleton<IProductoRepositorio>(sp => sp.GetRequiredService<ProductoEnMemoriaRepositorio>());
        builder.Services.AddSingleton<IProductoLecturaRepositorio>(sp => sp.GetRequiredService<ProductoEnMemoriaRepositorio>());

        // Persona 1: insumos y herramientas
        builder.Services.AddScoped<IInsumoMapper, InsumoMapper>();
        builder.Services.AddScoped<IInsumoServicio, InsumoServicio>();
        builder.Services.AddScoped<IHerramientaMapper, HerramientaMapper>();
        builder.Services.AddScoped<IHerramientaServicio, HerramientaServicio>();

        // Persona 2: especies, lotes y labores
        builder.Services.AddScoped<IEspecieMapper, EspecieMapper>();
        builder.Services.AddScoped<IEspecieServicio, EspecieServicio>();

        // Persona 3: clientes y pedidos
        builder.Services.AddSingleton<ClienteEnMemoriaRepositorio>();
        builder.Services.AddSingleton<IClienteRepositorio>(sp => sp.GetRequiredService<ClienteEnMemoriaRepositorio>());
        builder.Services.AddSingleton<IClienteLecturaRepositorio>(sp => sp.GetRequiredService<ClienteEnMemoriaRepositorio>());
        builder.Services.AddScoped<IClienteMapper, ClienteMapper>();
        builder.Services.AddScoped<IClienteServicio, ClienteServicio>();

        builder.Services.AddSingleton<IPedidoRepositorio, PedidoEnMemoriaRepositorio>();
        builder.Services.AddScoped<IPedidoMapper, PedidoMapper>();
        builder.Services.AddScoped<IPedidoServicio, PedidoServicio>();

        var app = builder.Build();

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
