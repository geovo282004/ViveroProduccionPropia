using System.ComponentModel.DataAnnotations;
using ViveroWebApi.Dominio.Enumeraciones;

namespace ViveroWebApi.DTOs
{
    // Para crear un pedido: el documento del cliente y cómo quiere comprar.
    public record PedidoRequest(
        [Required] string DocumentoCliente,
        bool ACredito,                 // true: se descuenta del cupo de crédito del cliente
        bool AceptaDespachoParcial,    // true: acepta recibir una parte ahora y el resto después
        string Observaciones);

    // Para agregar una línea: qué producto (por código) y cuántas unidades.
    public record LineaPedidoRequest(
        [Required] string CodigoProducto,
        [Range(1, int.MaxValue)] int Cantidad,
        TamanioMatera? TamanioMatera);   // solo para plantas: en qué tamaño de matera la quiere el cliente (null en insumos y herramientas)

    public record LineaPedidoResponse(
        int IdLinea,
        string CodigoProducto,
        string NombreProducto,
        TamanioMatera? TamanioMatera,
        int Cantidad,
        decimal PrecioUnitario,
        int CantidadComprometida,
        int CantidadDespachada,
        decimal Subtotal);

    public record PedidoResponse(
        int Numero,
        DateTime Fecha,
        EstadoPedido Estado,
        string DocumentoCliente,
        string NombreCliente,
        decimal Total,
        bool AlcanzaMinimo,
        bool ACredito,
        bool AceptaDespachoParcial,
        string Observaciones,
        List<LineaPedidoResponse> Lineas);
}
