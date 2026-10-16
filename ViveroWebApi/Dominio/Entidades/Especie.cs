namespace ViveroWebApi.Dominio.Entidades
{
public class Especie : Producto
    {
     public string IdEspecie { get; set; } = string.Empty;  

     public string NombreCientifico { get; set; } = string.Empty;

    public string NombreComun { get; set; } = string.Empty;

    public string TamanioMatera { get; set; } = string.Empty;

    public decimal PrecioPorTamanio { get; set; }

    public List<Lote> Lotes { get; set; } = new List<Lote>();   // Relación uno a muchos con Lote, faltaba esta linea o eso creo//

    public override int Disponibilidad () // agregué override para que pueda sobreescribir el método de la clase Producto, y así poder calcular la disponibilidad de la especie según los lotes que tiene
        {
            int total = 0;
            foreach (Lote lote in Lotes) // mira canasta por canasta
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