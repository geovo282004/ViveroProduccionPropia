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
        private readonly Dictionary<TamanioMatera, decimal> _precios = new Dictionary<TamanioMatera, decimal>();

        // Lista privada para que nadie agregue lotes sin control.
        private readonly List<Lote> _lotes = new List<Lote>();

        public Especie(string codigo, string nombre, string nombreCientifico,
                       decimal precioPequena, decimal precioMediana, decimal precioGrande)
            : base(codigo, nombre, precioPequena)   // el precio base es el de la matera pequeña
        {
            NombreCientifico = nombreCientifico;
            AsignarPrecios(precioPequena, precioMediana, precioGrande);
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
    }
}
