using ViveroWebApi.Dominio.Enumeraciones;
using ViveroWebApi.Dominio.Excepciones;

namespace ViveroWebApi.Dominio.Entidades
{
    // Una Especie ES un Producto (herencia), por eso puede venderse en un pedido.
    // Su Codigo es el id y su Nombre es el nombre común: no se repiten, ya vienen del padre.
    // Una Especie TIENE lotes (composición).
    public class Especie : Producto
    {
        private string _nombreCientifico = string.Empty;

        // Diccionario tamaño de matera -> precio. Así el precio se busca sin usar if.
        private readonly Dictionary<TamanioMatera, decimal> _precios = new Dictionary<TamanioMatera, decimal>();

        // Lista privada: nadie desde afuera puede agregar o borrar lotes sin control.
        private readonly List<Lote> _lotes = new List<Lote>();

        public Especie(string codigo, string nombre, string nombreCientifico,
                       decimal precioPequena, decimal precioMediana, decimal precioGrande)
            : base(codigo, nombre, precioPequena)   // el precio base del producto es el de la matera pequeña
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

        // Desde afuera la lista es de solo lectura; para agregar se usa AgregarLote.
        public IReadOnlyList<Lote> Lotes
        {
            get { return _lotes.AsReadOnly(); }
        }

        // Precio de la planta según el tamaño de matera del lote.
        public decimal CalcularPrecio(TamanioMatera tamanio)
        {
            return _precios[tamanio];
        }

        // Cambia los tres precios a la vez, validando que tengan sentido.
        public void ActualizarPrecios(decimal precioPequena, decimal precioMediana, decimal precioGrande)
        {
            AsignarPrecios(precioPequena, precioMediana, precioGrande);
            ActualizarPrecio(precioPequena);   // mantiene el precio base del producto al día
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

        // Busca un lote de esta especie. Devuelve null si no existe (el servicio decide el 404).
        public Lote? ObtenerLote(string idLote)
        {
            return _lotes.FirstOrDefault(l => l.IdLote == idLote);
        }

        // Polimorfismo: Insumo y Herramienta devuelven lo de bodega; la Especie suma
        // solo lo disponible de los lotes que están en estado Listo.
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
