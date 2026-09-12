using CampusEquipment.Core.Entities;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusEquipment.Infrastructure.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly AppDbContext _context;

    public EquipmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync()
    {
        return await _context.Equipment
            .AsNoTracking()
            .Include(e => e.Department)
            .OrderBy(e => e.AssetCode)
            .ToListAsync();
    }

    public async Task<Equipment?> GetByIdAsync(int id)
    {
        return await _context.Equipment
            .AsNoTracking()
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.EquipmentId == id);
    }

    public async Task<Equipment?> GetByAssetCodeAsync(string assetCode)
    {
        return await _context.Equipment
            .AsNoTracking()
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.AssetCode == assetCode);
    }

    public async Task<bool> AssetCodeExistsAsync(string assetCode, int? excludeId = null)
    {
        return await _context.Equipment
            .AnyAsync(e => e.AssetCode == assetCode
                        && (!excludeId.HasValue || e.EquipmentId != excludeId.Value));
    }

    public async Task<Equipment> AddAsync(Equipment equipment)
    {
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();
        return equipment;
    }

    public async Task UpdateAsync(Equipment equipment)
    {
        _context.Equipment.Update(equipment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var equipment = await _context.Equipment.FindAsync(id);
        if (equipment is not null)
        {
            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
        }
    }
}