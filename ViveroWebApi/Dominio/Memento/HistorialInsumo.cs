namespace ViveroWebApi.Dominio.Memento
{
    // CUIDADOR: guarda las fotos de cada insumo en orden (como un álbum).
    // No sabe qué hay dentro de la foto: solo la guarda y la devuelve.
    public interface IHistorialInsumos
    {
        void Guardar(string codigo, InsumoMemento memento);
        InsumoMemento? SacarUltimo(string codigo);
    }

    public class HistorialInsumos : IHistorialInsumos
    {
        // Para cada código de insumo hay una pila (Stack) de fotos:
        // la última foto que entra es la primera que sale (como Ctrl + Z).
        private readonly Dictionary<string, Stack<InsumoMemento>> _historial =
            new Dictionary<string, Stack<InsumoMemento>>();

        public void Guardar(string codigo, InsumoMemento memento)
        {
            if (!_historial.ContainsKey(codigo))
                _historial[codigo] = new Stack<InsumoMemento>();

            _historial[codigo].Push(memento);   // Push = poner encima de la pila
        }

        // Devuelve la última foto, o null si no hay ninguna.
        public InsumoMemento? SacarUltimo(string codigo)
        {
            if (!_historial.ContainsKey(codigo) || _historial[codigo].Count == 0)
                return null;

            return _historial[codigo].Pop();    // Pop = sacar la de encima
        }
    }
}