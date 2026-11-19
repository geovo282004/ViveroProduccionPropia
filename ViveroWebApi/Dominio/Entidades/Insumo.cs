using ViveroWebApi.Dominio.Enumeraciones;
using ViveroWebApi.Dominio.Excepciones;
using ViveroWebApi.Dominio.Memento;

namespace ViveroWebApi.Dominio.Entidades
{
    public class Insumo : ProductoConStock
    {
private decimal _costoProveedor;

        // Constructor vacío solo para Entity Framework (lo usa al leer de la base de datos).
    private Insumo()
    {
    }

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
            // MEMENTO: el insumo se toma una foto de sí mismo.
    public InsumoMemento CrearMemento()
    {
        return new InsumoMemento(Nombre, Precio, CantidadDisponible,
            UnidadMedida, CostoProveedor, FechaVencimiento);
    }

    // MEMENTO: el insumo vuelve a como estaba en la foto.
    public void Restaurar(InsumoMemento memento)
    {
        Nombre = memento.Nombre;
        ActualizarPrecio(memento.Precio);
        ActualizarStock(memento.CantidadDisponible);
        UnidadMedida = memento.UnidadMedida;
        CostoProveedor = memento.CostoProveedor;
        FechaVencimiento = memento.FechaVencimiento;
    }
    }
}






    