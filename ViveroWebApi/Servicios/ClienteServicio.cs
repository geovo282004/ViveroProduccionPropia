using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.Dominio.Excepciones;
using ViveroWebApi.DTOs;
using ViveroWebApi.Excepciones;
using ViveroWebApi.Mappers;
using ViveroWebApi.Repositorios;

namespace ViveroWebApi.Servicios
{
    public interface IClienteServicio
    {
        List<ClienteResponse> ObtenerTodos();
        ClienteResponse ObtenerPorDocumento(string documento);
        ClienteResponse Crear(ClienteRequest request);
        ClienteResponse Actualizar(string documento, ClienteRequest request);
        void Eliminar(string documento);
    }

    public class ClienteServicio : IClienteServicio
    {
        private readonly IClienteRepositorio _repositorio;
        private readonly IClienteMapper _mapper;

        public ClienteServicio(IClienteRepositorio repositorio, IClienteMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public List<ClienteResponse> ObtenerTodos()
        {
            return _repositorio.ObtenerTodos().Select(_mapper.ARespuesta).ToList();
        }

        public ClienteResponse ObtenerPorDocumento(string documento)
        {
            return _mapper.ARespuesta(BuscarCliente(documento));
        }

        public ClienteResponse Crear(ClienteRequest request)
        {
            if (_repositorio.ObtenerPorDocumento(request.Documento) != null)
                throw new DatoInvalidoException($"Ya existe un cliente con documento {request.Documento}.");

            Cliente cliente = _mapper.ADominio(request);   // el constructor del dominio valida los datos
            _repositorio.Guardar(cliente);
            return _mapper.ARespuesta(cliente);
        }

        public ClienteResponse Actualizar(string documento, ClienteRequest request)
        {
            Cliente cliente = BuscarCliente(documento);
            cliente.Nombre = request.Nombre;
            cliente.Correo = request.Correo;
            cliente.Direccion = request.Direccion;
            cliente.TipoCliente = _mapper.CrearTipo(request.TipoCliente);
            cliente.CupoCredito = request.CupoCredito;
            _repositorio.Guardar(cliente);
            return _mapper.ARespuesta(cliente);
        }

        public void Eliminar(string documento)
        {
            BuscarCliente(documento);   // si no existe, lanza 404
            _repositorio.Eliminar(documento);
        }

        // Busca el cliente o lanza RecursoNoEncontradoException (404).
        private Cliente BuscarCliente(string documento)
        {
            Cliente? cliente = _repositorio.ObtenerPorDocumento(documento);
            if (cliente == null)
                throw new RecursoNoEncontradoException($"No existe un cliente con documento {documento}.");
            return cliente;
        }
    }
}
