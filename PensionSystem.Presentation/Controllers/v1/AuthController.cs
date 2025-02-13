using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using PensionSystem.Application.DTOS;
using PensionSystem.Presentation.Services;

namespace PensionSystem.Presentation.Controllers.v1
{

    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {

        private readonly AuthServices _authServices;

        public AuthController(AuthServices authServices)
        {

            _authServices = authServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterMemberDto registerDto)
        {
            var response = await _authServices.Register(registerDto);
            if (response.Status == 400)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDto>> Add(LoginRequest loginRequestModel)
        {
            var result = await _authServices.LogginUser(loginRequestModel);

            if (result.Success && !string.IsNullOrEmpty(result.Token))
            {
                Response.Headers.Add("Authorization", $"Bearer {result.Token}"); // Add token to response header
            }

            //This will add the token to response body
            return Ok(result);
        }

    }   

}

