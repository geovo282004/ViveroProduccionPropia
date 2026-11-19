namespace ViveroWebApi.Dominio.Memento
{
    // MEMENTO: una "foto" de cómo estaba un insumo en un momento.
    // Es un record: una vez creada, la foto no se puede cambiar.
    public record InsumoMemento(
        string Nombre,
        decimal Precio,
        int CantidadDisponible,
        string UnidadMedida,
        decimal CostoProveedor,
        DateTime? FechaVencimiento);
}