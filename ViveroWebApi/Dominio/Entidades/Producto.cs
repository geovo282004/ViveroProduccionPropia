namespace ViveroWebApi.Dominio.Entidades
{
    public abstract class Producto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public bool ActivoCatalogo { get; set; }

        // Abstracto: cada hijo dice cómo calcula su disponibilidad.
        public abstract int Disponibilidad();

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            Precio = nuevoPrecio;
        }

        public void ActivarCatalogo()
        {
            ActivoCatalogo = true;
        }

        public void DesactivarCatalogo()
        {
            ActivoCatalogo = false;
        }
    }
}
