using Microsoft.AspNetCore.Mvc;
using ViveroWebApi.DTOs;
using ViveroWebApi.Servicios;

namespace ViveroWebApi.Controllers
{
    [ApiController]
    [Route("api/especies")]
    public class EspeciesController : ControllerBase
    {
        private readonly IEspecieServicio _servicio;

        public EspeciesController(IEspecieServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<EspecieResponse>), StatusCodes.Status200OK)]
        public ActionResult<List<EspecieResponse>> ObtenerTodas()
        {
            return Ok(_servicio.ObtenerTodas());
        }

        [HttpGet("{codigo}")]
        [ProducesResponseType(typeof(EspecieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EspecieResponse> ObtenerPorCodigo(string codigo)
        {
            return Ok(_servicio.ObtenerPorCodigo(codigo));
        }

        [HttpPost]
        [ProducesResponseType(typeof(EspecieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<EspecieResponse> Crear(EspecieRequest request)
        {
            EspecieResponse creada = _servicio.Crear(request);
            return CreatedAtAction(nameof(ObtenerPorCodigo), new { codigo = creada.Codigo }, creada);
        }

        [HttpPut("{codigo}")]
        [ProducesResponseType(typeof(EspecieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EspecieResponse> Actualizar(string codigo, EspecieRequest request)
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

        // Rutas de los lotes

        [HttpPost("{codigo}/lotes")]
        [ProducesResponseType(typeof(EspecieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EspecieResponse> AgregarLote(string codigo, LoteRequest request)
        {
            EspecieResponse especie = _servicio.AgregarLote(codigo, request);
            return CreatedAtAction(nameof(ObtenerPorCodigo), new { codigo = especie.Codigo }, especie);
        }

        [HttpPost("{codigo}/lotes/{idLote}/bajas")]
        [ProducesResponseType(typeof(EspecieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EspecieResponse> RegistrarBaja(string codigo, string idLote, BajaRequest request)
        {
            return Ok(_servicio.RegistrarBaja(codigo, idLote, request));
        }

        [HttpPost("{codigo}/lotes/{idLote}/listo")]
        [ProducesResponseType(typeof(EspecieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EspecieResponse> MarcarListo(string codigo, string idLote)
        {
            return Ok(_servicio.MarcarListo(codigo, idLote));
        }

        [HttpPost("{codigo}/lotes/{idLote}/labores")]
        [ProducesResponseType(typeof(EspecieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EspecieResponse> AgregarLabor(string codigo, string idLote, LaborRequest request)
        {
            return Ok(_servicio.AgregarLabor(codigo, idLote, request));
        }

        [HttpPost("{codigo}/lotes/{idLote}/labores/{idLabor}/realizacion")]
        [ProducesResponseType(typeof(EspecieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EspecieResponse> RegistrarRealizacionLabor(string codigo, string idLote, int idLabor)
        {
            return Ok(_servicio.RegistrarRealizacionLabor(codigo, idLote, idLabor));
        }

        [HttpPost("{codigo}/lotes/{idLote}/trasplante")]
        [ProducesResponseType(typeof(EspecieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EspecieResponse> Trasplantar(string codigo, string idLote, TrasplanteRequest request)
        {
            return Ok(_servicio.Trasplantar(codigo, idLote, request));
        }
    }
}
