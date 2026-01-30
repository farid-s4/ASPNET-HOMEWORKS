using System.ComponentModel.DataAnnotations;

namespace ProfileCardMVC.Models;

public class Project
{
    public int Id { get; set; }
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public DateTime CreatedAt { get; set; } =  DateTime.Now;
    public DateTime UpdatedAt { get; set; } 
    public bool IsActive { get; set; } = true;
    
    public int UserId { get; set; } 
    public User User { get; set; }
}
