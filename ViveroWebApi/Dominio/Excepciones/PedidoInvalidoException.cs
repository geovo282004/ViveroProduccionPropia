namespace ViveroWebApi.Dominio.Excepciones
{
    // Alarma propia para las reglas del pedido: por ejemplo, cancelar un pedido
    // ya despachado o no tener cupo de crédito suficiente.
    public class PedidoInvalidoException : Exception
    {
        public PedidoInvalidoException(string mensaje) : base(mensaje)
        {
        }
    }
}