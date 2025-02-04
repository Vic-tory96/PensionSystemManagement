using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Domain.Entity;
using PensionSystem.Infrastructure.DBContext;
using PensionSystem.Infrastructure.Services;
using Xunit;

namespace PensionSystem.UnitTest
{
    public class MemberServiceTests
    {
        private readonly PensionSystemContext _context;
        private readonly MemberService _memberService;

        public MemberServiceTests()
        {
            // Use in-memory database for testing
            var options = new DbContextOptionsBuilder<PensionSystemContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PensionSystemContext(options);
            _memberService = new MemberService(_context);
        }

        [Fact]
        public async Task RegisterMember_ShouldAddMember_WhenValidMemberProvided()
        {
            // Arrange
            var member = new Member
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var result = await _memberService.RegisterMember(member);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);

            var savedMember = await _context.Members.FirstOrDefaultAsync(m => m.Id == "1");
            Assert.NotNull(savedMember); // Verify member was saved
        }

        [Fact]
        public async Task DeleteMember_ShouldSoftDeleteMember_WhenValidIdProvided()
        {
            // Arrange
            var member = new Member
            {
                Id = "1",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                IsDeleleted = false
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // Act
            var result = await _memberService.DeleteMember(member.Id);

            // Assert
            Assert.True(result);

            var deletedMember = await _context.Members.FirstOrDefaultAsync(m => m.Id == "1");
            Assert.True(deletedMember.IsDeleleted); // Verify soft delete
        }

        [Fact]
        public async Task GetMemberById_ShouldReturnMember_WhenIdIsValid()
        {
            // Arrange
            var member = new Member { Id = "1", FirstName = "Alice", LastName ="Smith", Email = "alice@example.com" };
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // Act
            var result = await _memberService.GetMemberById("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alice", result.FirstName);
        }

        [Fact]
        public async Task DeleteMember_ShouldThrowArgumentNullException_WhenIdIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _memberService.DeleteMember(null));
        }
    }
}
