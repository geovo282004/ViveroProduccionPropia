using ViveroWebApi.Dominio.Entidades;

namespace ViveroWebApi.Repositorios
{
    public interface IPedidoLecturaRepositorio
    {
        Pedido? ObtenerPorNumero(int numero);
        List<Pedido> ObtenerTodos();
        int SiguienteNumero();
    }

    public interface IPedidoEscrituraRepositorio
    {
        void Guardar(Pedido pedido);
    }

    public interface IPedidoRepositorio : IPedidoLecturaRepositorio, IPedidoEscrituraRepositorio
    {
    }

    public class PedidoEnMemoriaRepositorio : IPedidoRepositorio
    {
        private readonly Dictionary<int, Pedido> _almacen = new Dictionary<int, Pedido>();

        public Pedido? ObtenerPorNumero(int numero)
        {
            _almacen.TryGetValue(numero, out Pedido? pedido);
            return pedido;
        }

        public List<Pedido> ObtenerTodos()
        {
            return _almacen.Values.ToList();
        }

        // El próximo número de pedido: 1 si no hay ninguno, si no el mayor + 1.
        public int SiguienteNumero()
        {
            if (_almacen.Count == 0) return 1;
            return _almacen.Keys.Max() + 1;
        }

        public void Guardar(Pedido pedido)
        {
            _almacen[pedido.Numero] = pedido;
        }
    }
}
