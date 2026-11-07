namespace ViveroWebApi.Dominio.Enumeraciones
{
    // Tamaño de la matera, de menor a mayor. El ORDEN IMPORTA: C# guarda cada valor como
    // un número (Pequena = 0, Mediana = 1, Grande = 2), así que se pueden comparar
    // (Grande > Mediana). Eso se usa en el trasplante: solo se puede pasar a una matera mayor.
    public enum TamanioMatera
    {
        Pequena,
        Mediana,
        Grande
    }
}
