using ViveroWebApi.Dominio.Enumeraciones;
using ViveroWebApi.Dominio.Excepciones;

namespace ViveroWebApi.Dominio.Entidades
{
    // Grupo de plantas sembradas el mismo día en la misma matera.
    // Un lote tiene labores y vive dentro de una especie.
    public class Lote
    {
        // Si muere más de este porcentaje, el lote pasa a revisión.
        private const double LimiteMortalidad = 0.30;

        private string _idLote = string.Empty;
        private string _ubicacion = string.Empty;
        private int _cantidadSembrada;
        private int _cantidadViva;
        private int _cantidadComprometida;

        // Lista privada para que nadie agregue labores sin control.
        private readonly List<Labor> _labores = new List<Labor>();

        public Lote(string idLote, DateTime fechaSiembra, int cantidadSembrada,
                    string ubicacion, TamanioMatera tamanioMatera)
        {
            IdLote = idLote;
            FechaSiembra = fechaSiembra.Date;
            CantidadSembrada = cantidadSembrada;
            CantidadViva = cantidadSembrada;       // al inicio todas están vivas
            CantidadComprometida = 0;
            Ubicacion = ubicacion;
            TamanioMatera = tamanioMatera;
            Estado = EstadoLote.EnCrecimiento;
        }

        public string IdLote
        {
            get { return _idLote; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DatoInvalidoException("El id del lote no puede estar vacío.");
                _idLote = value;
            }
        }

        public DateTime FechaSiembra { get; private set; }

        public int CantidadSembrada
        {
            get { return _cantidadSembrada; }
            private set
            {
                if (value <= 0)
                    throw new DatoInvalidoException("Un lote debe tener al menos una planta sembrada.");
                _cantidadSembrada = value;
            }
        }

        public int CantidadViva
        {
            get { return _cantidadViva; }
            private set
            {
                if (value < 0)
                    throw new DatoInvalidoException("La cantidad viva no puede ser negativa.");
                if (value > CantidadSembrada)
                    throw new DatoInvalidoException("No puede haber más plantas vivas que sembradas.");
                _cantidadViva = value;
            }
        }

        public int CantidadComprometida
        {
            get { return _cantidadComprometida; }
            private set
            {
                if (value < 0)
                    throw new DatoInvalidoException("La cantidad comprometida no puede ser negativa.");
                if (value > CantidadViva)
                    throw new DatoInvalidoException("No se pueden comprometer más plantas que las vivas.");
                _cantidadComprometida = value;
            }
        }

        public string Ubicacion
        {
            get { return _ubicacion; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DatoInvalidoException("La ubicación del lote no puede estar vacía.");
                _ubicacion = value;
            }
        }

        // El tamaño de matera es del lote, no de la especie.
        public TamanioMatera TamanioMatera { get; private set; }

        public EstadoLote Estado { get; private set; }

        // Solo lectura; para agregar se usa AgregarLabor.
        public IReadOnlyList<Labor> Labores
        {
            get { return _labores.AsReadOnly(); }
        }

        // Porcentaje de plantas muertas (0.25 = 25 %).
        // Se usa (double) porque dividir dos int quita los decimales.
        public double CalcularMortalidad()
        {
            if (CantidadSembrada == 0)
                return 0;   // para no dividir por cero
            int bajas = CantidadSembrada - CantidadViva;
            return (double)bajas / CantidadSembrada;
        }

        // Plantas vivas que no están comprometidas en un pedido.
        public int CalcularDisponibilidad()
        {
            return CantidadViva - CantidadComprometida;
        }

        // Resta las plantas muertas. Si la mortalidad pasa de 30 %, el lote pasa a revisión.
        public void RegistrarBaja(int cantidad)
        {
            if (Estado == EstadoLote.Descartado)
                throw new DatoInvalidoException("No se pueden registrar bajas en un lote descartado.");
            if (cantidad <= 0)
                throw new DatoInvalidoException("La cantidad de bajas debe ser mayor que cero.");
            if (cantidad > CantidadViva)
                throw new DatoInvalidoException("No se pueden registrar más bajas que plantas vivas.");
            if (CantidadViva - cantidad < CantidadComprometida)
                throw new DatoInvalidoException("Las bajas dejarían menos plantas vivas que las comprometidas en pedidos.");

            CantidadViva = CantidadViva - cantidad;

            if (CalcularMortalidad() > LimiteMortalidad)
                Estado = EstadoLote.EnRevision;
        }

        // Compromete plantas para un pedido.
        public void Comprometer(int cantidad)
        {
            if (cantidad <= 0)
                throw new DatoInvalidoException("La cantidad a comprometer debe ser mayor que cero.");
            CantidadComprometida = CantidadComprometida + cantidad;
        }

        // Libera plantas comprometidas (por ejemplo si se cancela un pedido).
        public void LiberarComprometidas(int cantidad)
        {
            if (cantidad <= 0)
                throw new DatoInvalidoException("La cantidad a liberar debe ser mayor que cero.");
            CantidadComprometida = CantidadComprometida - cantidad;
        }

        public void MarcarListo()
        {
            if (Estado == EstadoLote.EnRevision)
                throw new DatoInvalidoException("Un lote en revisión no puede marcarse como listo hasta liberarse.");
            if (Estado == EstadoLote.Descartado)
                throw new DatoInvalidoException("Un lote descartado no puede marcarse como listo.");
            Estado = EstadoLote.Listo;
        }

        public void Descartar()
        {
            Estado = EstadoLote.Descartado;
        }

        // Solo deja pasar a una matera más grande.
        public void Trasplantar(TamanioMatera nuevoTamanio)
        {
            if (Estado == EstadoLote.Descartado)
                throw new DatoInvalidoException("No se puede trasplantar un lote descartado.");
            if (nuevoTamanio <= TamanioMatera)
                throw new DatoInvalidoException("El trasplante solo puede hacerse a una matera más grande.");
            TamanioMatera = nuevoTamanio;
        }

        // Crea una labor para este lote, contando desde hoy.
        public Labor AgregarLabor(string descripcion, int periodicidadDias)
        {
            Labor labor = new Labor(_labores.Count + 1, descripcion, periodicidadDias, DateTime.Today);
            _labores.Add(labor);
            return labor;
        }
    }
}
