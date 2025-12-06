using System.ComponentModel.DataAnnotations;

namespace ReznichenkoWeb.Models;

public class Member
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public string MembershipType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    [Range(1, 120)]
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
}
