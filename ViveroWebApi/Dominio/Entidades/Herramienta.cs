using ViveroWebApi.Dominio.Enumeraciones;
namespace ViveroWebApi.Dominio.Entidades
{
    public class Herramienta  : ProductoConStock
    {
        // Constructor vacío solo para Entity Framework (lo usa al leer de la base de datos).
        private Herramienta()
        {
        }

        public Herramienta(string codigo, string nombre, decimal precio, int cantidadDisponible, string ubicacion, bool tieneGarantia) 

            : base(codigo, nombre, precio, cantidadDisponible)
        {
            Ubicacion = ubicacion;
            TieneGarantia = tieneGarantia;
        }
        public override TipoProducto Tipo
        {
            get { return TipoProducto.Herramienta; }
        }
        public string Ubicacion { get; set; } = string.Empty;
        public bool TieneGarantia { get; set; }
    }
    }
    
