using ViveroWebApi.Dominio.Enumeraciones;
using ViveroWebApi.Dominio.Excepciones;

namespace ViveroWebApi.Dominio.Entidades
{
    // La factura completa de un cliente. Tiene muchas líneas (composición).
    public class Pedido
    {
        public const decimal MontoMinimo = 50000m;

        private readonly List<LineaPedido> _lineas = new List<LineaPedido>();

        public Pedido(int numero, Cliente cliente, bool aCredito, bool aceptaDespachoParcial)
        {
            if (numero <= 0)
                throw new DatoInvalidoException("El número del pedido debe ser mayor que cero.");

            Numero = numero;
            Cliente = cliente;
            ACredito = aCredito;
            AceptaDespachoParcial = aceptaDespachoParcial;
            Fecha = DateTime.Now;
            Estado = EstadoPedido.Creado;
        }

        // Constructor vacío solo para Entity Framework (lo usa al leer de la base de datos).
        private Pedido()
        {
            Cliente = null!;
        }

        public int Numero { get; private set; }
        public DateTime Fecha { get; private set; }
        public EstadoPedido Estado { get; private set; }
        public string Observaciones { get; set; } = string.Empty;
        public Cliente Cliente { get; private set; }

        // Si es a crédito, se descuenta del cupo del cliente al reservar.
        public bool ACredito { get; private set; }

        // Si el cliente acepta recibir una parte ahora y el resto después.
        public bool AceptaDespachoParcial { get; private set; }

        // El total no se guarda: se calcula cada vez que se pide.
        public decimal Total
        {
            get { return CalcularTotal(); }
        }

        public IReadOnlyList<LineaPedido> Lineas
        {
            get { return _lineas; }
        }

        public LineaPedido AgregarLinea(Producto producto, int cantidad)
        {
            if (Estado != EstadoPedido.Creado)
                throw new PedidoInvalidoException("Solo se pueden agregar líneas a un pedido en estado Creado.");
            if (!producto.ActivoCatalogo)
                throw new PedidoInvalidoException("El producto no está activo en el catálogo.");

            // El descuento depende del tipo de cliente y del tipo de producto.
            decimal precioConDescuento = producto.Precio * (1 - Cliente.Descuento(producto));

            LineaPedido linea = new LineaPedido(_lineas.Count + 1, producto, cantidad, precioConDescuento);
            _lineas.Add(linea);
            return linea;
        }

        public decimal CalcularTotal()
        {
            decimal total = 0;
            foreach (LineaPedido linea in _lineas)
            {
                total += linea.Subtotal;
            }
            return total;
        }

        public bool AlcanzarMinimo()
        {
            return CalcularTotal() >= MontoMinimo;
        }

        // Se puede reservar estando Creado, o ParcialmenteDespachado (para apartar lo que faltó).
        public bool Reservar()
        {
            bool esPrimeraReserva = Estado == EstadoPedido.Creado;
            if (!esPrimeraReserva && Estado != EstadoPedido.ParcialmenteDespachado) return false;
            if (!Cliente.EstaActivo()) return false;
            if (!Cliente.PuedeReservar()) return false;
            if (!AlcanzarMinimo()) return false;
            if (esPrimeraReserva && ACredito && !Cliente.TieneCupoPara(Total)) return false;

            bool completo = true;
            bool algoApartado = false;
            foreach (LineaPedido linea in _lineas)
            {
                linea.Reservar();
                if (!linea.EstaCompleta()) completo = false;
                if (linea.CantidadComprometida > 0) algoApartado = true;
            }

            // Si no se pudo apartar todo y el cliente NO acepta despacho parcial, se deshace.
            // Si lo acepta, se queda con lo que se pudo apartar.
            bool sirveParcial = AceptaDespachoParcial && algoApartado;
            if (!completo && !sirveParcial)
            {
                LiberarReserva();
                return false;
            }

            if (esPrimeraReserva && ACredito)
            {
                Cliente.UsarCredito(Total);
            }

            Estado = EstadoPedido.Reservado;
            return true;
        }

        public void LiberarReserva()
        {
            foreach (LineaPedido linea in _lineas)
            {
                linea.LiberarReserva();
            }
        }

        // Entrega lo apartado. Si se entregó todo queda Despachado; si falta algo, ParcialmenteDespachado.
        public bool DespacharPedido()
        {
            if (Estado != EstadoPedido.Reservado) return false;

            foreach (LineaPedido linea in _lineas)
            {
                linea.Despachar();
            }

            bool todoEntregado = _lineas.All(l => l.EstaEntregada());
            Estado = todoEntregado ? EstadoPedido.Despachado : EstadoPedido.ParcialmenteDespachado;
            return true;
        }

        public void Cancelar()
        {
            if (Estado == EstadoPedido.Despachado)
                throw new PedidoInvalidoException("No se puede cancelar un pedido ya despachado.");
            if (Estado == EstadoPedido.Cancelado)
                throw new PedidoInvalidoException("El pedido ya está cancelado.");

            bool usabaCredito = ACredito && Estado != EstadoPedido.Creado;
            LiberarReserva();
            if (usabaCredito)
            {
                Cliente.LiberarCredito(Total);
            }
            Estado = EstadoPedido.Cancelado;
        }
    }
}
