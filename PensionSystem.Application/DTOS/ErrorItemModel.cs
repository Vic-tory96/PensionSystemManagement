using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.DTOS
{
    public class ErrorItemModel
    {
        public string Key { get; set; } 
        public List<string> ErrorMessages { get; set; } 
    }
}
