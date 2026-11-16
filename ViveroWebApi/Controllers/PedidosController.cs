using Microsoft.AspNetCore.Mvc;
using ViveroWebApi.DTOs;
using ViveroWebApi.Servicios;

namespace ViveroWebApi.Controllers
{
    // El controlador es "el mesero": recibe la petición, se la pasa al servicio y devuelve la respuesta.
    // No tiene try/catch: los errores los atrapa el ManejadorGlobalExcepciones.
    [ApiController]
    [Route("api/pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoServicio _servicio;

        public PedidosController(IPedidoServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PedidoResponse>), StatusCodes.Status200OK)]
        public ActionResult<List<PedidoResponse>> ObtenerTodos()
        {
            return Ok(_servicio.ObtenerTodos());
        }

        [HttpGet("{numero:int}")]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PedidoResponse> ObtenerPorNumero(int numero)
        {
            return Ok(_servicio.ObtenerPorNumero(numero));
        }

        // POST api/pedidos -> crea un pedido vacío para un cliente
        [HttpPost]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PedidoResponse> Crear(PedidoRequest request)
        {
            PedidoResponse creado = _servicio.Crear(request);
            return CreatedAtAction(nameof(ObtenerPorNumero), new { numero = creado.Numero }, creado);
        }

        // POST api/pedidos/1/lineas -> agrega un producto al pedido
        [HttpPost("{numero:int}/lineas")]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PedidoResponse> AgregarLinea(int numero, LineaPedidoRequest request)
        {
            return Ok(_servicio.AgregarLinea(numero, request));
        }

        [HttpPost("{numero:int}/reservar")]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PedidoResponse> Reservar(int numero)
        {
            return Ok(_servicio.Reservar(numero));
        }

        [HttpPost("{numero:int}/despachar")]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PedidoResponse> Despachar(int numero)
        {
            return Ok(_servicio.Despachar(numero));
        }

        [HttpPost("{numero:int}/cancelar")]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PedidoResponse> Cancelar(int numero)
        {
            return Ok(_servicio.Cancelar(numero));
        }
    }
}
