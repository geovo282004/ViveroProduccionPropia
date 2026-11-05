namespace ViveroWebApi.Dominio.Entidades
{
    // Representa un producto y una cantidad dentro de un pedido.
    public class LineaPedido
    {
        // Identificador de esta línea dentro del pedido.
        public int IdLinea { get; set; }

        // Producto al que corresponde esta línea.
        // Se asigna antes de usar los métodos de la clase.
        public Producto Producto { get; set; } = null!;

        // Cantidad de unidades solicitadas por el cliente.
        public int Cantidad { get; set; }

        // Precio de cada unidad aplicado a este pedido.
        public decimal PrecioUnitario { get; set; }

        // Cantidad que esta línea logró reservar.
        // Solo se puede modificar desde esta clase.
        public int CantidadComprometida { get; private set; }

        public decimal Subtotal {get;set;} 
        
        // Calcula el subtotal multiplicando el precio por la cantidad solicitada.
        public decimal CalcularSubtotal()
        {
            return PrecioUnitario * Cantidad;
        }

        // Registra cuántas unidades están disponibles para esta línea.
        // Si no hay suficientes, registra las unidades disponibles.
        public void Reservar()
        {
            int disponible = Producto.Disponibilidad();

            if (disponible >= Cantidad)
            {
                CantidadComprometida = Cantidad;
            }
            else
            {
                CantidadComprometida = disponible;
            }
        }

        // Indica si se pudo reservar toda la cantidad solicitada.
        public bool EstaCompleta()
        {
            return CantidadComprometida == Cantidad;
        }

        // Libera la cantidad registrada como reservada en esta línea.
        public void LiberarReserva()
        {
            CantidadComprometida = 0;
        }
    }
}