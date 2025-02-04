
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.DTOS;
using PensionSystem.Application.Helpers;
using PensionSystem.Application.Services;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.DBContext;

namespace PensionSystem.Presentation.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ContributionsController : ControllerBase
    {
        private readonly IContributionServices _contributionService;
        private readonly IMemberService _memberService;
        public ContributionsController(IContributionServices contributionService, IMemberService memberService)
        {
            _contributionService = contributionService;

            _memberService = memberService;
        }

        [HttpPost("mandatory")]
        public async Task<IActionResult> AddMonthlyMandatoryContribution([FromBody] AddContributionRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Invalid", "Invalid Parameters");
                return BadRequest(ResponseBuilder.BuildResponse<object>(400, "BadRequest", ModelState, null));
            }
            var member = await _memberService.GetMemberByEmail(request.Email);

            if (member == null)
            {
                return NotFound(ResponseBuilder.BuildResponse<object>(404, "Member not found", null, null));
            }

            await _contributionService.AddMonthlyMandatoryContribution(member, request.Amount);

            return CreatedAtAction(nameof(AddMonthlyMandatoryContribution), new { request.Email }, request);
        }

        [HttpPost("voluntary")]
        public async Task<IActionResult> AddVoluntaryContribution([FromBody] AddContributionRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Invalid", "Invalid Parameters");
                return BadRequest(ResponseBuilder.BuildResponse<object>(400, "BadRequest", ModelState, null));
            }

            var member = await _memberService.GetMemberByEmail(request.Email);
            if (member == null)
            {
                return NotFound(ResponseBuilder.BuildResponse<object>(404, "Member not found", null, null));
            }

            await _contributionService.AddVoluntaryContribution(member, request.Amount);

            return CreatedAtAction(nameof(AddVoluntaryContribution), new { request.Email }, request);
        }

        [HttpGet("statement/{email}")]
        public async Task<IActionResult> GetContributionStatement(string email)
        {
            var member = await _memberService.GetMemberByEmail(email);
            if (member == null)
            {
                return NotFound(ResponseBuilder.BuildResponse<object>(404, "Member not found", null, null));
            }

            var statement = await _contributionService.GenerateContributionStatement(member);

            var response = new ContributionStatementResponseDto
            {
                TotalMandatoryContributions = statement.TotalMandatoryContributions,
                TotalVoluntaryContributions = statement.TotalVoluntaryContributions,
                TotalContributions = statement.TotalContributions
            };

            return Ok(ResponseBuilder.BuildResponse(200, "Success", null, response));
        }

        [HttpPost("benefit/{email}")]
        public async Task<IActionResult> CalculateBenefit(string email)
        {
            var member = await _memberService.GetMemberByEmail(email);
            if (member == null)
            {
                return NotFound(ResponseBuilder.BuildResponse<object>(404, "Member not found", null, null));
            }

            var benefit = await _contributionService.CalculateBenefit(member);

            var response = new BenefitResponseDto
            {
                BenefitAmount = benefit
            };

            return Ok(ResponseBuilder.BuildResponse(200, "Benefit calculated successfully", null, response));
        }

        [HttpGet("total-contributions/{memberId}")]
        public async Task<IActionResult> GetTotalContributions(string memberId)
        {
            if (string.IsNullOrWhiteSpace(memberId))
            {
                ModelState.AddModelError("MemberId", "Member ID is required.");
                return BadRequest(ResponseBuilder.BuildResponse(400, "Bad Request", ModelState, (decimal?)null));
            }

            var member = await _memberService.GetMemberById(memberId);
            if (member == null)
            {
                ModelState.AddModelError("MemberId", "Member not found.");
                return NotFound(ResponseBuilder.BuildResponse(404, "Member Not Found", ModelState, (decimal?)null));
            }
            var totalContributions = await _contributionService.GetTotalContributions(member);
            return Ok(ResponseBuilder.BuildResponse(200, "Success", null, totalContributions));
        }

        [HttpGet("total-contributions/{memberId}/{contributionType}")]
        public async Task<IActionResult> GetTotalContributionsByType(string memberId, ContributionType contributionType)
        {
            if (string.IsNullOrWhiteSpace(memberId))
            {
                ModelState.AddModelError("MemberId", "Member ID is required.");
                return BadRequest(ResponseBuilder.BuildResponse(400, "Bad Request", ModelState, (decimal?)null));
            }

            var member = await _memberService.GetMemberById(memberId);
            if (member == null)
            {
                ModelState.AddModelError("MemberId", "Member not found.");
                return NotFound(ResponseBuilder.BuildResponse(404, "Member Not Found", ModelState, (decimal?)null));
            }
            var totalContributions = await _contributionService.GetTotalContributionsByType(member, contributionType);

            return Ok(ResponseBuilder.BuildResponse(200, "Success", null, totalContributions));
        }

        [HttpGet("total-contributions-for-month/{memberId}")]
        public async Task<IActionResult> GetTotalContributionsForMonth(string memberId)
        {
            if (string.IsNullOrWhiteSpace(memberId))
            {
                ModelState.AddModelError("MemberId", "Member ID is required.");
                return BadRequest(ResponseBuilder.BuildResponse(400, "Bad Request", ModelState, (decimal?)null));
            }
            var member = await _memberService.GetMemberById(memberId);
            if (member == null)
            {
                ModelState.AddModelError("MemberId", "Member not found.");
                return NotFound(ResponseBuilder.BuildResponse(404, "Member Not Found", ModelState, (decimal?)null));
            }
            var totalContributions = await _contributionService.GetTotalContributionsForMonth(member);

            return Ok(ResponseBuilder.BuildResponse(200, "Success", null, totalContributions));
        }

        [HttpPost("calculate-monthly-interest/{memberId}")]
        public async Task<IActionResult> CalculateMonthlyInterest(string memberId)
        {
            if (string.IsNullOrWhiteSpace(memberId))
            {
                ModelState.AddModelError("MemberId", "Member ID is required.");
                return BadRequest(ResponseBuilder.BuildResponse(400, "Bad Request", ModelState, (decimal?)null));
            }
            var getMember = await _memberService.GetMemberById(memberId);
            if (getMember == null)
            {
                ModelState.AddModelError("MemberId", "Member not found.");
                return NotFound(ResponseBuilder.BuildResponse(404, "Member Not Found", ModelState, (decimal?)null));
            }
            var interest = _contributionService.CalculateMonthlyInterest(getMember);
            if (interest == null)
            {
                // If no interest is calculated (e.g., no contributions), return a response with a message
                return Ok(ResponseBuilder.BuildResponse(200, "No contributions this month or no interest calculated", ModelState, (decimal?)null));
            }

            return Ok(ResponseBuilder.BuildResponse(200, "Interest Calculated Successfully", null, interest));
        }
    }
}
