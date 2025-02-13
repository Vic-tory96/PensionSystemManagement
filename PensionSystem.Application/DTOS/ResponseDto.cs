using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.DTOS
{
    public class ResponseDto
    {
        public int Status { get; set; }
        public string StatusText { get; set; }
        public object? Errs { get; set; }
        public object? Data { get; set; }

        public ResponseDto(int status, string statusText, object? errs = null, object? data = null)
        {
            Status = status;
            StatusText = statusText;
            Errs = errs;
            Data = data;
        }
    }

}
