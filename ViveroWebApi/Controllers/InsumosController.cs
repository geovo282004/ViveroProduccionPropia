using Microsoft.AspNetCore.Mvc;
using ViveroWebApi.DTOs;
using ViveroWebApi.Servicios;

namespace ViveroWebApi.Controllers
{
    // El controlador es "el mesero": recibe la petición, se la pasa al servicio y devuelve la respuesta.
    // No tiene try/catch: los errores los atrapa el ManejadorGlobalExcepciones.
    [ApiController]
    [Route("api/insumos")]
    public class InsumosController : ControllerBase
    {
        private readonly IInsumoServicio _servicio;

        public InsumosController(IInsumoServicio servicio)
        {
            _servicio = servicio;
        }

        // GET api/insumos -> lista todos
        [HttpGet]
        [ProducesResponseType(typeof(List<InsumoResponse>), StatusCodes.Status200OK)]
        public ActionResult<List<InsumoResponse>> ObtenerTodos()
        {
            return Ok(_servicio.ObtenerTodos());
        }

        // GET api/insumos/I-01 -> uno por código
        [HttpGet("{codigo}")]
        [ProducesResponseType(typeof(InsumoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<InsumoResponse> ObtenerPorCodigo(string codigo)
        {
            return Ok(_servicio.ObtenerPorCodigo(codigo));
        }

        // POST api/insumos -> crea uno nuevo
        [HttpPost]
        [ProducesResponseType(typeof(InsumoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<InsumoResponse> Crear(InsumoRequest request)
        {
            InsumoResponse creado = _servicio.Crear(request);
            return CreatedAtAction(nameof(ObtenerPorCodigo), new { codigo = creado.Codigo }, creado);
        }

        // PUT api/insumos/I-01 -> actualiza
        [HttpPut("{codigo}")]
        [ProducesResponseType(typeof(InsumoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<InsumoResponse> Actualizar(string codigo, InsumoRequest request)
        {
            return Ok(_servicio.Actualizar(codigo, request));
        }

        // DELETE api/insumos/I-01 -> elimina
        [HttpDelete("{codigo}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Eliminar(string codigo)
        {
            _servicio.Eliminar(codigo);
            return NoContent();
        }
    }
}
