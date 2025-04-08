using System.ComponentModel.DataAnnotations;

namespace LabProject.Models
{
    public class ClassInformationModel
{
    public int Id { get; set; }
    
    [Required]
    public string ClassName { get; set; } = string.Empty; // Default value
    
    public int StudentCount { get; set; }
    
    [Required]
    public string Description { get; set; } = string.Empty; // Default value
}

}
