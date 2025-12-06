using System.ComponentModel.DataAnnotations;

namespace ReznichenkoWeb.Models;

public class Trainer
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    [Range(18, 100)]
    public int Age { get; set; }
    [Required]
    public string Gender { get; set; } = string.Empty;
    [Range(0, 50)]
    public int Experience { get; set; }
    public string Specialization { get; set; } = string.Empty;
    [Phone]
    public string Phone { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
