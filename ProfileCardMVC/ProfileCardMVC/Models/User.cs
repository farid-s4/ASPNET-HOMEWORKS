using System.ComponentModel.DataAnnotations;

namespace ProfileCardMVC.Models;

public class User
{
    public int Id { get; set; }
    
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(30)]
    public string Email { get; set; } = string.Empty;
    [MaxLength(150)]
    public string ShortBio { get; set; } = string.Empty;
    [MaxLength(500)]
    public string LongBio { get; set; } = string.Empty;
    
    public string ImageUrl { get; set; } = string.Empty;
    
    [Required]
    public DateTime Birthday { get; set; }

    public IEnumerable<Skill> Skills { get; set; } = new List<Skill>();
    public IEnumerable<Project> Projects { get; set; } = new List<Project>();
}