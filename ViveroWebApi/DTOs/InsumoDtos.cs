using System.ComponentModel.DataAnnotations;
using ViveroWebApi.Dominio.Enumeraciones;

namespace ViveroWebApi.DTOs
{
    // DTO = Data Transfer Object: solo los datos que viajan entre la API y quien la usa.
    // Son "record": clases cortas que solo guardan datos (como explicó el profe).

    // Request: lo que LLEGA para crear o actualizar un insumo.
    public record InsumoRequest(
        [Required] string Codigo,
        [Required] string Nombre,
        [Range(0, double.MaxValue)] decimal Precio,
        [Range(0, int.MaxValue)] int CantidadDisponible,
        [Required] string UnidadMedida,
        [Range(0, double.MaxValue)] decimal CostoProveedor,
        DateTime? FechaVencimiento);   // puede ir vacío (null) si el insumo no vence

    // Response: lo que la API DEVUELVE. No tiene por qué ser igual a la clase del dominio.
    public record InsumoResponse(
        string Codigo,
        string Nombre,
        decimal Precio,
        string UnidadMedida,
        int Disponibilidad,
        EstadoInsumo Estado,
        DateTime? FechaVencimiento,
        bool ActivoCatalogo);
}
