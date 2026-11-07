using Microsoft.AspNetCore.Mvc;
using ViveroWebApi.DTOs;
using ViveroWebApi.Servicios;

namespace ViveroWebApi.Controllers
{
    [ApiController]
    [Route("api/herramientas")]
    public class HerramientasController : ControllerBase
    {
        private readonly IHerramientaServicio _servicio;

        public HerramientasController(IHerramientaServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<HerramientaResponse>), StatusCodes.Status200OK)]
        public ActionResult<List<HerramientaResponse>> ObtenerTodas()
        {
            return Ok(_servicio.ObtenerTodas());
        }

        [HttpGet("{codigo}")]
        [ProducesResponseType(typeof(HerramientaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<HerramientaResponse> ObtenerPorCodigo(string codigo)
        {
            return Ok(_servicio.ObtenerPorCodigo(codigo));
        }

        [HttpPost]
        [ProducesResponseType(typeof(HerramientaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<HerramientaResponse> Crear(HerramientaRequest request)
        {
            HerramientaResponse creada = _servicio.Crear(request);
            return CreatedAtAction(nameof(ObtenerPorCodigo), new { codigo = creada.Codigo }, creada);
        }

        [HttpPut("{codigo}")]
        [ProducesResponseType(typeof(HerramientaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<HerramientaResponse> Actualizar(string codigo, HerramientaRequest request)
        {
            return Ok(_servicio.Actualizar(codigo, request));
        }

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
