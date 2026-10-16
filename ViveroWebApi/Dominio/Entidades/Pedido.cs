namespace ViveroWebApi.Dominio.Entidades
{
    public class Pedido
    {
        public const decimal MontoMinimo = 50000m;

        public int Numero { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Creado";
        public decimal Total { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public Cliente Cliente { get; set; } = new Cliente();
        public List<LineaPedido> Lineas { get; set; } = new List<LineaPedido>();
        public LineaPedido AgregarLinea(Producto producto, int cantidad)
        {
            if (Estado != "Creado")
            {
                throw new InvalidOperationException("Solo se pueden agregar líneas a un pedido en estado Creado.");
            }
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor que cero.");
            }
            if (!producto.ActivoCatalogo)
            {
                throw new InvalidOperationException("El producto no está activo en el catálogo.");
            }
            LineaPedido linea = new LineaPedido();
            linea.IdLinea = Lineas.Count + 1;
            linea.Producto = producto;
            linea.Cantidad = cantidad;
            linea.PrecioUnitario = producto.Precio * (1 - Cliente.Descuento(producto));
            linea.CalcularSubtotal();

            Lineas.Add(linea);
            CalcularTotal();
            return linea;
        }

        public decimal CalcularTotal()
        {
            decimal total = 0;
            foreach (LineaPedido linea in Lineas)
            {
                total += linea.CalcularSubtotal();
            }
            Total = total;
            return Total;
        }

        public bool AlcanzarMinimo()
        {
            return CalcularTotal() >= MontoMinimo;
        }
        public bool Reservar()
        {
            if (Estado != "Creado") return false;
            if (!Cliente.EstaActivo()) return false;
            if (!Cliente.PuedeReservar()) return false;
            if (!AlcanzarMinimo()) return false;

            bool completo = true;
            foreach (LineaPedido linea in Lineas)
            {
                linea.Reservar();
                if (!linea.EstaCompleta())
                {
                    completo = false;
                }
            }

            if (!completo)
            {
                LiberarReserva();
                return false;
            }

            Estado = "Reservado";
            return true;
        }

        public void LiberarReserva()
        {
            foreach (LineaPedido linea in Lineas)
            {
                linea.LiberarReserva();
            }
            if (Estado == "Reservado")
            {
                Estado = "Creado";
            }
        }
        public bool DespacharPedido()
        {
            if (Estado != "Reservado") return false;

            foreach (LineaPedido linea in Lineas)
            {
                if (!linea.EstaCompleta()) return false;
            }

            Estado = "Despachado";
            return true;
        }
        public void Cancelar()
        {
            if (Estado == "Despachado")
            {
                throw new InvalidOperationException("No se puede cancelar un pedido ya despachado.");
            }

            LiberarReserva();
            Estado = "Cancelado";
        }
    }
}