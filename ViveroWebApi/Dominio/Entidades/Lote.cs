namespace ViveroWebApi.Dominio.Entidades
{
    public class Lote
    {
        public string   IdLote { get; set; } = string.Empty;
        public DateTime FechaSiembra { get; set; }
        public int CantidadSembrada { get; set; }
        public int CantidadViva { get; set; }
        public int CantidadComprometida{ get; set; }
        public string Ubicacion { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;
        public List<Labor> Labores { get; set; } = new List<Labor>();

        public double CalcularMortalidad()
        {
            int bajas = CantidadSembrada - CantidadViva;
            return (double)bajas / CantidadSembrada;
        }
        public int CalcularDisponibilidad()
        {
            return CantidadViva - CantidadComprometida;
        }

        public void RegistrarBaja(int cantidad)
        {
            CantidadViva = CantidadViva - cantidad;
        }

        public void MarcarListo ()
        {
            Estado = "Listo";
        }

        public void MarcarEnRevision()
        {
            Estado = "En revision";
        }

        public void Descartar () 
        {
            Estado = "Descartado";
        }
    }
}



    