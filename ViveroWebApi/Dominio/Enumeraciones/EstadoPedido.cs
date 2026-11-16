namespace ViveroWebApi.Dominio.Enumeraciones
{
    // Estados posibles de un pedido (antes eran textos como "Reservado").
    public enum EstadoPedido
    {
        Creado,                  // se está armando
        Reservado,               // tiene unidades apartadas, listo para despachar
        ParcialmenteDespachado,  // se entregó una parte; el resto espera disponibilidad
        Despachado,              // se entregó todo
        Cancelado                // se anuló y se liberó lo apartado
    }
}
