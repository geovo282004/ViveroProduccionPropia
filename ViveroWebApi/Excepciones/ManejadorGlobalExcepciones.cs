using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ViveroWebApi.Dominio.Excepciones;

namespace ViveroWebApi.Excepciones
{
    // Manejador global: TODAS las excepciones de la API llegan aquí.
    // Por eso los controladores no necesitan try/catch (como lo explicó el profe).
    // Según el tipo de error, decide qué código HTTP devolver.
    public class ManejadorGlobalExcepciones : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception excepcion, CancellationToken cancelacion)
        {
            int codigo;
            string titulo;

            if (excepcion is RecursoNoEncontradoException)
            {
                codigo = StatusCodes.Status404NotFound;      // no existe
                titulo = "Recurso no encontrado";
            }
            else if (excepcion is DatoInvalidoException || excepcion is PedidoInvalidoException)
            {
                codigo = StatusCodes.Status400BadRequest;    // el usuario mandó algo que no se puede
                titulo = "Solicitud inválida";
            }
            else
            {
                codigo = StatusCodes.Status500InternalServerError;   // error inesperado
                titulo = "Error interno del servidor";
            }

            ProblemDetails respuesta = new ProblemDetails
            {
                Status = codigo,
                Title = titulo,
                Detail = excepcion.Message
            };

            contexto.Response.StatusCode = codigo;
            await contexto.Response.WriteAsJsonAsync(respuesta, cancelacion);
            return true;   // true = "ya manejé el error"
        }
    }
}
