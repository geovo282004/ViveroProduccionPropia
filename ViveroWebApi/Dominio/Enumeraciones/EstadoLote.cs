namespace ViveroWebApi.Dominio.Enumeraciones
{
    // Estados posibles de un lote. Antes eran textos ("Listo"); con enum el compilador
    // avisa si alguien escribe un estado que no existe.
    public enum EstadoLote
    {
        EnCrecimiento,   // sembrado, aún no está listo para vender
        Listo,           // sus plantas vivas ya se pueden vender
        EnRevision,      // murió más del 30 %: no se vende hasta revisarlo
        Descartado       // se perdió por completo
    }
}
