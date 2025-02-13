using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.DTOS
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } // Nullable, as it's only set when login succeeds.

        public LoginResponseDto(bool success, string message, string token = null)
        {
            Success = success;
            Message = message;
            Token = token;
        }
    }

}
