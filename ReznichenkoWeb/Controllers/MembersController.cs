using Microsoft.AspNetCore.Mvc;
using FluentValidation;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberRepository _memberRepository;
    private readonly IValidator<CreateMemberDto> _createMemberValidator;
    private readonly IValidator<UpdateMemberDto> _updateMemberValidator;

    public MembersController(
        IMemberRepository memberRepository,
        IValidator<CreateMemberDto> createMemberValidator,
        IValidator<UpdateMemberDto> updateMemberValidator)
    {
        _memberRepository = memberRepository;
        _createMemberValidator = createMemberValidator;
        _updateMemberValidator = updateMemberValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllMembers()
    {
        var members = await _memberRepository.GetAllAsync();
        var memberDtos = members.Select(m => new MemberDto
        {
            Id = m.Id,
            Name = m.Name,
            Email = m.Email,
            Phone = m.Phone,
            JoinDate = m.JoinDate,
            MembershipType = m.MembershipType,
            IsActive = m.IsActive
        });
        return Ok(memberDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetMemberById(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null)
            return NotFound($"Член з ID {id} не знайдений");

        var memberDto = new MemberDto
        {
            Id = member.Id,
            Name = member.Name,
            Email = member.Email,
            Phone = member.Phone,
            JoinDate = member.JoinDate,
            MembershipType = member.MembershipType,
            IsActive = member.IsActive
        };
        return Ok(memberDto);
    }

    [HttpPost]
    public async Task<ActionResult<MemberDto>> CreateMember([FromBody] CreateMemberDto createMemberDto)
    {
        var validationResult = await _createMemberValidator.ValidateAsync(createMemberDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var member = new Member
        {
            Name = createMemberDto.Name,
            Email = createMemberDto.Email,
            Phone = createMemberDto.Phone,
            MembershipType = createMemberDto.MembershipType,
            JoinDate = DateTime.UtcNow,
            IsActive = true
        };

        await _memberRepository.AddAsync(member);

        var memberDto = new MemberDto
        {
            Id = member.Id,
            Name = member.Name,
            Email = member.Email,
            Phone = member.Phone,
            JoinDate = member.JoinDate,
            MembershipType = member.MembershipType,
            IsActive = member.IsActive
        };

        return CreatedAtAction(nameof(GetMemberById), new { id = memberDto.Id }, memberDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MemberDto>> UpdateMember(int id, [FromBody] UpdateMemberDto updateMemberDto)
    {
        var validationResult = await _updateMemberValidator.ValidateAsync(updateMemberDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null)
            return NotFound($"Член з ID {id} не знайдений");

        member.Name = updateMemberDto.Name;
        member.Email = updateMemberDto.Email;
        member.Phone = updateMemberDto.Phone;
        member.MembershipType = updateMemberDto.MembershipType;
        member.IsActive = updateMemberDto.IsActive;

        await _memberRepository.UpdateAsync(member);

        var memberDto = new MemberDto
        {
            Id = member.Id,
            Name = member.Name,
            Email = member.Email,
            Phone = member.Phone,
            JoinDate = member.JoinDate,
            MembershipType = member.MembershipType,
            IsActive = member.IsActive
        };

        return Ok(memberDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMember(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null)
            return NotFound($"Член з ID {id} не знайдений");

        await _memberRepository.DeleteAsync(member);
        return NoContent();
    }
}
