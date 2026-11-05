namespace ViveroWebApi.Dominio.Excepciones
{
    public class DatoInvalidoException : Exception
    {
        public DatoInvalidoException(string mensaje) : base(mensaje)
        {
        }
    }
}