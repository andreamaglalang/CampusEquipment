using CampusEquipment.Core.Entities;

namespace CampusEquipment.Core.Repositories;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
}