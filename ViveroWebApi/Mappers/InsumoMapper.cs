using ViveroWebApi.Dominio.Entidades;
using ViveroWebApi.DTOs;

namespace ViveroWebApi.Mappers
{
    // Mapper = "el transformador". Convierte entre DTO y dominio en las dos direcciones.
    public interface IInsumoMapper
    {
        Insumo ADominio(InsumoRequest request);     // lo que llega  -> objeto del dominio
        InsumoResponse ARespuesta(Insumo insumo);   // objeto del dominio -> lo que se devuelve
    }

    public class InsumoMapper : IInsumoMapper
    {
        public Insumo ADominio(InsumoRequest request)
        {
            return new Insumo(request.Codigo, request.Nombre, request.Precio, request.CantidadDisponible,
                              request.UnidadMedida, request.CostoProveedor, request.FechaVencimiento);
        }

        // Solo devolvemos lo que necesita quien pregunta (no el costo del proveedor, por ejemplo).
        public InsumoResponse ARespuesta(Insumo insumo)
        {
            return new InsumoResponse(insumo.Codigo, insumo.Nombre, insumo.Precio,
                                      insumo.UnidadMedida, insumo.Disponibilidad(), insumo.Estado,
                                      insumo.FechaVencimiento, insumo.ActivoCatalogo);
        }
    }
}
