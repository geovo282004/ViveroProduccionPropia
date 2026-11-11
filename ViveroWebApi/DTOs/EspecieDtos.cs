using System.ComponentModel.DataAnnotations;
using ViveroWebApi.Dominio.Enumeraciones;

namespace ViveroWebApi.DTOs
{
    // Lo que llega para crear o actualizar una especie.
    public record EspecieRequest(
        [Required] string Codigo,
        [Required] string Nombre,
        [Required] string NombreCientifico,
        [Range(0, double.MaxValue)] decimal PrecioPequena,
        [Range(0, double.MaxValue)] decimal PrecioMediana,
        [Range(0, double.MaxValue)] decimal PrecioGrande);

    // Lo que llega para crear un lote.
    public record LoteRequest(
        [Required] string IdLote,
        DateTime FechaSiembra,
        [Range(1, int.MaxValue)] int CantidadSembrada,
        [Required] string Ubicacion,
        TamanioMatera TamanioMatera);

    // Lo que llega para crear una labor.
    public record LaborRequest(
        [Required] string Descripcion,
        [Range(1, int.MaxValue)] int PeriodicidadDias);

    // Cantidad de plantas muertas.
    public record BajaRequest(
        [Range(1, int.MaxValue)] int Cantidad);

    // Tamaño de matera al que se pasa el lote.
    public record TrasplanteRequest(
        TamanioMatera NuevoTamanio);

    public record LaborResponse(
        int IdLabor,
        string Descripcion,
        int PeriodicidadDias,
        DateTime? FechaUltimaRealizacion,
        DateTime FechaProxima,
        bool EstaVencida,
        bool EsCritica);

    // Lo que la API devuelve de un lote (con su precio según la matera).
    public record LoteResponse(
        string IdLote,
        DateTime FechaSiembra,
        int CantidadSembrada,
        int CantidadViva,
        int CantidadComprometida,
        int Disponibilidad,
        double Mortalidad,
        string Ubicacion,
        TamanioMatera TamanioMatera,
        decimal Precio,
        EstadoLote Estado,
        List<LaborResponse> Labores);

    // Lo que la API devuelve de una especie, con sus lotes.
    public record EspecieResponse(
        string Codigo,
        string Nombre,
        string NombreCientifico,
        int Disponibilidad,
        bool ActivoCatalogo,
        List<LoteResponse> Lotes);
}
