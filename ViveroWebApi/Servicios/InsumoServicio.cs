using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.DTOs;
using ViveroWebApi.Excepciones;
using ViveroWebApi.Mappers;
using ViveroWebApi.Repositorios;
using ViveroWebApi.Dominio.Excepciones;
using ViveroWebApi.Dominio.Memento;

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
        InsumoResponse Deshacer(string codigo); 
    }

    public class InsumoServicio : IInsumoServicio
    {
        // Dependemos de INTERFACES, no de clases concretas (inversión de dependencias).
        // Program.cs decide qué clase real se usa (inyección de dependencias).
        private readonly IProductoRepositorio _repositorio;
        private readonly IInsumoMapper _mapper;
        private readonly IHistorialInsumos _historial;   // el álbum de fotos (Memento)

        public InsumoServicio(IProductoRepositorio repositorio, IInsumoMapper mapper, IHistorialInsumos historial)
        {
            _repositorio = repositorio;
            _mapper = mapper;
            _historial = historial;
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
            // MEMENTO: antes de cambiar, se guarda la foto de cómo estaba.
            _historial.Guardar(codigo, insumo.CrearMemento());
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
                // MEMENTO: vuelve el insumo a como estaba antes del último cambio (como Ctrl + Z).
        public InsumoResponse Deshacer(string codigo)
        {
            Insumo insumo = BuscarInsumo(codigo);

            InsumoMemento? foto = _historial.SacarUltimo(codigo);
            if (foto == null)
                throw new DatoInvalidoException("No hay cambios para deshacer en este insumo.");

            insumo.Restaurar(foto);
            _repositorio.Guardar(insumo);   // guarda en la base de datos cómo quedó
            return _mapper.ARespuesta(insumo);
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
