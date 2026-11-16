using Microsoft.AspNetCore.Mvc;
using ViveroWebApi.DTOs;
using ViveroWebApi.Servicios;

namespace ViveroWebApi.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteServicio _servicio;

        public ClientesController(IClienteServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ClienteResponse>), StatusCodes.Status200OK)]
        public ActionResult<List<ClienteResponse>> ObtenerTodos()
        {
            return Ok(_servicio.ObtenerTodos());
        }

        [HttpGet("{documento}")]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ClienteResponse> ObtenerPorDocumento(string documento)
        {
            return Ok(_servicio.ObtenerPorDocumento(documento));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ClienteResponse> Crear(ClienteRequest request)
        {
            ClienteResponse creado = _servicio.Crear(request);
            return CreatedAtAction(nameof(ObtenerPorDocumento), new { documento = creado.Documento }, creado);
        }

        [HttpPut("{documento}")]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ClienteResponse> Actualizar(string documento, ClienteRequest request)
        {
            return Ok(_servicio.Actualizar(documento, request));
        }

        [HttpDelete("{documento}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Eliminar(string documento)
        {
            _servicio.Eliminar(documento);
            return NoContent();
        }
    }
}
