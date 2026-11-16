using Microsoft.EntityFrameworkCore;
using ViveroWebApi.Data;
using ViveroWebApi.Dominio.Entidades;

namespace ViveroWebApi.Repositorios
{
    // Repositorios que guardan en SQLite usando Entity Framework (a través del ViveroDbContext).
    // Implementan LAS MISMAS interfaces que los repositorios en memoria, por eso los servicios
    // y los controladores no cambian: solo cambia el registro en Program.cs (inversión de dependencias).

    public class ProductoSqliteRepositorio : IProductoRepositorio
    {
        private readonly ViveroDbContext _db;

        public ProductoSqliteRepositorio(ViveroDbContext db)
        {
            _db = db;
        }

        public Producto? ObtenerPorCodigo(string codigo)
        {
            return _db.Productos.FirstOrDefault(p => p.Codigo == codigo);
        }

        public List<Producto> ObtenerTodos()
        {
            return _db.Productos.ToList();
        }

        // Si el producto es nuevo lo agrega; si ya existía, EF ya sabe qué cambió.
        // SaveChanges escribe los cambios en la base de datos.
        public void Guardar(Producto producto)
        {
            if (_db.Entry(producto).State == EntityState.Detached)
                _db.Productos.Add(producto);
            _db.SaveChanges();
        }

        public void Eliminar(string codigo)
        {
            Producto? producto = ObtenerPorCodigo(codigo);
            if (producto == null) return;
            _db.Productos.Remove(producto);
            _db.SaveChanges();
        }
    }

    public class ClienteSqliteRepositorio : IClienteRepositorio
    {
        private readonly ViveroDbContext _db;

        public ClienteSqliteRepositorio(ViveroDbContext db)
        {
            _db = db;
        }

        public Cliente? ObtenerPorDocumento(string documento)
        {
            return _db.Clientes.FirstOrDefault(c => c.Documento == documento);
        }

        public List<Cliente> ObtenerTodos()
        {
            return _db.Clientes.ToList();
        }

        public void Guardar(Cliente cliente)
        {
            if (_db.Entry(cliente).State == EntityState.Detached)
                _db.Clientes.Add(cliente);
            _db.SaveChanges();
        }

        public void Eliminar(string documento)
        {
            Cliente? cliente = ObtenerPorDocumento(documento);
            if (cliente == null) return;
            _db.Clientes.Remove(cliente);
            _db.SaveChanges();
        }
    }

    public class PedidoSqliteRepositorio : IPedidoRepositorio
    {
        private readonly ViveroDbContext _db;

        public PedidoSqliteRepositorio(ViveroDbContext db)
        {
            _db = db;
        }

        // Include = carga previa (eager loading): trae el pedido con su cliente
        // y cada línea con su producto, porque el pedido los necesita para calcular.
        private IQueryable<Pedido> PedidosCompletos()
        {
            return _db.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Lineas)
                    .ThenInclude(l => l.Producto);
        }

        public Pedido? ObtenerPorNumero(int numero)
        {
            return PedidosCompletos().FirstOrDefault(p => p.Numero == numero);
        }

        public List<Pedido> ObtenerTodos()
        {
            return PedidosCompletos().ToList();
        }

        public int SiguienteNumero()
        {
            if (!_db.Pedidos.Any()) return 1;
            return _db.Pedidos.Max(p => p.Numero) + 1;
        }

        // Guarda el pedido y TODO lo que cambió con él (líneas, cupo del cliente...).
        public void Guardar(Pedido pedido)
        {
            if (_db.Entry(pedido).State == EntityState.Detached)
                _db.Pedidos.Add(pedido);
            _db.SaveChanges();
        }
    }
}
