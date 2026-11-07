using System.ComponentModel.DataAnnotations;

namespace ViveroWebApi.DTOs
{
    // Lo que llega para crear o actualizar una herramienta.
    public record HerramientaRequest(
        [Required] string Codigo,
        [Required] string Nombre,
        [Range(0, double.MaxValue)] decimal Precio,
        [Range(0, int.MaxValue)] int CantidadDisponible,
        string Ubicacion,
        bool TieneGarantia);

    // Lo que la API devuelve de una herramienta.
    public record HerramientaResponse(
        string Codigo,
        string Nombre,
        decimal Precio,
        bool TieneGarantia,
        int Disponibilidad,
        bool ActivoCatalogo);
}
