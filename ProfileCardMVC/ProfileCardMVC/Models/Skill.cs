using System.ComponentModel.DataAnnotations;

namespace ProfileCardMVC.Models;

public class Skill
{
    public int Id { get; set; }
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    
    public int UserId { get; set; } 
    public User User { get; set; }
}