using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PensionSystem.Domain.Entity;

namespace PensionSystem.Application.Services
{
    public interface IMemberService
    {
        Task<Member> RegisterMember(Member member);
        Task<Member> GetMemberById(string id);
        Task<Member> GetMemberByEmail(string email);
        Task<IEnumerable<Member>> GetAllMember();

        Task<bool> UpdateMember(Member member);
        Task<bool> DeleteMember(string id);
        Task SaveChangesAsync();
    }
}
