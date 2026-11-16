using ViveroWebApi.Dominio.Entidades;

namespace ViveroWebApi.Repositorios
{
    // Igual que en productos: una interfaz para leer y otra para escribir.
    public interface IClienteLecturaRepositorio
    {
        Cliente? ObtenerPorDocumento(string documento);
        List<Cliente> ObtenerTodos();
    }

    public interface IClienteEscrituraRepositorio
    {
        void Guardar(Cliente cliente);
        void Eliminar(string documento);
    }

    public interface IClienteRepositorio : IClienteLecturaRepositorio, IClienteEscrituraRepositorio
    {
    }

    public class ClienteEnMemoriaRepositorio : IClienteRepositorio
    {
        private readonly Dictionary<string, Cliente> _almacen = new Dictionary<string, Cliente>();

        public Cliente? ObtenerPorDocumento(string documento)
        {
            _almacen.TryGetValue(documento, out Cliente? cliente);
            return cliente;
        }

        public List<Cliente> ObtenerTodos()
        {
            return _almacen.Values.ToList();
        }

        public void Guardar(Cliente cliente)
        {
            _almacen[cliente.Documento] = cliente;
        }

        public void Eliminar(string documento)
        {
            _almacen.Remove(documento);
        }
    }
}
