using System.ComponentModel.DataAnnotations;
using renteasy_api.Domain.Enums;

namespace renteasy_api.Application.DTOs.Properties;

public class CreatePropertyRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Range(0.01, 10000)]
    public decimal? SizeSqm { get; set; }

    [Range(-5, 200)]
    public int? Floor { get; set; }

    public List<BillCategory> BillCategories { get; set; } = [];
}
