using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.Services;
using PensionSystem.Domain.Entity;
using PensionSystem.Infrastructure.DBContext;

namespace PensionSystem.Infrastructure.Services
{
    public class MemberService : IMemberService
    {
        private readonly PensionSystemContext _context;
        public MemberService(PensionSystemContext context)
        {
            _context = context;
        }
        public async Task<bool> DeleteMember(string id)
        {
            if(string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException("id");

            var getMember = await GetMemberById(id);
            if(getMember == null) return false;
           
            //Soft Delete
            getMember.IsDeleleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Member>> GetAllMember()
        {
           return await _context.Members.ToListAsync();
        }

        public async Task<Member> GetMemberById(string id)
        {
            if(string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            return await _context.Members.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Member> GetMemberByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException("email");
            return await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
        }
        public async Task<Member> RegisterMember(Member member)
        {
            if(member == null) throw new ArgumentNullException(nameof(member));

            // Check for duplicate emails
            var existingMember = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == member.Email);
            if (existingMember != null)
                throw new InvalidOperationException("A member with this email already exists.");

            await _context.AddAsync(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<bool> UpdateMember(Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            var existingMember = await GetMemberById(member.Id);
            if (existingMember == null) return false; // Member not found

            // Update fields
            existingMember.FirstName = member.FirstName;
            existingMember.LastName = member.LastName;
            existingMember.Email = member.Email;
            existingMember.PhoneNumber = member.PhoneNumber;
            existingMember.DateOfBirth = member.DateOfBirth;

            await _context.SaveChangesAsync(); // Save changes
            return true; // 
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync(); // Persist changes
        }
    }
}
