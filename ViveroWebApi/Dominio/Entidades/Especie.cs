using ViveroWebApi.Dominio.Enumeraciones;
using ViveroWebApi.Dominio.Excepciones;

namespace ViveroWebApi.Dominio.Entidades
{
    // Una especie es un producto, por eso se puede vender en un pedido.
    // Su Codigo es el id y su Nombre es el nombre común (vienen de Producto).
    // Una especie tiene lotes.
    public class Especie : Producto
    {
        private string _nombreCientifico = string.Empty;

        // Precio según el tamaño de matera.
        private Dictionary<TamanioMatera, decimal> _precios = new Dictionary<TamanioMatera, decimal>();

        // Lista privada para que nadie agregue lotes sin control.
        private readonly List<Lote> _lotes = new List<Lote>();

        public Especie(string codigo, string nombre, string nombreCientifico,
            decimal precioPequena, decimal precioMediana, decimal precioGrande)
            : base(codigo, nombre, precioPequena)   // el precio base es el de la matera pequeña
        {
            NombreCientifico = nombreCientifico;
            AsignarPrecios(precioPequena, precioMediana, precioGrande);
        }

        // Constructor vacío solo para Entity Framework (lo usa al leer de la base de datos).
        private Especie()
        {
        }

        public override TipoProducto Tipo
        {
            get { return TipoProducto.Planta; }
        }

        public string NombreCientifico
        {
            get { return _nombreCientifico; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DatoInvalidoException("El nombre científico no puede estar vacío.");
                _nombreCientifico = value;
            }
        }

        // Solo lectura; para agregar se usa AgregarLote.
        public IReadOnlyList<Lote> Lotes
        {
            get { return _lotes.AsReadOnly(); }
        }

        // Precio según el tamaño de matera.
        public decimal CalcularPrecio(TamanioMatera tamanio)
        {
            return _precios[tamanio];
        }

        // Cambia los tres precios.
        public void ActualizarPrecios(decimal precioPequena, decimal precioMediana, decimal precioGrande)
        {
            AsignarPrecios(precioPequena, precioMediana, precioGrande);
            ActualizarPrecio(precioPequena);   // actualiza el precio base
        }

        private void AsignarPrecios(decimal precioPequena, decimal precioMediana, decimal precioGrande)
        {
            if (precioPequena < 0 || precioMediana < 0 || precioGrande < 0)
                throw new DatoInvalidoException("Los precios no pueden ser negativos.");
            if (precioMediana < precioPequena || precioGrande < precioMediana)
                throw new DatoInvalidoException("Una matera más grande no puede costar menos que una más pequeña.");

            _precios[TamanioMatera.Pequena] = precioPequena;
            _precios[TamanioMatera.Mediana] = precioMediana;
            _precios[TamanioMatera.Grande] = precioGrande;
        }

        public void AgregarLote(Lote lote)
        {
            if (lote == null)
                throw new DatoInvalidoException("El lote no puede ser nulo.");
            if (ObtenerLote(lote.IdLote) != null)
                throw new DatoInvalidoException($"Ya existe un lote con id {lote.IdLote} en esta especie.");
            _lotes.Add(lote);
        }

        // Devuelve null si el lote no existe.
        public Lote? ObtenerLote(string idLote)
        {
            return _lotes.FirstOrDefault(l => l.IdLote == idLote);
        }

        // Suma lo disponible de los lotes que están en estado Listo.
        public override int Disponibilidad()
        {
            int total = 0;
            foreach (Lote lote in _lotes)
            {
                if (lote.Estado == EstadoLote.Listo)
                {
                    total += lote.CalcularDisponibilidad();
                }
            }
            return total;

        
        }
                // El precio de una planta depende del tamaño de la matera.
        public override decimal PrecioPara(TamanioMatera? tamanio)
        {
            if (tamanio == null)
                throw new DatoInvalidoException("Para una planta hay que indicar el tamaño de matera (Pequena, Mediana o Grande).");
            return CalcularPrecio(tamanio.Value);
        }

        // Plantas listas, pero solo de los lotes con ese tamaño de matera.
        public override int DisponibilidadPara(TamanioMatera? tamanio)
        {
            int total = 0;
            foreach (Lote lote in _lotes)
            {
                if (lote.Estado == EstadoLote.Listo && EsDelTamanio(lote, tamanio))
                {
                    total += lote.CalcularDisponibilidad();
                }
            }
            return total;
        }

        // Si no se pide tamaño, sirve cualquier lote.
        private static bool EsDelTamanio(Lote lote, TamanioMatera? tamanio)
        {
            return tamanio == null || lote.TamanioMatera == tamanio;
        }

        // Plantas: apartar es comprometerlas en los lotes listos, empezando por el primero.
        public override void Apartar(int cantidad, TamanioMatera? tamanio)
        {
            if (cantidad <= 0)
                throw new DatoInvalidoException("La cantidad a apartar debe ser mayor que cero.");
            if (cantidad > DisponibilidadPara(tamanio))
                throw new DatoInvalidoException("No hay suficientes plantas listas para apartar de ese tamaño.");

            int falta = cantidad;
            foreach (Lote lote in _lotes)
            {
                if (falta == 0) break;
                if (lote.Estado != EstadoLote.Listo) continue;
                if (!EsDelTamanio(lote, tamanio)) continue;

                int tomar = Math.Min(lote.CalcularDisponibilidad(), falta);
                if (tomar > 0)
                {
                    lote.Comprometer(tomar);
                    falta = falta - tomar;
                }
            }
        }

        // Si se cancela el pedido, las plantas comprometidas vuelven a estar libres.
        public override void Liberar(int cantidad, TamanioMatera? tamanio)
        {
            int falta = cantidad;
            foreach (Lote lote in _lotes)
            {
                if (falta == 0) break;
                if (!EsDelTamanio(lote, tamanio)) continue;
                int tomar = Math.Min(lote.CantidadComprometida, falta);
                if (tomar > 0)
                {
                    lote.LiberarComprometidas(tomar);
                    falta = falta - tomar;
                }
            }
        }

        // Al despachar, las plantas comprometidas salen del vivero.
        public override void Entregar(int cantidad, TamanioMatera? tamanio)
        {
            int falta = cantidad;
            foreach (Lote lote in _lotes)
            {
                if (falta == 0) break;
                if (!EsDelTamanio(lote, tamanio)) continue;
                int tomar = Math.Min(lote.CantidadComprometida, falta);
                if (tomar > 0)
                {
                    lote.Entregar(tomar);
                    falta = falta - tomar;
                }
            }
        }
    }
}
