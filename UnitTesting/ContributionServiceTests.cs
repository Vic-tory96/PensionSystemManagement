using Microsoft.EntityFrameworkCore;
using Moq;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.DBContext;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using PensionSystem.Domain.Entity;
using PensionSystem.Infrastructure.Services;

namespace UnitTesting
{
    public class ContributionServiceTests
    {
        private readonly PensionSystemContext _context;
        private readonly ContributionService _contributionService;

        public ContributionServiceTests()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<PensionSystemContext>()
                .UseInMemoryDatabase(databaseName: "PensionSystemTestDb")
                .Options;

            _context = new PensionSystemContext(options); // Use the in-memory context
            _contributionService = new ContributionService(_context); // Use the actual service with in-memory context
        }

        [Fact]
        public async Task AddMonthlyMandatoryContribution_ShouldThrowException_WhenContributionAlreadyExists()
        {
            // Arrange
            var member = new Member { Id = "1" };
            var existingContribution = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Monthly,
                Amount = 100,
                ReferenceNumber = "REF123"
            };

            // Add existing contribution to the in-memory database
            _context.Contributions.Add(existingContribution);
            await _context.SaveChangesAsync();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _contributionService.AddMonthlyMandatoryContribution(member, 200));

            Assert.Equal("Only one mandatory contribution allowed per month.", exception.Message);
        }

        [Fact]
        public async Task AddMonthlyMandatoryContribution_ShouldAddContribution_WhenNoExistingContribution()
        {
            // Arrange
            var member = new Member { Id = "1" };
            var amount = 200;

            // Act
            await _contributionService.AddMonthlyMandatoryContribution(member, amount);

            // Assert
            var contribution = await _context.Contributions
                .FirstOrDefaultAsync(c => c.MemberId == member.Id && c.Amount == amount);
            Assert.NotNull(contribution);
            Assert.Equal(member.Id, contribution.MemberId);
            Assert.Equal(amount, contribution.Amount);
        }

        [Fact]
        public async Task AddVoluntaryContribution_ShouldAddContribution()
        {
            // Arrange
            var member = new Member { Id = "1" };  // Assuming Member Id is a string
            var amount = 200m;  // Voluntary contribution amount

            // Act
            await _contributionService.AddVoluntaryContribution(member, amount);

            // Assert
            var contribution = await _context.Contributions
                .FirstOrDefaultAsync(c => c.MemberId == member.Id && c.Amount == amount && c.ContributionType == ContributionType.Voluntary);
            Assert.NotNull(contribution);  // Ensure the contribution was added
            Assert.Equal(member.Id, contribution.MemberId);
            Assert.Equal(amount, contribution.Amount);
            Assert.Equal(ContributionType.Voluntary, contribution.ContributionType);
        }

        [Fact]
        public async Task GetTotalContributionsByType_ShouldReturnCorrectSum()
        {
            // Arrange
            var member = new Member { Id = "1" };  // Assuming Member Id is a string
            var voluntaryContribution = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Voluntary,
                Amount = 200,
                ReferenceNumber = "VOL123"
            };
            var mandatoryContribution = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Monthly,
                Amount = 100,
                ReferenceNumber = "MAND123"
            };

            // Add contributions to the in-memory database
            _context.Contributions.Add(voluntaryContribution);
            _context.Contributions.Add(mandatoryContribution);
            await _context.SaveChangesAsync();

            // Act
            var totalVoluntaryContributions = await _contributionService.GetTotalContributionsByType(member, ContributionType.Voluntary);
            var totalMandatoryContributions = await _contributionService.GetTotalContributionsByType(member, ContributionType.Monthly);

            // Assert
            Assert.Equal(200, totalVoluntaryContributions);  // Sum of voluntary contributions
            Assert.Equal(100, totalMandatoryContributions);  // Sum of mandatory contributions
        }

        [Fact]
        public async Task GetTotalContributions_ShouldReturnCorrectSum()
        {
            // Arrange
            var member = new Member { Id = "1" };  // Assuming Member Id is a string
            var contribution1 = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Monthly,
                Amount = 200,
                ReferenceNumber = "REF123"
            };
            var contribution2 = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Voluntary,
                Amount = 150,
                ReferenceNumber = "REF124"
            };
            var contribution3 = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Monthly,
                Amount = 100,
                ReferenceNumber = "REF125"
            };

            // Add contributions to the in-memory database
            _context.Contributions.Add(contribution1);
            _context.Contributions.Add(contribution2);
            _context.Contributions.Add(contribution3);
            await _context.SaveChangesAsync();

            // Act
            var totalContributions = await _contributionService.GetTotalContributions(member);

            // Assert
            var expectedTotal = contribution1.Amount + contribution2.Amount + contribution3.Amount;
            Assert.Equal(expectedTotal, totalContributions);  // Ensure the total is correctly calculated
        }

        [Fact]
        public async Task GetTotalContributions_ShouldReturnZero_WhenNoContributions()
        {
            // Arrange
            var member = new Member { Id = "2" };  // Member with no contributions

            // Act
            var totalContributions = await _contributionService.GetTotalContributions(member);

            // Assert
            Assert.Equal(0, totalContributions);  // No contributions should result in a total of 0
        }

        [Fact]
        public async Task GenerateContributionStatement_ShouldReturnCorrectStatement()
        {
            // Arrange
            var member = new Member { Id = "1" };  // Assuming Member Id is a string
            var mandatoryContribution1 = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Monthly,
                Amount = 200,
                ReferenceNumber = "REF123"
            };
            var mandatoryContribution2 = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Monthly,
                Amount = 150,
                ReferenceNumber = "REF126"
            };
            var voluntaryContribution = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Voluntary,
                Amount = 100,
                ReferenceNumber = "REF124"
            };

            // Add contributions to the in-memory database
            _context.Contributions.Add(mandatoryContribution1);
            _context.Contributions.Add(mandatoryContribution2);
            _context.Contributions.Add(voluntaryContribution);
            await _context.SaveChangesAsync();

            // Act
            var statement = await _contributionService.GenerateContributionStatement(member);

            // Assert
            Assert.Equal(member.Id, statement.MemberId);  // Ensure the MemberId matches
            Assert.Equal(350, statement.TotalMandatoryContributions);  // Total mandatory contributions: 200 + 150
            Assert.Equal(100, statement.TotalVoluntaryContributions);  // Voluntary contributions: 100
            Assert.Equal(450, statement.TotalContributions);  // Total contributions: 350 + 100
        }

        [Fact]
        public async Task GenerateContributionStatement_ShouldReturnZeroForNoContributions()
        {
            // Arrange
            var member = new Member { Id = "2" };  // Member with no contributions

            // Act
            var statement = await _contributionService.GenerateContributionStatement(member);

            // Assert
            Assert.Equal(member.Id, statement.MemberId);  // Ensure the MemberId matches
            Assert.Equal(0, statement.TotalMandatoryContributions);  // No mandatory contributions
            Assert.Equal(0, statement.TotalVoluntaryContributions);  // No voluntary contributions
            Assert.Equal(0, statement.TotalContributions);  // Total contributions should be 0
        }
        [Fact]
        public async Task HasMetMinimumContributionPeriod_ShouldReturnTrue_WhenContributedForAtLeastOneYear()
        {
            // Arrange
            var member = new Member
            {
                Id = "1",
                Email = "test@example.com", // Adding required field
                FirstName = "John",         // Adding required field
                LastName = "Doe",           // Adding required field
                Contributions = new List<Contribution>()
            };

            // Create a contribution 2 years ago
            var contribution1 = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now.AddYears(-2),  // Contribution made 2 years ago
                ContributionType = ContributionType.Monthly,
                Amount = 200,
                ReferenceNumber = "REF123"
            };

            // Add member and contribution to the in-memory database
            _context.Members.Add(member);
            _context.Contributions.Add(contribution1);
            await _context.SaveChangesAsync(); // Save changes to the in-memory database

            // Act
            var result = await _contributionService.HasMetMinimumContributionPeriod(member);

            // Assert
            Assert.True(result);  // Member should have met the minimum contribution period (1 year)
        }
       
        [Fact]
        public async Task HasMetMinimumContributionPeriod_ShouldReturnFalse_WhenContributedForLessThanOneYear()
        {
            // Arrange
            var member = new Member { Id = "2" };
            var contribution1 = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now.AddMonths(-6),  // Contribution made 6 months ago
                ContributionType = ContributionType.Monthly,
                Amount = 150,
                ReferenceNumber = "REF124"
            };

            // Add contribution to the in-memory database
            _context.Contributions.Add(contribution1);
            await _context.SaveChangesAsync();

            // Act
            var result = await _contributionService.HasMetMinimumContributionPeriod(member);

            // Assert
            Assert.False(result);  // Member should not have met the minimum contribution period (1 year)
        }

        [Fact]
        public async Task HasMetMinimumContributionPeriod_ShouldReturnFalse_WhenNoContributionsMade()
        {
            // Arrange
            var member = new Member { Id = "3" };  // Member with no contributions

            // Act
            var result = await _contributionService.HasMetMinimumContributionPeriod(member);

            // Assert
            Assert.False(result);  // Member should not have met the minimum contribution period (1 year)
        }

        [Fact]
        public async Task CalculateBenefit_ShouldThrowException_WhenMemberHasNotMetMinimumContributionPeriod()
        {
            // Arrange
            var member = new Member
            {
                Id = "1",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Contributions = new List<Contribution>()
            };

            // Add the member to the context
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _contributionService.CalculateBenefit(member));

            Assert.Equal("Member is not eligible for benefits due to insufficient contribution period.", exception.Message);
        }

        [Fact]
        public async Task CalculateBenefit_ShouldReturnBenefit_WhenMemberHasMetMinimumContributionPeriod()
        {
            // Arrange
            var member = new Member
            {
                Id = "1",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Contributions = new List<Contribution>()
            };

            var contribution1 = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now.AddYears(-2), // Contribution made 2 years ago
                ContributionType = ContributionType.Monthly,
                Amount = 200,
                ReferenceNumber = "REF123"
            };

            // Add the member and contribution to the context
            _context.Members.Add(member);
            _context.Contributions.Add(contribution1);
            await _context.SaveChangesAsync();

            // Act
            var result = await _contributionService.CalculateBenefit(member);

            // Assert
            Assert.Equal(200m * 0.05m, result); // Expect 5% of total contributions (200 * 0.05 = 10)
        }

        [Fact]
        public async Task CalculateMonthlyInterest_ShouldReturnNull_WhenNoContributionsMadeThisMonth()
        {
            // Arrange
            var member = new Member
            {
                Id = "1",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Contributions = new List<Contribution>()
            };

            // Add member to the context
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // Act
            var result = await _contributionService.CalculateMonthlyInterest(member);

            // Assert
            Assert.Null(result);  // No contributions this month, should return null
        }

        [Fact]
        public async Task CalculateMonthlyInterest_ShouldCalculateInterest_WhenContributionsMadeThisMonth()
        {
            // Arrange
            var member = new Member
            {
                Id = "1",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Contributions = new List<Contribution>()
            };

            var contributionDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 5);  // Set a fixed date within the current month

            var contribution1 = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = contributionDate,  // Contribution within the current month
                ContributionType = ContributionType.Monthly,
                Amount = 200,
                ReferenceNumber = "REF123"
            };

            // Add member and contribution to the context
            _context.Members.Add(member);
            _context.Contributions.Add(contribution1);
            await _context.SaveChangesAsync();

            // Act
            var result = await _contributionService.CalculateMonthlyInterest(member);

            // Assert
            Assert.NotNull(result);  // Interest should be calculated
            Assert.Equal(contribution1.Amount * 0.01m, result);  // 1% of 200 should be 2 (200 * 0.01)
        }


       
    }
}



