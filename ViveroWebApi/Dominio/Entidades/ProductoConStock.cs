using ViveroWebApi.Dominio.Excepciones;
namespace ViveroWebApi.Dominio.Entidades
{
    public abstract class ProductoConStock : Producto
    {
        private int _cantidadDisponible;
        // Constructor vacío solo para Entity Framework (lo usa al leer de la base de datos).
        protected ProductoConStock()
        {
        }

        protected ProductoConStock(string codigo, string nombre, decimal precio, int cantidadDisponible) 
        : base(codigo, nombre, precio)
        {
            CantidadDisponible = cantidadDisponible;
        }
        public int CantidadDisponible
        {
            get { return _cantidadDisponible; }
            private set
            {
                if (value < 0)
                    throw new DatoInvalidoException("La cantidad disponible no puede ser negativa.");
                _cantidadDisponible = value;
            }
        }
        public void ActualizarStock(int nuevoStock)
        {
            CantidadDisponible = nuevoStock;
        }
        public override int Disponibilidad()
        {
            return CantidadDisponible;
        }
    }
}