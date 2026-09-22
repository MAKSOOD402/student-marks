using System.ComponentModel.DataAnnotations;

namespace StudentMarksApi.Models;

public class StudentMark
{
    public int id { get; set; }

    [Required]
    [StringLength(100)]
    public string name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string subject { get; set; } = string.Empty;

    [Range(0, 100)]
    public decimal marks { get; set; }
}