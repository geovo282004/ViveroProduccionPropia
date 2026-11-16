using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.DTOs;

namespace ViveroWebApi.Mappers
{
    // El pedido se crea con su constructor en el servicio (necesita el cliente y un número),
    // por eso este mapper solo convierte del dominio a la respuesta.
    public interface IPedidoMapper
    {
        PedidoResponse ARespuesta(Pedido pedido);
    }

    public class PedidoMapper : IPedidoMapper
    {
        public PedidoResponse ARespuesta(Pedido pedido)
        {
            List<LineaPedidoResponse> lineas = pedido.Lineas
                .Select(l => new LineaPedidoResponse(l.IdLinea, l.Producto.Codigo, l.Producto.Nombre,
                                                     l.Cantidad, l.PrecioUnitario, l.CantidadComprometida,
                                                     l.CantidadDespachada, l.Subtotal))
                .ToList();

            return new PedidoResponse(pedido.Numero, pedido.Fecha, pedido.Estado,
                                      pedido.Cliente.Documento, pedido.Cliente.Nombre,
                                      pedido.Total, pedido.AlcanzarMinimo(), pedido.ACredito,
                                      pedido.AceptaDespachoParcial, pedido.Observaciones, lineas);
        }
    }
}
