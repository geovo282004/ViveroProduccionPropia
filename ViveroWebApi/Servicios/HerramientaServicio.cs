using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.DTOs;
using ViveroWebApi.Excepciones;
using ViveroWebApi.Mappers;
using ViveroWebApi.Repositorios;

namespace ViveroWebApi.Servicios
{
    public interface IHerramientaServicio
    {
        List<HerramientaResponse> ObtenerTodas();
        HerramientaResponse ObtenerPorCodigo(string codigo);
        HerramientaResponse Crear(HerramientaRequest request);
        HerramientaResponse Actualizar(string codigo, HerramientaRequest request);
        void Eliminar(string codigo);
    }

    public class HerramientaServicio : IHerramientaServicio
    {
        private readonly IProductoRepositorio _repositorio;
        private readonly IHerramientaMapper _mapper;

        public HerramientaServicio(IProductoRepositorio repositorio, IHerramientaMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public List<HerramientaResponse> ObtenerTodas()
        {
            return _repositorio.ObtenerTodos().OfType<Herramienta>().Select(_mapper.ARespuesta).ToList();
        }

        public HerramientaResponse ObtenerPorCodigo(string codigo)
        {
            return _mapper.ARespuesta(BuscarHerramienta(codigo));
        }

        public HerramientaResponse Crear(HerramientaRequest request)
        {
            if (_repositorio.ObtenerPorCodigo(request.Codigo) != null)
                throw new Dominio.Excepciones.DatoInvalidoException($"Ya existe un producto con código {request.Codigo}.");

            Herramienta herramienta = _mapper.ADominio(request);
            _repositorio.Guardar(herramienta);
            return _mapper.ARespuesta(herramienta);
        }

        public HerramientaResponse Actualizar(string codigo, HerramientaRequest request)
        {
            Herramienta herramienta = BuscarHerramienta(codigo);
            herramienta.Nombre = request.Nombre;
            herramienta.ActualizarPrecio(request.Precio);
            herramienta.ActualizarStock(request.CantidadDisponible);
            herramienta.Ubicacion = request.Ubicacion;
            herramienta.TieneGarantia = request.TieneGarantia;
            _repositorio.Guardar(herramienta);
            return _mapper.ARespuesta(herramienta);
        }

        public void Eliminar(string codigo)
        {
            BuscarHerramienta(codigo);
            _repositorio.Eliminar(codigo);
        }

        private Herramienta BuscarHerramienta(string codigo)
        {
            Herramienta? herramienta = _repositorio.ObtenerPorCodigo(codigo) as Herramienta;
            if (herramienta == null)
                throw new RecursoNoEncontradoException($"No existe una herramienta con código {codigo}.");
            return herramienta;
        }
    }
}
