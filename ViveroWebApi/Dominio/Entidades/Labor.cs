namespace ViveroWebApi.Dominio.Entidades
{
public class Labor 
    {
        public int IdLabor { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int PeriodicidadDias{ get; set; } 

        public DateTime FechaUltimaRealizacion { get; set; }
        
        public DateTime FechaProxima { get; set; }

       public bool EstaVencida()
       {
           return DateTime.Today > FechaProxima;
       }

        public void RegistrarRealizacion(DateTime FechaRealizacion)
        {
            FechaUltimaRealizacion = FechaRealizacion;
            FechaProxima = FechaRealizacion.AddDays(PeriodicidadDias);
        }
        
    }
}
