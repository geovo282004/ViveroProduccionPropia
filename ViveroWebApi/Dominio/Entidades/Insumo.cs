using ViveroWebApi.Dominio.Enumeraciones;
using ViveroWebApi.Dominio.Excepciones;

namespace ViveroWebApi.Dominio.Entidades
{
    public class Insumo : ProductoConStock
    {
private decimal _costoProveedor;

    public Insumo(string codigo, string nombre, decimal precio, int cantidadDisponible,
            string unidadMedida, decimal costoProveedor, DateTime? fechaVencimiento)
        : base(codigo, nombre, precio, cantidadDisponible)
        {
            UnidadMedida = unidadMedida;
            CostoProveedor = costoProveedor;
            FechaVencimiento = fechaVencimiento;    
        }

    public  override TipoProducto Tipo
    {
        get { return TipoProducto.Insumo; }
    }

    public string UnidadMedida { get;set; } = string.Empty;

    public decimal CostoProveedor
    {
        get { return _costoProveedor; }
        set
        {
            if (value < 0)
                throw new DatoInvalidoException("El costo del proveedor no puede ser negativo.");
            _costoProveedor = value;
        }
    }
    public DateTime? FechaVencimiento { get;  set; }
    public bool EstaVencido()
    {
        if (FechaVencimiento == null)
            return false;
        return DateTime.Today > FechaVencimiento.Value;
    }

    public EstadoInsumo Estado
    {
        get
        {
            if (EstaVencido())
                return EstadoInsumo.Vencido;
            if (CantidadDisponible == 0)
                return EstadoInsumo.Agotado;
            return EstadoInsumo.Disponible;
        }
    }
        public override int Disponibilidad()
        {
        if (EstaVencido())
                return 0;
            return base.Disponibilidad();
        }
    }
}






    