using ViveroWebApi.Dominio.Excepciones;
using ViveroWebApi.Dominio.Interfaces;

namespace ViveroWebApi.Dominio.Entidades
{
    public class Cliente
    {
        private string _documento = string.Empty;
        private string _nombre = string.Empty;
        private string _correo = string.Empty;
        private decimal _cupoCredito;

        public Cliente(string documento, string nombre, string correo, string direccion,
            ITipoCliente tipoCliente, decimal cupoCredito)
        {
            Documento = documento;
            Nombre = nombre;
            Correo = correo;
            Direccion = direccion;
            TipoCliente = tipoCliente;
            CupoCredito = cupoCredito;
            CreditoUsado = 0;
            Activo = true;
        }

        public string Documento
        {
            get { return _documento; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DatoInvalidoException("El documento no puede estar vacío.");
                _documento = value;
            }
        }

        public string Nombre
        {
            get { return _nombre; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DatoInvalidoException("El nombre del cliente no puede estar vacío.");
                _nombre = value;
            }
        }

        public string Correo
        {
            get { return _correo; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
                    throw new DatoInvalidoException("El correo debe tener un @.");
                _correo = value;
            }
        }

        public string Direccion { get; set; } = string.Empty;

        // Un objeto que cumple la interfaz ITipoCliente (Particular, Jardinero o Empresa).
        public ITipoCliente TipoCliente { get; set; }

        public bool Activo { get; set; }

        // ---------- Cupo de crédito ----------
        // Cuánto le puede fiar el vivero en total.
        public decimal CupoCredito
        {
            get { return _cupoCredito; }
            set
            {
                if (value < 0)
                    throw new DatoInvalidoException("El cupo de crédito no puede ser negativo.");
                _cupoCredito = value;
            }
        }

        // Cuánto de ese cupo ya está usado en pedidos a crédito. Solo cambia con UsarCredito/LiberarCredito.
        public decimal CreditoUsado { get; private set; }

        // Lo que le queda: se calcula, no se guarda.
        public decimal CupoDisponible
        {
            get { return CupoCredito - CreditoUsado; }
        }

        public bool TieneCupoPara(decimal valor)
        {
            return valor <= CupoDisponible;
        }

        public void UsarCredito(decimal valor)
        {
            if (!TieneCupoPara(valor))
                throw new PedidoInvalidoException($"El cliente no tiene cupo de crédito suficiente. Disponible: {CupoDisponible}.");
            CreditoUsado += valor;
        }

        public void LiberarCredito(decimal valor)
        {
            CreditoUsado -= valor;
            if (CreditoUsado < 0) CreditoUsado = 0;
        }

        // ---------- Reglas que dependen del tipo de cliente ----------
        // El descuento depende del tipo de cliente Y del tipo de producto.
        // Ya no hay if: el tipo de cliente responde, y el producto dice qué tipo es (polimorfismo).
        public decimal Descuento(Producto producto)
        {
            return TipoCliente.Descuento(producto.Tipo);
        }

        public bool PuedeReservar()
        {
            return TipoCliente.PuedeReservar();
        }

        public bool EstaActivo()
        {
            return Activo;
        }
    }
}
