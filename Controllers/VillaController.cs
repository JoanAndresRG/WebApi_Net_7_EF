﻿using AutoMapper;
using MagicVillaApi.DTOs;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;

        public VillaController(ILogger<VillaController> logger, IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService ?? throw new ArgumentNullException(nameof(villaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _apiResponse = new();
        }

        /// <summary>
        /// endpoint que retorna todos los regitros de villas
        /// </summary>
        /// <param name=""></param>
        /// <returns name="VillaDTO"></returns> 
        [AllowAnonymous]
        [HttpGet("GetVillas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                IEnumerable<Villa> villas = await _villaService.GetEntities( v => v.StateVilla == true );
                IEnumerable<VillaDTO> villasDTO = _mapper.Map<IEnumerable<VillaDTO>>(villas);
                _apiResponse.Response = villasDTO;
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


        /// <summary>
        /// endpoint que retorna una villa segun su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns name="VillaDTO"></returns> 
        [AllowAnonymous]
        [HttpGet("GetVilla/{nameVilla}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla([FromRoute] string nameVilla)
        {
            try
            {
                if (nameVilla == "")
                {
                    _logger.LogError("EL parametro de busqueda es obligatorio");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccessful = false;
                    return BadRequest(_apiResponse);
                }
                Villa villa = await _villaService.GetEntity(v => v.Name == nameVilla);
                VillaDTO villaDTO = _mapper.Map<VillaDTO>(villa);
                if (villaDTO == null)
                {
                    _logger.LogWarning("Villa no encontrada");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Response = HttpStatusCode.NotFound.ToString();
                    _apiResponse.IsSuccessful = false;
                    return NotFound(_apiResponse);
                }
                _logger.LogInformation("Villa encontrada con exito");
                _apiResponse.Response = villaDTO;
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


        /// <summary>
        /// endpoint que inserta un nuevo registro en la tabla
        /// </summary>
        /// <param name="id"></param>
        /// <returns name="VillaDTO"></returns>
        //[Authorize(Policy = "OrAdminOrUser")]
        [AllowAnonymous]
        [HttpPost("InsertVilla")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _apiResponse.Response = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccessful = false;
                    return BadRequest(_apiResponse);
                }
                Villa villa = _mapper.Map<Villa>(villaDTO);
                villa.CreationDate = DateTime.Now;
                villa.UpdateDate = DateTime.Now;
                var villaValid = await _villaService.GetEntity(v => v.Name.ToLower() == villa.Name.ToLower());
                if (villaValid != null)
                {
                    _apiResponse.Response = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.ErrorMesgges = new List<string>() { $"Nombre { villa.Name } no disponible" };
                    return BadRequest(_apiResponse);
                }
                await _villaService.CreateEntity(villa);
                _apiResponse.Response = villa;
                _apiResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { nameVilla = villa.Name }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccessful = false;
                _apiResponse.ErrorMesgges = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
        }


        /// <summary>
        /// endpoint que borra un registro de la tabal
        /// </summary>
        /// <param name="id"></param>
        /// <returns name=""></returns>
        //[Authorize(Policy = "AdminOnly")]
        [AllowAnonymous]
        [HttpPatch("DeleteVilla/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla([FromRoute] int id)
        {
            try
            {
                if (id <= 0)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                Villa villa = await _villaService.GetEntity(v => v.Id == id, false);
                if (villa == null)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                await _villaService.DeleteEntity(villa);
                _logger.LogInformation("Villa borrado con exito");
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


        [AllowAnonymous]
        [HttpGet("SearchVillasByPhrase/{phrase}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetListVillas([FromRoute] string phrase)
        {
            try
            {
                if (string.IsNullOrEmpty(phrase))
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Response = new List<VillaDTO>();
                    return BadRequest(_apiResponse);
                }
                List<Villa> listResponse = await _villaService.GetListEntities( v => v.Name.Contains(phrase) );
                if (listResponse == null || listResponse.Count == 0)
                {
                    _apiResponse.IsSuccessful = true;
                    _apiResponse.Response = new List<VillaDTO>();
                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(_apiResponse);
                }
                List<VillaDTO> villaDTOs = _mapper.Map<List<VillaDTO>>(listResponse);
                _logger.LogInformation("Villas encontrada con exito");
                _apiResponse.Response = villaDTOs;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);


            }
            catch ( Exception ex)
            {
                _apiResponse.IsSuccessful = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMesgges = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _apiResponse);
            }

        }

        /// <summary>
        /// endpoint que actualiza los datos de un registro de la tabal
        /// </summary>
        /// <param name="VillaDTO"></param>
        /// <returns name="VillaDTO"></returns>

        //[Authorize(Policy = "AdminOnly")]
        [AllowAnonymous]
        [HttpPut("UpdateVilla/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateVilla([FromRoute] string name, [FromBody] VillaDTO villaDTO)
        {
            try
            {
                if (String.IsNullOrEmpty(name))
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.ErrorMesgges = new List<string> { "Parametro name es obligatorio" };
                    return BadRequest(_apiResponse);
                }
                if (!ModelState.IsValid)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                Villa villa = _mapper.Map<Villa>(villaDTO);
                villa = await _villaService.UpdateVilla(name,villa);
                VillaDTO villaDtoRes = _mapper.Map<VillaDTO>(villa);
                _logger.LogInformation("Villa actualizada con exito");
                _apiResponse.Response = villaDtoRes;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccessful = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMesgges = new List<string>() { ex.Message.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _apiResponse);
            }
        }


        /// <summary>
        /// endpoint que actualiza un atributo especifico de algún registro
        /// </summary>
        /// <param name="id"></param>
        /// <returns name=""></returns>
        //[Authorize(Policy = "AdminOnly")]
        [AllowAnonymous]
        [HttpPatch("UpdatePartialVilla/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialVilla([FromRoute] string name, JsonPatchDocument<VillaDTO> jsonPatchDto)
        {
            try
            {
                if (jsonPatchDto == null || String.IsNullOrEmpty(name))
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                Villa villaUpdateResp = await _villaService.UpdatePartialVilla(name, jsonPatchDto);
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (villaUpdateResp == null) 
                {
                    _apiResponse.IsSuccessful = true;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return Ok(_apiResponse);
                }
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.Response = villaUpdateResp;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccessful = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMesgges = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _apiResponse);
            }
        }
    }
}
