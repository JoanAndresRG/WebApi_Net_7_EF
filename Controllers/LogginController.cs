using AutoMapper;
using MagicVillaApi.DTOs;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogginController : ControllerBase
    {
        private readonly ILogginService _logginService;
        private readonly IMapper _mapper;
        private readonly IGenerateTokenJWT _generateTokenJWT;
        protected APIResponse _apiResponse;
        public LogginController(ILogginService logginService, IMapper mapper, IGenerateTokenJWT generateTokenJWT)
        {
            _logginService = logginService;
            _apiResponse = new();
            _mapper = mapper;
            _generateTokenJWT = generateTokenJWT;
        }

        [AllowAnonymous]
        [HttpPost("LogginUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> LogginUser([FromBody] UserLoginDTO userLoginDTO)
        {
            try
            {
                User user = await AuthenticationUser(userLoginDTO);
                if (user == null)
                {
                    _apiResponse.IsSuccessful = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Response = BadRequest();
                    return BadRequest(_apiResponse);
                }
                user = await GenerateTokenJWT(user);

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
        [HttpPut("UpdateCredentials")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateCredentials( [FromBody] UserLogginDTOUpdate userLogginDTOUpdate )
        {
            try
            {
                User user = await _logginService.UpdateCredentials(userLogginDTOUpdate);
                UserDTO userDtoRes = _mapper.Map<UserDTO>(user);
                _apiResponse.Response = userDtoRes;
                _apiResponse.StatusCode = HttpStatusCode.OK;
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

        private async Task<User> AuthenticationUser(UserLoginDTO userLoginDTO)
        {
            return await _logginService.GetUserLoggin(userLoginDTO) ??
                throw new ArgumentException(null, nameof(userLoginDTO));
        }

        private async Task<User> GenerateTokenJWT(User user)
        {
            return await _generateTokenJWT.GenerateToken(user) ??
                throw new ArgumentException(null, nameof(user));
        }
    }
}
