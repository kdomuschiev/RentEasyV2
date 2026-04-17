using System.ComponentModel.DataAnnotations;
using renteasy_api.Domain.Enums;

namespace renteasy_api.Application.DTOs.Properties;

public class UpdatePropertyRequest
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string Address { get; set; } = string.Empty;

    public decimal? SizeSqm { get; set; }
    public int? Floor { get; set; }
    public List<BillCategory> BillCategories { get; set; } = [];
}
