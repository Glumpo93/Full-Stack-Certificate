using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; }
}