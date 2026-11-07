using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.DTOs;

namespace ViveroWebApi.Mappers
{
    public interface IHerramientaMapper
    {
        Herramienta ADominio(HerramientaRequest request);
        HerramientaResponse ARespuesta(Herramienta herramienta);
    }

    public class HerramientaMapper : IHerramientaMapper
    {
        public Herramienta ADominio(HerramientaRequest request)
        {
            return new Herramienta(request.Codigo, request.Nombre, request.Precio, request.CantidadDisponible,
                                   request.Ubicacion, request.TieneGarantia);
        }

        public HerramientaResponse ARespuesta(Herramienta herramienta)
        {
            return new HerramientaResponse(herramienta.Codigo, herramienta.Nombre, herramienta.Precio,
                                           herramienta.TieneGarantia, herramienta.Disponibilidad(),
                                           herramienta.ActivoCatalogo);
        }
    }
}
