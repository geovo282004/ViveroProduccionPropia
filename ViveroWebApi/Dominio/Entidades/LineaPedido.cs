using ViveroWebApi.Dominio.Excepciones;

namespace ViveroWebApi.Dominio.Entidades
{
    // Un renglón del pedido: un producto, cuántas unidades y a qué precio.
    public class LineaPedido
    {
        private int _cantidad;
        private decimal _precioUnitario;

        public LineaPedido(int idLinea, Producto producto, int cantidad, decimal precioUnitario)
        {
            IdLinea = idLinea;
            Producto = producto;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            CantidadComprometida = 0;
            CantidadDespachada = 0;
        }

        public int IdLinea { get; private set; }

        // Asociación: la línea TIENE un producto, pero no ES un producto.
        public Producto Producto { get; private set; }

        public int Cantidad
        {
            get { return _cantidad; }
            private set
            {
                if (value <= 0)
                    throw new DatoInvalidoException("La cantidad debe ser mayor que cero.");
                _cantidad = value;
            }
        }

        public decimal PrecioUnitario
        {
            get { return _precioUnitario; }
            private set
            {
                if (value < 0)
                    throw new DatoInvalidoException("El precio unitario no puede ser negativo.");
                _precioUnitario = value;
            }
        }

        // Lo apartado que todavía no se ha entregado.
        public int CantidadComprometida { get; private set; }

        // Lo que ya se le entregó al cliente (para el despacho parcial).
        public int CantidadDespachada { get; private set; }

        // Lo que falta por entregar: se calcula, no se guarda.
        public int CantidadPendiente
        {
            get { return Cantidad - CantidadDespachada; }
        }

        // El subtotal no se guarda: se calcula cada vez que se pide
        // (el profe pidió no guardar valores derivados).
        public decimal Subtotal
        {
            get { return CalcularSubtotal(); }
        }

        public decimal CalcularSubtotal()
        {
            return PrecioUnitario * Cantidad;
        }

        // Aparta lo que falta por entregar o, si no alcanza, lo que haya disponible.
        public void Reservar()
        {
            int disponible = Producto.Disponibilidad();
            CantidadComprometida = Math.Min(disponible, CantidadPendiente);
        }

        // Todo lo que falta por entregar quedó apartado.
        public bool EstaCompleta()
        {
            return CantidadComprometida == CantidadPendiente;
        }

        // Ya se entregó todo.
        public bool EstaEntregada()
        {
            return CantidadPendiente == 0;
        }

        // Entrega lo apartado.
        public void Despachar()
        {
            CantidadDespachada += CantidadComprometida;
            CantidadComprometida = 0;
        }

        public void LiberarReserva()
        {
            CantidadComprometida = 0;
        }
    }
}
