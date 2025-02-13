using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PensionSystem.Presentation.Configurations
{
    public class JwtAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtAuthenticationService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public void ConfigureJwtAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,  // Optionally validate token expiration
                        ClockSkew = TimeSpan.Zero, // Optional: eliminate clock skew
                        ValidIssuer = _jwtSettings.Issuer,
                        ValidAudience = _jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret))
                     
                    };
                });
        }
    }

}
