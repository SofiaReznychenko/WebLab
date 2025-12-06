using System.ComponentModel.DataAnnotations;

namespace ReznichenkoWeb.ViewModels
{
    public class MemberViewModel
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
        [Required]
        public string MembershipType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        [Range(1, 120)]
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; } = string.Empty;
    }
}
