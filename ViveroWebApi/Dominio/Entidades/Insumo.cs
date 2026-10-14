namespace ViveroWebApi.Dominio.Entidades
{
    public class Insumo : Producto
    {
     public string IdInsumo { get; set; } = string.Empty;
     public string UnidadMedida { get; set; } = string.Empty;
     public int CantidadDisponible { get; set; }
     public decimal CostoProveedor { get; set; }
    public string Estado { get; set; } = string.Empty;

    public  override int Disponibilidad()
    {
        return CantidadDisponible;
    }

    public void ActualizarStock(int nuevoStock)
    {
        CantidadDisponible = nuevoStock;
    }

    }
}



    