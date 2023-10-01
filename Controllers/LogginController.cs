using AutoMapper;
using MagicVillaApi.DTOs;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace MagicVillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogginController : ControllerBase
    {
        private readonly ILogginService _logginService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;
        public LogginController(ILogginService logginService, IConfiguration configuration, IMapper mapper)
        {
            _logginService = logginService;
            _configuration = configuration;
            _apiResponse = new();
            _mapper = mapper;
        }

        [HttpPost("loggin")]
        [AllowAnonymous]
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

        private async Task<User> AuthenticationUser(UserLoginDTO userLoginDTO)
        {
            return await _logginService.GetUserLoggin(userLoginDTO) ??
                throw new ArgumentException(null, nameof(userLoginDTO));
        }

        private async Task<User> GenerateTokenJWT(User user)
        {
            // Header
            var _symmetricSecurityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT:ClaveSecreta"])
                );
            var _signingCredentials = new SigningCredentials(
                    _symmetricSecurityKey, SecurityAlgorithms.HmacSha256
                );
            var _header = new JwtHeader(_signingCredentials);

            // Claims 
            var _claims = new[]
            {
                new Claim("UserName", user.UserName),
                new Claim("Email", user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRol) // Agregar el rol del usuario al token
            };

            // Payload

            var _payload = new JwtPayload(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: _claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(30)
            );

            // Token

            var _token = new JwtSecurityToken(
                    _header,
                    _payload
                );

            user.Token = new JwtSecurityTokenHandler().WriteToken(_token);

            return user;
        }
    }
}
