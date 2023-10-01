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
    public class UserController : ControllerBase
    {
        private readonly IUserApiService _userApiService;
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;
        public UserController(IUserApiService userApiService, IMapper mapper)
        {
            _userApiService = userApiService;
            _mapper = mapper;
            _apiResponse = new();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("CreateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateUser([FromBody] UserDtoCreate userDtoCreate)
        {
            try
            {
                User user = _mapper.Map<User>(userDtoCreate);
                user = await _userApiService.CreateUser(user);
                if (user == null)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Response = BadRequest();
                    return BadRequest(_apiResponse);
                }
                UserDTO userDtoRes = _mapper.Map<UserDTO>(user);
                _apiResponse.Response = userDtoRes;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetUser", new { id = user.Id }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccessful = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMesgges = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _apiResponse);
            }
        }


        [Authorize(Policy = "OrAdminOrUser")]
        [HttpGet("GetUser/{id:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetUser([FromRoute] int id)
        {
            try
            {

                User user = await _userApiService.GetUser(id);
                if (user == null)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Response = BadRequest();
                    return BadRequest(_apiResponse);
                }
                UserDTO userDTO = _mapper.Map<UserDTO>(user);
                _apiResponse.Response = userDTO;
                _apiResponse.StatusCode = HttpStatusCode.OK;
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

        [Authorize(Policy = "OrAdminOrUser")]
        [HttpGet("GetUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetUsers()
        {
            try
            {
                List<User> users = (List<User>)await _userApiService.GetUsers();
                if (users.Count < 1)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Response = BadRequest();
                    return BadRequest(_apiResponse);
                }
                List<UserDTO> userDTOs = _mapper.Map<List<UserDTO>>(users);
                _apiResponse.Response = userDTOs;
                _apiResponse.StatusCode = HttpStatusCode.OK;
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

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateUser([FromBody] UserDtoUpdate userDtoUpdate)
        {
            try
            {
                User user = _mapper.Map<User>(userDtoUpdate);
                user = await _userApiService.UpdateUser(user);
                if (user == null)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Response = BadRequest();
                    return BadRequest(_apiResponse);
                }
                UserDTO userDTO = _mapper.Map<UserDTO>(user);
                _apiResponse.Response = userDTO;
                _apiResponse.StatusCode = HttpStatusCode.OK;
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

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("DeleteUser/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            try
            {
                await _userApiService.DeleteUser(id);
                _apiResponse.IsSuccessful = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.Response = HttpStatusCode.NoContent;
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
