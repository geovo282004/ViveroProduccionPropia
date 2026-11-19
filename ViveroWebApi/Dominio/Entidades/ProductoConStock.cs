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

        // Insumos y herramientas: apartar es como sacarlo del estante.
        // Ya no está disponible para otro pedido.
        public override void Apartar(int cantidad)
        {
            if (cantidad <= 0)
                throw new DatoInvalidoException("La cantidad a apartar debe ser mayor que cero.");
            if (cantidad > Disponibilidad())
                throw new DatoInvalidoException("No hay suficiente inventario para apartar.");
            CantidadDisponible = CantidadDisponible - cantidad;
        }

        // Si se cancela el pedido, vuelve al estante.
        public override void Liberar(int cantidad)
        {
            if (cantidad <= 0)
                throw new DatoInvalidoException("La cantidad a liberar debe ser mayor que cero.");
            CantidadDisponible = CantidadDisponible + cantidad;
        }

        // Ya se descontó del inventario al apartar, así que al entregar no se resta otra vez.
        public override void Entregar(int cantidad)
        {
            if (cantidad <= 0)
                throw new DatoInvalidoException("La cantidad a entregar debe ser mayor que cero.");
        }
    }
}