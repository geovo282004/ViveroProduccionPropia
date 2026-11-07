using ViveroWebApi.Dominio.Entidades;

namespace ViveroWebApi.Repositorios
{
    // SOLID - separación de responsabilidades (como en la clase del 31 de octubre):
    // una interfaz solo LEE y otra solo ESCRIBE.

    // Lectura de productos (insumos, herramientas y especies, porque todos son Producto).
    public interface IProductoLecturaRepositorio
    {
        Producto? ObtenerPorCodigo(string codigo);
        List<Producto> ObtenerTodos();
    }

    // Escritura de productos.
    public interface IProductoEscrituraRepositorio
    {
        void Guardar(Producto producto);
        void Eliminar(string codigo);
    }

    // Une las dos. Los servicios dependen de esta interfaz, no de la clase concreta
    // (inversión de dependencias).
    public interface IProductoRepositorio : IProductoLecturaRepositorio, IProductoEscrituraRepositorio
    {
    }

    // Implementación en memoria: guarda los productos en un diccionario (código -> producto).
    // Cuando conectemos SQLite, se crea otra clase que implemente IProductoRepositorio
    // y solo se cambia una línea en Program.cs.
    public class ProductoEnMemoriaRepositorio : IProductoRepositorio
    {
        private readonly Dictionary<string, Producto> _almacen = new Dictionary<string, Producto>();

        public Producto? ObtenerPorCodigo(string codigo)
        {
            _almacen.TryGetValue(codigo, out Producto? producto);
            return producto;
        }

        public List<Producto> ObtenerTodos()
        {
            return _almacen.Values.ToList();
        }

        public void Guardar(Producto producto)
        {
            _almacen[producto.Codigo] = producto;   // si ya existe, lo reemplaza
        }

        public void Eliminar(string codigo)
        {
            _almacen.Remove(codigo);
        }
    }
}
