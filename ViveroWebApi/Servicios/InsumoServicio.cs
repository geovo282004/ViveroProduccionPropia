using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.DTOs;
using ViveroWebApi.Excepciones;
using ViveroWebApi.Mappers;
using ViveroWebApi.Repositorios;

namespace ViveroWebApi.Servicios
{
    // El servicio tiene la lógica: usa el repositorio para guardar/leer
    // y el mapper para convertir. El controlador solo lo llama.
    public interface IInsumoServicio
    {
        List<InsumoResponse> ObtenerTodos();
        InsumoResponse ObtenerPorCodigo(string codigo);
        InsumoResponse Crear(InsumoRequest request);
        InsumoResponse Actualizar(string codigo, InsumoRequest request);
        void Eliminar(string codigo);
    }

    public class InsumoServicio : IInsumoServicio
    {
        // Dependemos de INTERFACES, no de clases concretas (inversión de dependencias).
        // Program.cs decide qué clase real se usa (inyección de dependencias).
        private readonly IProductoRepositorio _repositorio;
        private readonly IInsumoMapper _mapper;

        public InsumoServicio(IProductoRepositorio repositorio, IInsumoMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public List<InsumoResponse> ObtenerTodos()
        {
            // OfType<Insumo>() se queda solo con los productos que son insumos.
            return _repositorio.ObtenerTodos().OfType<Insumo>().Select(_mapper.ARespuesta).ToList();
        }

        public InsumoResponse ObtenerPorCodigo(string codigo)
        {
            return _mapper.ARespuesta(BuscarInsumo(codigo));
        }

        public InsumoResponse Crear(InsumoRequest request)
        {
            if (_repositorio.ObtenerPorCodigo(request.Codigo) != null)
                throw new Dominio.Excepciones.DatoInvalidoException($"Ya existe un producto con código {request.Codigo}.");

            Insumo insumo = _mapper.ADominio(request);   // el constructor del dominio valida los datos
            _repositorio.Guardar(insumo);
            return _mapper.ARespuesta(insumo);
        }

        public InsumoResponse Actualizar(string codigo, InsumoRequest request)
        {
            Insumo insumo = BuscarInsumo(codigo);
            insumo.Nombre = request.Nombre;
            insumo.ActualizarPrecio(request.Precio);
            insumo.UnidadMedida = request.UnidadMedida;
            insumo.ActualizarStock(request.CantidadDisponible);
            insumo.CostoProveedor = request.CostoProveedor;
            insumo.FechaVencimiento = request.FechaVencimiento;
            _repositorio.Guardar(insumo);
            return _mapper.ARespuesta(insumo);
        }

        public void Eliminar(string codigo)
        {
            BuscarInsumo(codigo);   // si no existe, lanza 404
            _repositorio.Eliminar(codigo);
        }

        // Método privado de ayuda: busca el insumo o lanza RecursoNoEncontradoException (404).
        private Insumo BuscarInsumo(string codigo)
        {
            Insumo? insumo = _repositorio.ObtenerPorCodigo(codigo) as Insumo;
            if (insumo == null)
                throw new RecursoNoEncontradoException($"No existe un insumo con código {codigo}.");
            return insumo;
        }
    }
}
