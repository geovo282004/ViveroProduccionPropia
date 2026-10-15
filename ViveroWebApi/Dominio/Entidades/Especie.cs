namespace ViveroWebApi.Dominio.Entidades
{
 public class Especie 
    {
     public string IdEspecie { get; set; } = string.Empty;  

     public string NombreCientifico { get; set; } = string.Empty;

    public string NombreComun { get; set; } = string.Empty;

    public string TamanioMatera { get; set; } = string.Empty;

    public decimal PrecioPorTamanio { get; set; }

    public int Disponibilidad ()
        {
            int total = 0;
            foreach (Lote lote in Lotes) # mira canasta por canasta
            {
                if (lote.Estado == "Listo")
                {
                    total += lote.CalcularDisponibilidad();
                }
            }
            return total;
        }
    public decimal CalcularPrecio()
        {
            return PrecioPorTamanio;
        }
    }    
}