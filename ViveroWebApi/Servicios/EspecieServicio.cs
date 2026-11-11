using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.DTOs;
using ViveroWebApi.Excepciones;
using ViveroWebApi.Mappers;
using ViveroWebApi.Repositorios;

namespace ViveroWebApi.Servicios
{
    public interface IEspecieServicio
    {
        List<EspecieResponse> ObtenerTodas();
        EspecieResponse ObtenerPorCodigo(string codigo);
        EspecieResponse Crear(EspecieRequest request);
        EspecieResponse Actualizar(string codigo, EspecieRequest request);
        void Eliminar(string codigo);

        // Operaciones de los lotes (se entra por la especie)
        EspecieResponse AgregarLote(string codigo, LoteRequest request);
        EspecieResponse RegistrarBaja(string codigo, string idLote, BajaRequest request);
        EspecieResponse MarcarListo(string codigo, string idLote);
        EspecieResponse Trasplantar(string codigo, string idLote, TrasplanteRequest request);
        EspecieResponse AgregarLabor(string codigo, string idLote, LaborRequest request);
        EspecieResponse RegistrarRealizacionLabor(string codigo, string idLote, int idLabor);
    }

    public class EspecieServicio : IEspecieServicio
    {
        // Las especies se guardan en el repositorio de productos porque Especie es un Producto.
        // Los lotes no tienen repositorio, viven dentro de la especie.
        private readonly IProductoRepositorio _repositorio;
        private readonly IEspecieMapper _mapper;

        public EspecieServicio(IProductoRepositorio repositorio, IEspecieMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public List<EspecieResponse> ObtenerTodas()
        {
            return _repositorio.ObtenerTodos().OfType<Especie>().Select(_mapper.ARespuesta).ToList();
        }

        public EspecieResponse ObtenerPorCodigo(string codigo)
        {
            return _mapper.ARespuesta(BuscarEspecie(codigo));
        }

        public EspecieResponse Crear(EspecieRequest request)
        {
            if (_repositorio.ObtenerPorCodigo(request.Codigo) != null)
                throw new Dominio.Excepciones.DatoInvalidoException($"Ya existe un producto con código {request.Codigo}.");

            Especie especie = _mapper.ADominio(request);
            _repositorio.Guardar(especie);
            return _mapper.ARespuesta(especie);
        }

        public EspecieResponse Actualizar(string codigo, EspecieRequest request)
        {
            Especie especie = BuscarEspecie(codigo);
            especie.Nombre = request.Nombre;
            especie.NombreCientifico = request.NombreCientifico;
            especie.ActualizarPrecios(request.PrecioPequena, request.PrecioMediana, request.PrecioGrande);
            _repositorio.Guardar(especie);
            return _mapper.ARespuesta(especie);
        }

        public void Eliminar(string codigo)
        {
            BuscarEspecie(codigo);
            _repositorio.Eliminar(codigo);
        }

        public EspecieResponse AgregarLote(string codigo, LoteRequest request)
        {
            Especie especie = BuscarEspecie(codigo);
            especie.AgregarLote(_mapper.ADominio(request));
            return _mapper.ARespuesta(especie);
        }

        public EspecieResponse RegistrarBaja(string codigo, string idLote, BajaRequest request)
        {
            Especie especie = BuscarEspecie(codigo);
            BuscarLote(especie, idLote).RegistrarBaja(request.Cantidad);
            return _mapper.ARespuesta(especie);
        }

        public EspecieResponse MarcarListo(string codigo, string idLote)
        {
            Especie especie = BuscarEspecie(codigo);
            BuscarLote(especie, idLote).MarcarListo();
            return _mapper.ARespuesta(especie);
        }

        public EspecieResponse Trasplantar(string codigo, string idLote, TrasplanteRequest request)
        {
            Especie especie = BuscarEspecie(codigo);
            BuscarLote(especie, idLote).Trasplantar(request.NuevoTamanio);
            return _mapper.ARespuesta(especie);
        }

        public EspecieResponse AgregarLabor(string codigo, string idLote, LaborRequest request)
        {
            Especie especie = BuscarEspecie(codigo);
            BuscarLote(especie, idLote).AgregarLabor(request.Descripcion, request.PeriodicidadDias);
            return _mapper.ARespuesta(especie);
        }

        public EspecieResponse RegistrarRealizacionLabor(string codigo, string idLote, int idLabor)
        {
            Especie especie = BuscarEspecie(codigo);
            Lote lote = BuscarLote(especie, idLote);
            Labor? labor = lote.Labores.FirstOrDefault(l => l.IdLabor == idLabor);
            if (labor == null)
                throw new RecursoNoEncontradoException($"No existe la labor {idLabor} en el lote {idLote}.");
            labor.RegistrarRealizacion(DateTime.Today);
            return _mapper.ARespuesta(especie);
        }

        // Si no existe la especie lanza 404.
        private Especie BuscarEspecie(string codigo)
        {
            Especie? especie = _repositorio.ObtenerPorCodigo(codigo) as Especie;
            if (especie == null)
                throw new RecursoNoEncontradoException($"No existe una especie con código {codigo}.");
            return especie;
        }

        private Lote BuscarLote(Especie especie, string idLote)
        {
            Lote? lote = especie.ObtenerLote(idLote);
            if (lote == null)
                throw new RecursoNoEncontradoException($"No existe el lote {idLote} en la especie {especie.Codigo}.");
            return lote;
        }
    }
}
