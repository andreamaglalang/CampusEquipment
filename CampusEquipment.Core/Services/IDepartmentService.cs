using CampusEquipment.Core.DTOs;

namespace CampusEquipment.Core.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
}