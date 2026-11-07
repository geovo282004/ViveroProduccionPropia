using ViveroWebApi.Dominio.Excepciones;

namespace ViveroWebApi.Dominio.Entidades
{
    // Tarea de mantenimiento que se repite cada cierto número de días (riego, poda, abono...).
    // Un Lote "tiene" labores (composición): una labor no existe sin su lote.
    public class Labor
    {
        private string _descripcion = string.Empty;
        private int _periodicidadDias;

        // fechaBase es el día desde el cual se empieza a contar: la primera vez que toca
        // hacer la labor será fechaBase + periodicidadDias.
        public Labor(int idLabor, string descripcion, int periodicidadDias, DateTime fechaBase)
        {
            IdLabor = idLabor;
            Descripcion = descripcion;
            PeriodicidadDias = periodicidadDias;
            FechaProxima = fechaBase.Date.AddDays(periodicidadDias);
        }

        public int IdLabor { get; private set; }

        public string Descripcion
        {
            get { return _descripcion; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DatoInvalidoException("La descripción de la labor no puede estar vacía.");
                _descripcion = value;
            }
        }

        public int PeriodicidadDias
        {
            get { return _periodicidadDias; }
            private set
            {
                if (value <= 0)
                    throw new DatoInvalidoException("La periodicidad de la labor debe ser de al menos 1 día.");
                _periodicidadDias = value;
            }
        }

        // null mientras la labor nunca se haya realizado.
        public DateTime? FechaUltimaRealizacion { get; private set; }

        public DateTime FechaProxima { get; private set; }

        // Una labor está vencida si hoy ya pasó la fecha en la que tocaba hacerla.
        public bool EstaVencida()
        {
            return DateTime.Today > FechaProxima;
        }

        // Según el documento de dominio, una labor es crítica si lleva vencida
        // más de la mitad de su periodicidad.
        public bool EsCritica()
        {
            if (!EstaVencida())
                return false;
            double diasVencida = (DateTime.Today - FechaProxima).TotalDays;
            return diasVencida > PeriodicidadDias / 2.0;
        }

        // Guarda cuándo se hizo y calcula la próxima vez sumando la periodicidad en días.
        public void RegistrarRealizacion(DateTime fechaRealizacion)
        {
            FechaUltimaRealizacion = fechaRealizacion.Date;
            FechaProxima = fechaRealizacion.Date.AddDays(PeriodicidadDias);
        }
    }
}
