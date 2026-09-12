using System.ComponentModel.DataAnnotations;

namespace CampusEquipment.Core.DTOs;

public class CreateEquipmentDto
{
    [Required(ErrorMessage = "Asset Code is required.")]
    [StringLength(50, ErrorMessage = "Asset Code cannot exceed 50 characters.")]
    [Display(Name = "Asset Code")]
    public string AssetCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Brand { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    [Display(Name = "Purchase Date")]
    [DataType(DataType.Date)]
    public DateOnly? PurchaseDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(50)]
    public string Status { get; set; } = "Available";

    [Required(ErrorMessage = "Department is required.")]
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }
}