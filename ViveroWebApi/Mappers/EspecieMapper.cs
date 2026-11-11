using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.DTOs;

namespace ViveroWebApi.Mappers
{
    // Convierte entre DTO y dominio.
    public interface IEspecieMapper
    {
        Especie ADominio(EspecieRequest request);
        Lote ADominio(LoteRequest request);
        EspecieResponse ARespuesta(Especie especie);
    }

    public class EspecieMapper : IEspecieMapper
    {
        public Especie ADominio(EspecieRequest request)
        {
            return new Especie(request.Codigo, request.Nombre, request.NombreCientifico,
                               request.PrecioPequena, request.PrecioMediana, request.PrecioGrande);
        }

        public Lote ADominio(LoteRequest request)
        {
            return new Lote(request.IdLote, request.FechaSiembra, request.CantidadSembrada,
                            request.Ubicacion, request.TamanioMatera);
        }

        // Select recorre los lotes y convierte cada uno.
        // Se pasa la especie porque ella tiene los precios por matera.
        public EspecieResponse ARespuesta(Especie especie)
        {
            List<LoteResponse> lotes = especie.Lotes
                .Select(lote => ALoteRespuesta(especie, lote))
                .ToList();

            return new EspecieResponse(especie.Codigo, especie.Nombre, especie.NombreCientifico,
                                       especie.Disponibilidad(), especie.ActivoCatalogo, lotes);
        }

        private LoteResponse ALoteRespuesta(Especie especie, Lote lote)
        {
            List<LaborResponse> labores = lote.Labores
                .Select(ALaborRespuesta)
                .ToList();

            return new LoteResponse(lote.IdLote, lote.FechaSiembra, lote.CantidadSembrada,
                                    lote.CantidadViva, lote.CantidadComprometida,
                                    lote.CalcularDisponibilidad(), lote.CalcularMortalidad(),
                                    lote.Ubicacion, lote.TamanioMatera,
                                    especie.CalcularPrecio(lote.TamanioMatera),
                                    lote.Estado, labores);
        }

        private LaborResponse ALaborRespuesta(Labor labor)
        {
            return new LaborResponse(labor.IdLabor, labor.Descripcion, labor.PeriodicidadDias,
                                     labor.FechaUltimaRealizacion, labor.FechaProxima,
                                     labor.EstaVencida(), labor.EsCritica());
        }
    }
}
