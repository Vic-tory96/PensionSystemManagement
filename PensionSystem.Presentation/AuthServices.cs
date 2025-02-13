using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PensionSystem.Application.DTOS;
using PensionSystem.Application.Helpers;
using PensionSystem.Application.Services;
using PensionSystem.Domain.Entity;
using PensionSystem.Presentation.Configurations;

namespace PensionSystem.Presentation.Services
{
    public class AuthServices
    {
        private readonly IMemberService _memberService;
        private readonly IConfiguration _config;
        private readonly JwtSettings _jwtSettings;
        public AuthServices(IMemberService memeberService, IOptions<JwtSettings> jwtSettings, IConfiguration config)
        {
            _memberService = memeberService;
            _config = config;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<GlobalResponse<string>> Register(RegisterMemberDto registerDto)
        {
            var existingMember = await _memberService.GetMemberByEmail(registerDto.Email);
            if (existingMember != null)
            {
                return ResponseBuilder.BuildResponse(
                   status: 400,
                   statusText: "Bad Request",
                   errs: null,
                   data: "Email already registered."
                );
            }

            // Hash the password securely
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create new member entity
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

            await _memberService.RegisterMember(member);

            return ResponseBuilder.BuildResponse(
                status: 200,
                statusText: "Success",
                errs: null,
                data: "Registration successful."
            );
        }


        public async Task<LoginResponseDto> LogginUser(LoginRequest request)
        {
            var getUser = await _memberService.GetMemberByEmail(request.Email);
            if (getUser == null)
                return new LoginResponseDto(false, "User Not Found");

            bool checkPassword = BCrypt.Net.BCrypt.Verify(request.Password, getUser.PasswordHash);

            if (checkPassword)
                //If Loggin Success, create the Token 
                return new LoginResponseDto(true, "Login Successfully", GenerateJWT(getUser));
            else
                return new LoginResponseDto(false, "Invalid Password");
        }

        private string GenerateJWT (Member member)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                      new Claim(ClaimTypes.Email, member.Email)
            };
                var token = new JwtSecurityToken(
                  issuer: _jwtSettings.Issuer,
                  audience: _jwtSettings.Audience,
                  claims: userClaims,
                  expires: DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes),
                  signingCredentials: credentials
               );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
