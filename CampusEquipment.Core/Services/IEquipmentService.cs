using CampusEquipment.Core.DTOs;

namespace CampusEquipment.Core.Services;

public interface IEquipmentService
{
    Task<IEnumerable<EquipmentDto>> GetAllAsync();
    Task<EquipmentDto?> GetByIdAsync(int id);
    Task<EquipmentDto> CreateAsync(CreateEquipmentDto dto);
    Task UpdateAsync(UpdateEquipmentDto dto);
    Task RetireAsync(int id);
    Task DeleteAsync(int id);
    Task<IEnumerable<EquipmentDto>> SearchAsync(
        string? search,
        string? category,
        string? status,
        int? departmentId);
}