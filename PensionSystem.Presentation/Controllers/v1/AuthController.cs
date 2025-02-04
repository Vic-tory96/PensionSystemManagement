using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PensionSystem.Application.DTOS;
using PensionSystem.Application.Helpers;
using PensionSystem.Application.Services;
using PensionSystem.Domain.Entity;
using PensionSystem.Infrastructure.DBContext;
using PensionSystem.Presentation.Configurations;

namespace PensionSystem.Presentation.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly PensionSystemContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMemberService _service;
        private readonly JwtSettings _jwtSettings;
        public AuthController(PensionSystemContext context, IConfiguration configuration, IOptions<JwtSettings> jwtSettings, IMemberService service)
        {
            _context = context;
            _configuration = configuration;
            _service = service;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterMemberDto registerDto)
        {
            var getEmail = await _service.GetMemberByEmail(registerDto.Email);
            if (getEmail != null)
            {
                var errorResponse = ResponseBuilder.BuildResponse(
                      status: 400,
                      statusText: "Bad Request",
                      errs: ModelState,
                      data: "Email already registered."
                );

                return BadRequest(errorResponse);
            }

            // Hash the password securely
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create a new member entity
            var member = new Member
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                DateOfBirth = registerDto.DateOfBirth,
                PasswordHash = hashedPassword,

            };

            await _service.RegisterMember(member);


          //  await _service.SaveChangesAsync();

            var successResponse = ResponseBuilder.BuildResponse(
                 status: 200,
                 statusText: "Success",
                 errs: null,
                 data: "Registration successful."
            );

            return Ok(successResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var member = await _service.GetMemberByEmail(request.Email);
            if (member == null || !BCrypt.Net.BCrypt.Verify(request.Password, member.PasswordHash))
            {
                var errorResponse = ResponseBuilder.BuildResponse(
                    status: 401,
                    statusText: "Unauthorized",
                    errs: null,
                    data: "Invalid credentials."
                );

                return Ok(errorResponse);
            }

            var token = GenerateJwtToken(member);

            var successResponse = ResponseBuilder.BuildResponse(
                 status: 200,
                 statusText: "Success",
                 errs: null,
                 data: new { Token = token }
            );

            return Ok(successResponse);
        }
        
        private string GenerateJwtToken(Member member)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
               new Claim(ClaimTypes.Email, member.Email),
               new Claim(ClaimTypes.Role, "Member") // default
            };
                
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    SigningCredentials = creds

            };
            var token =  tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token); // come back
            Console.WriteLine($"Generated Token: {jwtToken}"); // come back
            return jwtToken;

            //return tokenHandler.WriteToken(token);
        }

        //private string GenerateJwtToken(Member member)
        //{
        //    var claims = new[]
        //    {
        //    new Claim(ClaimTypes.NameIdentifier, member.Id),
        //    new Claim(ClaimTypes.Email, member.Email),
        //    new Claim(ClaimTypes.Role, "Member") // Default role
        //};

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(60),
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }


}

