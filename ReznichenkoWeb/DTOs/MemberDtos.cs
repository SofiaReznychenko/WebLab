using System.ComponentModel.DataAnnotations;

public class MemberDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public string MembershipType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
}

public class CreateMemberDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MembershipType { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
}

public class UpdateMemberDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MembershipType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
}
