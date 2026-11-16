using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.Dominio.Enumeraciones;
using ViveroWebApi.Dominio.Interfaces;

namespace ViveroWebApi.Data
{
    // El DbContext es "el puente" entre las clases y la base de datos (ORM con Entity Framework Core).
    // Cada DbSet es una tabla. OnModelCreating explica cómo se guarda cada clase.
    // Solo lo usan los repositorios: ni los controladores ni los servicios lo conocen.
    public class ViveroDbContext : DbContext
    {
        // Las opciones (qué base de datos y la cadena de conexión) llegan desde Program.cs.
        public ViveroDbContext(DbContextOptions<ViveroDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; } = null!;   // insumos, herramientas y especies
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Pedido> Pedidos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigurarProductos(modelBuilder);
            ConfigurarEspecies(modelBuilder);
            ConfigurarClientes(modelBuilder);
            ConfigurarPedidos(modelBuilder);
        }

        // ---------- Productos: herencia en una sola tabla ----------
        // Insumo, Herramienta y Especie heredan de Producto, así que van en la misma tabla "Productos".
        // La columna "TipoRegistro" dice qué clase es cada fila (esto se llama TPH: una tabla por jerarquía).
        private static void ConfigurarProductos(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>(producto =>
            {
                producto.ToTable("Productos");
                producto.HasKey(p => p.Codigo);
                producto.HasDiscriminator<string>("TipoRegistro")
                    .HasValue<Insumo>("Insumo")
                    .HasValue<Herramienta>("Herramienta")
                    .HasValue<Especie>("Especie");
            });

            // Clase intermedia abstracta (tiene CantidadDisponible para insumos y herramientas).
            modelBuilder.Entity<ProductoConStock>();
        }

        // ---------- Especies, lotes y labores ----------
        private static void ConfigurarEspecies(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Especie>(especie =>
            {
                // Los 3 precios por tamaño de matera están en un diccionario privado (_precios).
                // Se guardan en una sola columna de texto, así: {"Pequena":20000,"Mediana":30000,"Grande":45000}
                especie.Property<Dictionary<TamanioMatera, decimal>>("_precios")
                    .HasColumnName("Precios")
                    .HasConversion(
                        precios => PreciosATexto(precios),
                        texto => TextoAPrecios(texto),
                        new ValueComparer<Dictionary<TamanioMatera, decimal>>(
                            (a, b) => PreciosATexto(a!) == PreciosATexto(b!),
                            d => PreciosATexto(d).GetHashCode(),
                            d => TextoAPrecios(PreciosATexto(d))));

                // Una especie TIENE lotes (composición): los lotes no existen sin su especie.
                // OwnsMany = "le pertenecen". Tabla Lotes, llave = código de la especie + id del lote.
                especie.OwnsMany(e => e.Lotes, lote =>
                {
                    lote.ToTable("Lotes");
                    lote.WithOwner().HasForeignKey("EspecieCodigo");
                    lote.HasKey("EspecieCodigo", nameof(Lote.IdLote));

                    // Un lote TIENE labores. Llave = especie + lote + id de la labor.
                    lote.OwnsMany(l => l.Labores, labor =>
                    {
                        labor.ToTable("Labores");
                        labor.WithOwner().HasForeignKey("EspecieCodigo", "LoteIdLote");
                        labor.HasKey("EspecieCodigo", "LoteIdLote", nameof(Labor.IdLabor));
                        labor.Property(l => l.IdLabor).ValueGeneratedNever();   // el id lo pone el dominio
                    });
                });
            });
        }

        // ---------- Clientes ----------
        private static void ConfigurarClientes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(cliente =>
            {
                cliente.ToTable("Clientes");
                cliente.HasKey(c => c.Documento);

                // TipoCliente es una INTERFAZ (Particular, Jardinero o Empresa) y no se puede guardar directo.
                // Se guarda su nombre como texto y al leer se vuelve a crear el objeto (conversión de valor).
                cliente.Property(c => c.TipoCliente)
                    .HasConversion(
                        tipo => tipo.Nombre,
                        nombre => CrearTipoCliente(nombre));
            });
        }

        // ---------- Pedidos y líneas ----------
        private static void ConfigurarPedidos(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pedido>(pedido =>
            {
                pedido.ToTable("Pedidos");
                pedido.HasKey(p => p.Numero);
                pedido.Property(p => p.Numero).ValueGeneratedNever();   // el número lo pone el servicio

                // Muchos pedidos son de un cliente. Restrict: no deja borrar un cliente que tiene pedidos.
                pedido.HasOne(p => p.Cliente)
                    .WithMany()
                    .HasForeignKey("ClienteDocumento")
                    .OnDelete(DeleteBehavior.Restrict);

                // Un pedido TIENE líneas (composición). Llave = número del pedido + id de la línea.
                pedido.OwnsMany(p => p.Lineas, linea =>
                {
                    linea.ToTable("LineasPedido");
                    linea.WithOwner().HasForeignKey("PedidoNumero");
                    linea.HasKey("PedidoNumero", nameof(LineaPedido.IdLinea));
                    linea.Property(l => l.IdLinea).ValueGeneratedNever();

                    // Cada línea apunta a un producto. Restrict: no deja borrar un producto que está en un pedido.
                    linea.HasOne(l => l.Producto)
                        .WithMany()
                        .HasForeignKey("ProductoCodigo")
                        .OnDelete(DeleteBehavior.Restrict);
                });
            });
        }

        // ---------- Ayudantes para las conversiones ----------
        private static string PreciosATexto(Dictionary<TamanioMatera, decimal> precios)
        {
            return JsonSerializer.Serialize(precios);
        }

        private static Dictionary<TamanioMatera, decimal> TextoAPrecios(string texto)
        {
            return JsonSerializer.Deserialize<Dictionary<TamanioMatera, decimal>>(texto)
                   ?? new Dictionary<TamanioMatera, decimal>();
        }

        private static ITipoCliente CrearTipoCliente(string nombre)
        {
            if (nombre == "Jardinero") return new ClienteJardinero();
            if (nombre == "Empresa") return new ClienteEmpresa();
            return new ClienteParticular();
        }
    }
}
