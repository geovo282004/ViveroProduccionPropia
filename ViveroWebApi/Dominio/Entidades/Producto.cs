using ViveroWebApi.Dominio.Enumeraciones;
using ViveroWebApi.Dominio.Excepciones;

namespace ViveroWebApi.Dominio.Entidades
{

    public abstract class Producto

    {
        protected Producto(string codigo, string nombre, decimal precio)
        {
            Codigo = codigo;
            Nombre = nombre;
            Precio = precio;
            ActivoCatalogo = true;
        }
        private string _codigo = string.Empty;   // la caja fuerte (privada)

        public string Codigo                      // la puerta
        {
            get { return _codigo; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DatoInvalidoException("El código del producto no puede estar vacío.");
                _codigo = value;
            }
        }

        private string _nombre = string.Empty;
        public string Nombre
        {
            get { return _nombre; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DatoInvalidoException("El nombre del producto no puede estar vacío.");
                _nombre = value;
            }
        }
        private decimal _precio;
        public decimal Precio
        {
            get { return _precio; }
            private set
            {
                if (value < 0)
                    throw new DatoInvalidoException("El precio del producto no puede ser negativo.");
                _precio = value;
            }
        }
        public bool ActivoCatalogo { get; private set; }

        public abstract TipoProducto Tipo { get; }

        // Abstracto: cada hijo dice cómo calcula su disponibilidad.
        public abstract int Disponibilidad();

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            Precio = nuevoPrecio;
        }

        public void ActivarCatalogo()
        {
            ActivoCatalogo = true;
        }

        public void DesactivarCatalogo()
        {
            ActivoCatalogo = false;
        }
    }
}
