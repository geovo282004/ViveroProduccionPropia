namespace ViveroWebApi.Dominio.Enumeraciones
{
    // Estados de un lote.
    public enum EstadoLote
    {
        EnCrecimiento,   // todavía no se puede vender
        Listo,           // ya se puede vender
        EnRevision,      // murió más del 30 %
        Descartado       // se perdió el lote
    }
}
