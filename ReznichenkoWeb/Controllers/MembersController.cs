using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberRepository _memberRepository;
    private readonly IValidator<CreateMemberDto> _createMemberValidator;
    private readonly IValidator<UpdateMemberDto> _updateMemberValidator;
    private readonly IMemoryCache _cache;
    private readonly GymSettings _settings;
    private const string MEMBERS_CACHE_KEY = "members_all";

    public MembersController(
        IMemberRepository memberRepository,
        IValidator<CreateMemberDto> createMemberValidator,
        IValidator<UpdateMemberDto> updateMemberValidator,
        IMemoryCache cache,
        IOptionsSnapshot<GymSettings> settings)
    {
        _memberRepository = memberRepository;
        _createMemberValidator = createMemberValidator;
        _updateMemberValidator = updateMemberValidator;
        _cache = cache;
        _settings = settings.Value;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllMembers()
    {
        if (!_cache.TryGetValue(MEMBERS_CACHE_KEY, out IEnumerable<MemberDto> memberDtos))
        {
            var members = await _memberRepository.GetAllAsync();
            memberDtos = members.Select(m => new MemberDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                JoinDate = m.JoinDate,
                MembershipType = m.MembershipType,
                IsActive = m.IsActive
            }).ToList();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(MEMBERS_CACHE_KEY, memberDtos, cacheOptions);
        }

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
        if (!_settings.AllowNewRegistrations)
        {
            return BadRequest("Реєстрація нових членів тимчасово припинена адміністратором.");
        }

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
        _cache.Remove(MEMBERS_CACHE_KEY);

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
        _cache.Remove(MEMBERS_CACHE_KEY);

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
        _cache.Remove(MEMBERS_CACHE_KEY);
        return NoContent();
    }
}
