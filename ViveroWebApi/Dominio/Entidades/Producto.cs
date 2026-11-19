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
        // Constructor vacío solo para Entity Framework (lo usa al leer de la base de datos).
        protected Producto()
        {
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

        // Polimorfismo: cada tipo de producto sabe cómo se aparta, se libera y se entrega su inventario.
        // Apartar = reservarlo para un pedido. Liberar = devolverlo si se cancela. Entregar = sale del vivero.
       // El tamaño de matera solo le importa a las plantas; los demás productos lo ignoran.
        public abstract void Apartar(int cantidad, TamanioMatera? tamanio);
        public abstract void Liberar(int cantidad, TamanioMatera? tamanio);
        public abstract void Entregar(int cantidad, TamanioMatera? tamanio);

        // Precio y disponibilidad según el tamaño de matera.
        // Por defecto el tamaño no importa (insumos y herramientas). La Especie los sobrescribe.
        public virtual decimal PrecioPara(TamanioMatera? tamanio)
        {
            return Precio;
        }

        public virtual int DisponibilidadPara(TamanioMatera? tamanio)
        {
            return Disponibilidad();
        }

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
