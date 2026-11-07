namespace ViveroWebApi.Excepciones
{
    // Se lanza cuando se busca algo que no existe (un insumo, un cliente, un pedido...).
    // El manejador global la convierte en una respuesta 404 (Not Found).
    public class RecursoNoEncontradoException : Exception
    {
        public RecursoNoEncontradoException(string mensaje) : base(mensaje)
        {
        }
    }
}
