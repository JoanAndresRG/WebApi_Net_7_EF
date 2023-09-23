using AutoMapper;
using MagicVillaApi.Models.Class;
using MagicVillaApi.Models.DTOs;
using MagicVillaApi.Services.Intefaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaController(ILogger<VillaController> logger, IVillaService villaService, IMapper mapper )
        {
            _villaService = villaService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// endpoint que retorna todos los regitros de villas
        /// </summary>
        /// <param name=""></param>
        /// <returns name="VillaDTO"></returns> 
        [HttpGet("getVillas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villas = await _villaService.GetVillas();
            IEnumerable<VillaDTO> villasDTO = _mapper.Map<IEnumerable<VillaDTO>>(villas);
            return Ok(villasDTO);
        }


        /// <summary>
        /// endpoint que retorna una villa segun su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns name="VillaDTO"></returns> 
        [HttpGet("getVilla/{id}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogError("EL parametro de busqueda es obligatorio");
                return BadRequest();
            }
            Villa villa = await _villaService.GetVilla(id);
            VillaDTO villaDTO = _mapper.Map<VillaDTO>(villa);
            if (villaDTO == null)
            {
                _logger.LogWarning("Villa no encontrada");
                return NotFound();
            }
            _logger.LogInformation("Villa encontrada con exito");
            return Ok(villaDTO);
        }


        /// <summary>
        /// endpoint que inserta un nuevo registro en la tabla
        /// </summary>
        /// <param name="id"></param>
        /// <returns name="VillaDTO"></returns>
        [HttpPost("insertVilla")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            Villa villa = _mapper.Map<Villa>(villaDTO); if (villa == null) return BadRequest(ModelState);
            villa.CreationDate = DateTime.Now;
            villa.UpdateDate = DateTime.Now;
            await _villaService.CreateVilla(villa);
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);
        }


        /// <summary>
        /// endpoint que borra un registro de la tabal
        /// </summary>
        /// <param name="id"></param>
        /// <returns name=""></returns>
        [HttpDelete("delete/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            await _villaService.DeleteVilla(id);
            _logger.LogInformation("Villa borrado con exito");
            return NoContent();
        }

        /// <summary>
        /// endpoint que actualiza los datos de un registro de la tabal
        /// </summary>
        /// <param name="VillaDTO"></param>
        /// <returns name="VillaDTO"></returns>
        [HttpPut("update/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> UpdateVilla([FromRoute]int id, [FromBody] VillaDTO villaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Villa villa = _mapper.Map<Villa>(villaDTO); 
            villa.Id = id;
            Villa villaRes = await _villaService.UpdateVilla(villa);
            VillaDTO villaDtoRes = _mapper.Map<VillaDTO>(villaRes);
            _logger.LogInformation("Villa actualizada con exito");
            return Ok(villaDtoRes);
        }


        /// <summary>
        /// endpoint que actualiza un atributo especifico de algún registro
        /// </summary>
        /// <param name="id"></param>
        /// <returns name=""></returns>
        [HttpPatch("updatePartialVilla/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialVilla([FromRoute]int id, JsonPatchDocument<VillaDTO> jsonPatchDto)
        {
            if (jsonPatchDto == null || id <= 0)
            {
                return BadRequest();
            }
            await _villaService.UpdatePartialVilla(id, jsonPatchDto);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}
