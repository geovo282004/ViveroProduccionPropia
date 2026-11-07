using ViveroWebApi.Dominio.Enumeraciones;

namespace ViveroWebApi.Dominio.Interfaces
{
    // Interfaz: un "contrato". Dice QUÉ debe saber hacer cada tipo de cliente, pero no CÓMO.
    // Cada clase que la implementa escribe su propia versión, y Cliente ya no necesita if por tipo.
    public interface ITipoCliente
    {
        string Nombre { get; }

        // El descuento depende de los DOS: del tipo de cliente (la clase que implementa)
        // y del tipo de producto (el parámetro).
        decimal Descuento(TipoProducto tipoProducto);

        bool PuedeReservar();
    }
}
