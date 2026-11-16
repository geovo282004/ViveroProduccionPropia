using System.ComponentModel.DataAnnotations;

namespace ViveroWebApi.DTOs
{
    // TipoCliente llega como texto: "Particular", "Jardinero" o "Empresa".
    // El mapper lo convierte en el objeto que cumple la interfaz ITipoCliente.
    public record ClienteRequest(
        [Required] string Documento,
        [Required] string Nombre,
        [Required] string Correo,
        string Direccion,
        [Required] string TipoCliente,
        [Range(0, double.MaxValue)] decimal CupoCredito);

    public record ClienteResponse(
        string Documento,
        string Nombre,
        string Correo,
        string TipoCliente,
        bool Activo,
        bool PuedeReservar,
        decimal CupoCredito,
        decimal CupoDisponible);
}
