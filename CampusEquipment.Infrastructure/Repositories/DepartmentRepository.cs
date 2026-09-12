using CampusEquipment.Core.Entities;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusEquipment.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        return await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        return await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DepartmentId == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Departments
            .AnyAsync(d => d.DepartmentId == id);
    }
}