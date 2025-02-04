

using PensionSystem.Domain.Entities;

namespace PensionSystem.Domain.Entity
{
    public class Member : BaseEntity
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public Employer Employer { get; set; }
        public string EmployerId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public bool IsEligibleForBenefit { get; set; }  // New field
        public bool IsDeleleted { get; set; } = false;
        public List<Contribution> Contributions { get; set; }
        public List<Benefit> Benefits { get; set; } = new List<Benefit>();

        public Member()
        {
            Contributions = new List<Contribution>();  // Initialize the collection to avoid null reference errors
            Benefits = new List<Benefit>();
        }



    }
}
