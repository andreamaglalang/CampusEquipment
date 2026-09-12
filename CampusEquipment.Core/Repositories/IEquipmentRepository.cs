using CampusEquipment.Core.Entities;

namespace CampusEquipment.Core.Repositories;

public interface IEquipmentRepository
{
    Task<IEnumerable<Equipment>> GetAllAsync();
    Task<Equipment?> GetByIdAsync(int id);
    Task<Equipment?> GetByAssetCodeAsync(string assetCode);
    Task<bool> AssetCodeExistsAsync(string assetCode, int? excludeId = null);
    Task<Equipment> AddAsync(Equipment equipment);
    Task UpdateAsync(Equipment equipment);
    Task DeleteAsync(int id);
}