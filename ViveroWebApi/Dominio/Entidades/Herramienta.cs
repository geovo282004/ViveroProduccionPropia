namespace ViveroWebApi.Dominio.Entidades
{
    public class Herramienta : Producto
    { 
        public string IdHerramienta { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public bool TieneGarantia { get; set; }
    
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