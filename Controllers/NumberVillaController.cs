using AutoMapper;
using MagicVillaApi.DTOs;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NumberVillaController : ControllerBase
    {
        private readonly INumberVillaService _numberVillaService;
        private readonly IVillaService _villaService;
        private readonly ILogger<NumberVillaController> _logger;
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;

        public NumberVillaController( IVillaService villaService, INumberVillaService numberVillaService, IMapper mapper, ILogger<NumberVillaController> logger)
        {
            _villaService = villaService;
            _numberVillaService = numberVillaService;
            _mapper = mapper;
            _logger = logger;
            _apiResponse = new();
        }

        [Authorize]
        [HttpGet("getNumberVillas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetNumberVillas()
        {
            try
            {
                IEnumerable<NumberVilla> numberVillas = await _numberVillaService.GetEntities();
                IEnumerable<NumberVillaDTO> numberVillasDto = _mapper.Map<IEnumerable<NumberVillaDTO>>(numberVillas);
                _apiResponse.Response = numberVillasDto;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccessful = false;
                _apiResponse.ErrorMesgges = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
        }

        [Authorize]
        [HttpGet("getNumVilla/{numVilla}", Name = "GetNumbVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetNumbVilla([FromRoute] int numVilla)
        {
            try
            {
                if (numVilla <= 0)
                {
                    _logger.LogError("EL parametro de busqueda es obligatorio");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccessful = false;
                    return BadRequest(_apiResponse);
                }
                NumberVilla numberVilla = await _numberVillaService.GetEntity(v => v.NumVilla == numVilla);
                NumberVillaDTO numberVillaDTO = _mapper.Map<NumberVillaDTO>(numberVilla);
                if (numberVillaDTO == null)
                {
                    _logger.LogWarning("Villa no encontrada");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Response = HttpStatusCode.NotFound.ToString();
                    _apiResponse.IsSuccessful = false;
                    return NotFound(_apiResponse);
                }
                _logger.LogInformation("Villa encontrada con exito");
                _apiResponse.Response = numberVillaDTO;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccessful = false;
                _apiResponse.ErrorMesgges = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpPost("insertNumberVilla")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateNumerVilla([FromBody] NumberVillaDTO numberVillaDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                NumberVilla numberVilla = _mapper.Map<NumberVilla>(numberVillaDTO);
                Villa villa = await _villaService.GetEntity(v => v.Id == numberVilla.IdVilla);
                if (villa == null)
                {
                    _logger.LogInformation($"No exite una villa con el id {numberVilla.IdVilla}.");
                    _apiResponse.Response = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccessful = false;
                    return BadRequest(_apiResponse);
                }
                numberVilla.CreationDate = DateTime.Now;
                numberVilla.UpdateDate = DateTime.Now;
                numberVilla.DetailsVilla = $"Details Villa: {villa.Details} - {villa.Amenity}";
                await _numberVillaService.CreateEntity(numberVilla);
                _apiResponse.Response = numberVilla;
                _apiResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetNumbVilla", new { numVilla = numberVilla.NumVilla }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccessful = false;
                _apiResponse.ErrorMesgges = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("delete/{numVilla:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNumVilla([FromRoute] int numVilla)
        {
            try
            {
                if (numVilla <= 0)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                NumberVilla numberVilla = await _numberVillaService.GetEntity(v => v.NumVilla == numVilla, false);
                if (numberVilla == null)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                await _numberVillaService.DeleteEntity(numberVilla);
                _logger.LogInformation("NumVilla borrado con exito");
                _apiResponse.Response = NoContent();

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccessful = false;
                _apiResponse.ErrorMesgges = new List<string>() { ex.ToString() };
            }
            return BadRequest(_apiResponse);
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpPut("update/{NumVilla:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Update([FromRoute] int NumVilla, [FromBody] NumberVillaDTO numberVillaDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                NumberVilla numberVilla = _mapper.Map<NumberVilla>(numberVillaDTO);
                NumberVilla numberVillaRes = await _numberVillaService.UpdateVilla(numberVilla);
                NumberVillaDTO numberVillaDTORes = _mapper.Map<NumberVillaDTO>(numberVillaRes);
                _logger.LogInformation("NumVilla actualizada con exito");
                _apiResponse.Response = numberVillaDTORes;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccessful = false;
                _apiResponse.ErrorMesgges = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
        }
    }
}
