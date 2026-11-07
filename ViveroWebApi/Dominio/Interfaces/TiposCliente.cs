using ViveroWebApi.Dominio.Enumeraciones;

namespace ViveroWebApi.Dominio.Interfaces
{
    // Los tres tipos de cliente. Cada uno cumple el contrato ITipoCliente a su manera.
    // Cada uno guarda sus descuentos en un diccionario: tipo de producto -> porcentaje.
    // Así no hay ningún if, ni por tipo de cliente ni por tipo de producto.
    // Las herramientas nunca tienen descuento (documento del vivero).

    // Particular: compra lo disponible hoy, sin descuento y sin reservar.
    public class ClienteParticular : ITipoCliente
    {
        public string Nombre => "Particular";

        public decimal Descuento(TipoProducto tipoProducto)
        {
            return 0m;
        }

        public bool PuedeReservar() { return false; }
    }

    // Jardinero: 12 % en plantas y 8 % en insumos.
    public class ClienteJardinero : ITipoCliente
    {
        private readonly Dictionary<TipoProducto, decimal> _descuentos = new Dictionary<TipoProducto, decimal>
        {
            { TipoProducto.Planta, 0.12m },
            { TipoProducto.Insumo, 0.08m },
            { TipoProducto.Herramienta, 0m }
        };

        public string Nombre => "Jardinero";

        public decimal Descuento(TipoProducto tipoProducto)
        {
            return _descuentos[tipoProducto];
        }

        public bool PuedeReservar() { return true; }
    }

    // Empresa: 20 % en plantas y 15 % en insumos.
    public class ClienteEmpresa : ITipoCliente
    {
        private readonly Dictionary<TipoProducto, decimal> _descuentos = new Dictionary<TipoProducto, decimal>
        {
            { TipoProducto.Planta, 0.20m },
            { TipoProducto.Insumo, 0.15m },
            { TipoProducto.Herramienta, 0m }
        };

        public string Nombre => "Empresa";

        public decimal Descuento(TipoProducto tipoProducto)
        {
            return _descuentos[tipoProducto];
        }

        public bool PuedeReservar() { return true; }
    }
}
