using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Entities;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;

namespace CampusEquipment.Infrastructure.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;

    public DepartmentService(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var departments = await _repository.GetAllAsync();
        return departments.Select(MapToDto);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var department = await _repository.GetByIdAsync(id);
        return department is null ? null : MapToDto(department);
    }

    public Task<bool> ExistsAsync(int id)
        => _repository.ExistsAsync(id);

    private static DepartmentDto MapToDto(Department d) => new()
    {
        DepartmentId = d.DepartmentId,
        Name = d.Name,
        Description = d.Description
    };
}