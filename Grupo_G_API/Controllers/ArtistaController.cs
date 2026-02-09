using Microsoft.AspNetCore.Mvc;
using Grupo_G_API.Servicios;

namespace Grupo_G_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistaController : ControllerBase
    {
        private readonly IArtistaService _artistaService;

        public ArtistaController(IArtistaService artistaService)
        {
            _artistaService = artistaService;
        }

        /// <summary>Lista todos los artistas.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Models.Artista>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Models.Artista>>> Listar()
        {
            var lista = await _artistaService.ListarAsync();
            return Ok(lista);
        }

        /// <summary>Obtiene un artista por id (información al hacer clic en el cantante).</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Models.Artista), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Models.Artista>> ObtenerPorId(int id)
        {
            var artista = await _artistaService.ObtenerPorIdAsync(id);
            if (artista == null)
                return NotFound();
            return Ok(artista);
        }

        /// <summary>Lista las canciones del artista.</summary>
        [HttpGet("{id:int}/canciones")]
        [ProducesResponseType(typeof(List<Models.ArtistaCancionItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Models.ArtistaCancionItemDto>>> ListarCanciones(int id)
        {
            var lista = await _artistaService.ListarCancionesPorArtistaAsync(id);
            return Ok(lista);
        }
    }
}
