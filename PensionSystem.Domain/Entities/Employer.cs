
using System.ComponentModel.DataAnnotations;


namespace PensionSystem.Domain.Entity
{
    public class Employer : BaseEntity
    {
        
        public string CompanyName { get; set; }  

        public string RegistrationNumber { get; set; }  

        public bool ActiveStatus { get; set; }  

        public List<Member> Members { get; set; }

        public Employer()
        {
            Members = new List<Member>();
        }
    }
}
