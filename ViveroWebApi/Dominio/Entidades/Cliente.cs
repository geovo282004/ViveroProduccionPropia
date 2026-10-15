namespace ViveroWebApi.Dominio.Entidades
{
    public class Cliente
    {
        public string Documento { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string TipoCliente { get; set; } = string.Empty;
        public bool activo { get; set; } = true;

        public decimal Descuento (Producto producto)
        {
            if (TipoCliente == "Particular")
            {
                return 0;

            } 
            if (TipoCliente == "Jardinero")
            {
                return 0.08m;
            }
            if (TipoCliente == "Empresa")
            {
                return 0.15m;
            }
            return 0;
        } 

        public bool PuedeReservar()
        {
            if (TipoCliente == "Jardinero" || TipoCliente == "Empresa")
            {
                return true;
            }
            return false;
        }
        public bool EstaActivo()
        {
            return activo;
        }

    }

}