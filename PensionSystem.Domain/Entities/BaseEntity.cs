using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Domain.Entity
{
    public class BaseEntity
    {
        
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public DateTime AddedDate { get; set; } = DateTime.Now;
           
           
    }
}
