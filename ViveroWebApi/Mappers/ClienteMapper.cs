using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.Dominio.Excepciones;
using ViveroWebApi.Dominio.Interfaces;
using ViveroWebApi.DTOs;

namespace ViveroWebApi.Mappers
{
    public interface IClienteMapper
    {
        Cliente ADominio(ClienteRequest request);
        ITipoCliente CrearTipo(string tipo);
        ClienteResponse ARespuesta(Cliente cliente);
    }

    public class ClienteMapper : IClienteMapper
    {
        // Diccionario: texto -> cómo crear ese tipo de cliente.
        // Así no usamos un if/switch por cada tipo. Para agregar un tipo nuevo, se agrega una línea.
        private readonly Dictionary<string, Func<ITipoCliente>> _tipos = new Dictionary<string, Func<ITipoCliente>>
        {
            { "Particular", () => new ClienteParticular() },
            { "Jardinero",  () => new ClienteJardinero() },
            { "Empresa",    () => new ClienteEmpresa() }
        };

        public Cliente ADominio(ClienteRequest request)
        {
            return new Cliente(request.Documento, request.Nombre, request.Correo,
                               request.Direccion, CrearTipo(request.TipoCliente), request.CupoCredito);
        }

        public ITipoCliente CrearTipo(string tipo)
        {
            if (!_tipos.ContainsKey(tipo))
                throw new DatoInvalidoException("El tipo de cliente debe ser Particular, Jardinero o Empresa.");
            return _tipos[tipo]();
        }

        public ClienteResponse ARespuesta(Cliente cliente)
        {
            return new ClienteResponse(cliente.Documento, cliente.Nombre, cliente.Correo,
                                       cliente.TipoCliente.Nombre, cliente.EstaActivo(), cliente.PuedeReservar(),
                                       cliente.CupoCredito, cliente.CupoDisponible);
        }
    }
}
