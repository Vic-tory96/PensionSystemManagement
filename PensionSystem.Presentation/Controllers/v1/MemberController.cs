using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Application.DTOS;
using PensionSystem.Application.Helpers;
using PensionSystem.Application.Services;
using PensionSystem.Domain.Entity;

namespace PensionSystem.Presentation.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterMember([FromBody] RegisterMemberDto request)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Invalid", "Invalid Parameters");
                return BadRequest(ResponseBuilder.BuildResponse<object>(400, "BadRequest", ModelState, null));
            }

            //Check if member already exist
            var existingMember = await _memberService.GetMemberByEmail(request.Email);

            if (existingMember != null)
            {
                return Conflict(ResponseBuilder.BuildResponse<object>(409, "Conflict", null, "A member with this email already exists."));
            }

            var member = new Member
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _memberService.RegisterMember(member);

            var responseDto = new MemberResponseDto
            {
                FirstName = member.FirstName,
                LastName = member.LastName,
                DateOfBirth = member.DateOfBirth,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber
            };
            return CreatedAtAction(nameof(GetMemberById), new { id = result.Id }, ResponseBuilder.BuildResponse(201, "Created", null, responseDto));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateMember([FromBody] UpdateMemberDto request)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Invalid", "Invalid Parameters");
                return BadRequest(ResponseBuilder.BuildResponse<object>(400, "BadRequest", ModelState, null));
            }

            var member = new Member
            {
                Id = request.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _memberService.UpdateMember(member);
            if (!result)
            {
                ModelState.AddModelError("NotFound", "Member not found");
                return NotFound(ResponseBuilder.BuildResponse<object>(404, "Not Found", ModelState, null));
            }

            return Ok(ResponseBuilder.BuildResponse(200, "Success", null, "Member updated successfully"));

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(string id)
        {
            var member = await _memberService.GetMemberById(id);
            if (member == null)
            {
                ModelState.AddModelError("NotFound", "Member not found");
                return NotFound(ResponseBuilder.BuildResponse<object>(404, "Not Found", ModelState, null));
            }

            return Ok(ResponseBuilder.BuildResponse(200, "Success", null, member));
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetMemberByEmail(string email)
        {
            var member = await _memberService.GetMemberByEmail(email);
            if (member == null)
            {
                ModelState.AddModelError("NotFound", "Member not found");
                return NotFound(ResponseBuilder.BuildResponse<object>(404, "Not Found", ModelState, null));
            }

            return Ok(ResponseBuilder.BuildResponse(200, "Success", null, member));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMembers()
        {
            var members = await _memberService.GetAllMember();
            return Ok(ResponseBuilder.BuildResponse(200, "Success", null, members));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(string id)
        {
            var result = await _memberService.DeleteMember(id);
            if (!result)
            {
                ModelState.AddModelError("NotFound", "Member not found");
                return NotFound(ResponseBuilder.BuildResponse<object>(404, "Not Found", ModelState, null));
            }

            return Ok(ResponseBuilder.BuildResponse(200, "Success", null, "Member deleted successfully"));
        }
    }

}
