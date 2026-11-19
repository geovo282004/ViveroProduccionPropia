using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.Dominio.Excepciones;
using ViveroWebApi.DTOs;
using ViveroWebApi.Excepciones;
using ViveroWebApi.Mappers;
using ViveroWebApi.Repositorios;

namespace ViveroWebApi.Servicios
{
    public interface IPedidoServicio
    {
        List<PedidoResponse> ObtenerTodos();
        PedidoResponse ObtenerPorNumero(int numero);
        PedidoResponse Crear(PedidoRequest request);
        PedidoResponse AgregarLinea(int numero, LineaPedidoRequest request);
        PedidoResponse Reservar(int numero);
        PedidoResponse Despachar(int numero);
        PedidoResponse Cancelar(int numero);
    }

    public class PedidoServicio : IPedidoServicio
    {
        private readonly IPedidoRepositorio _pedidos;

        // SOLID - segregación de interfaces: de clientes y productos solo necesitamos LEER,
        // así que pedimos únicamente las interfaces de lectura.
        private readonly IClienteLecturaRepositorio _clientes;
        private readonly IProductoLecturaRepositorio _productos;
        private readonly IPedidoMapper _mapper;

        public PedidoServicio(IPedidoRepositorio pedidos, IClienteLecturaRepositorio clientes,
        IProductoLecturaRepositorio productos, IPedidoMapper mapper)
        {
            _pedidos = pedidos;
            _clientes = clientes;
            _productos = productos;
            _mapper = mapper;
        }

        public List<PedidoResponse> ObtenerTodos()
        {
            return _pedidos.ObtenerTodos().Select(_mapper.ARespuesta).ToList();
        }

        public PedidoResponse ObtenerPorNumero(int numero)
        {
            return _mapper.ARespuesta(BuscarPedido(numero));
        }

        public PedidoResponse Crear(PedidoRequest request)
        {
            Cliente? cliente = _clientes.ObtenerPorDocumento(request.DocumentoCliente);
            if (cliente == null)
                throw new RecursoNoEncontradoException($"No existe un cliente con documento {request.DocumentoCliente}.");

            Pedido pedido = new Pedido(_pedidos.SiguienteNumero(), cliente, request.ACredito, request.AceptaDespachoParcial);
            pedido.Observaciones = request.Observaciones ?? string.Empty;
            _pedidos.Guardar(pedido);
            return _mapper.ARespuesta(pedido);
        }

        public PedidoResponse AgregarLinea(int numero, LineaPedidoRequest request)
        {
            Pedido pedido = BuscarPedido(numero);

            Producto? producto = _productos.ObtenerPorCodigo(request.CodigoProducto);
            if (producto == null)
                throw new RecursoNoEncontradoException($"No existe un producto con código {request.CodigoProducto}.");

            // Toda la lógica (estado, producto activo, descuento, subtotal) está en el dominio.
            pedido.AgregarLinea(producto, request.Cantidad, request.TamanioMatera);
            _pedidos.Guardar(pedido);   // guarda el cambio en la base de datos
            return _mapper.ARespuesta(pedido);
        }

        public PedidoResponse Reservar(int numero)
        {
            Pedido pedido = BuscarPedido(numero);
            if (!pedido.Reservar())
                throw new PedidoInvalidoException(
                    "No se pudo reservar. Revise que el pedido esté en estado Creado, que el cliente esté activo y pueda reservar, " +
                    "que el total llegue a $50.000, que haya cupo de crédito si es a crédito, y que haya disponibilidad " +
                    "(o que el cliente acepte despacho parcial).");
            _pedidos.Guardar(pedido);   // guarda el pedido y el cupo usado del cliente
            return _mapper.ARespuesta(pedido);
        }

        public PedidoResponse Despachar(int numero)
        {
            Pedido pedido = BuscarPedido(numero);
            if (!pedido.DespacharPedido())
                throw new PedidoInvalidoException("Solo se puede despachar un pedido en estado Reservado.");
            _pedidos.Guardar(pedido);
            return _mapper.ARespuesta(pedido);
        }

        public PedidoResponse Cancelar(int numero)
        {
            Pedido pedido = BuscarPedido(numero);
            pedido.Cancelar();   // si ya está despachado, el dominio lanza PedidoInvalidoException
            _pedidos.Guardar(pedido);   // guarda el estado y el cupo liberado
            return _mapper.ARespuesta(pedido);
        }

        private Pedido BuscarPedido(int numero)
        {
            Pedido? pedido = _pedidos.ObtenerPorNumero(numero);
            if (pedido == null)
                throw new RecursoNoEncontradoException($"No existe el pedido número {numero}.");
            return pedido;
        }
    }
}
