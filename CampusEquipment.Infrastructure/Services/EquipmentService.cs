using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Entities;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;
using Microsoft.Extensions.Logging;

namespace CampusEquipment.Infrastructure.Services;

public class EquipmentService : IEquipmentService
{
    private static readonly string[] ValidStatuses =
    {
        "Available", "Assigned", "UnderMaintenance", "Retired"
    };

    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<EquipmentService> _logger;

    public EquipmentService(
        IEquipmentRepository equipmentRepository,
        IDepartmentRepository departmentRepository,
        ILogger<EquipmentService> logger)
    {
        _equipmentRepository = equipmentRepository;
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<EquipmentDto>> GetAllAsync()
    {
        var items = await _equipmentRepository.GetAllAsync();
        return items.Select(MapToDto);
    }

    public async Task<EquipmentDto?> GetByIdAsync(int id)
    {
        var item = await _equipmentRepository.GetByIdAsync(id);
        if (item is null)
        {
            _logger.LogWarning("Equipment {EquipmentId} not found.", id);
            return null;
        }
        return MapToDto(item);
    }

    public async Task<IEnumerable<EquipmentDto>> SearchAsync(
        string? search,
        string? category,
        string? status,
        int? departmentId)
    {
        var all = await _equipmentRepository.GetAllAsync();

        var query = all.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(e =>
                e.AssetCode.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (e.Brand != null && e.Brand.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(e => e.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(e => e.Status == status);
        }

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        return query.Select(MapToDto);
    }

    public async Task<EquipmentDto> CreateAsync(CreateEquipmentDto dto)
    {
        ValidateRequiredFields(dto.AssetCode, dto.Name, dto.Category, dto.Status, dto.DepartmentId);
        ValidateStatusValue(dto.Status);

        if (await _equipmentRepository.AssetCodeExistsAsync(dto.AssetCode))
        {
            _logger.LogWarning("Create rejected: duplicate AssetCode {AssetCode}.", dto.AssetCode);
            throw new BusinessRuleException($"Asset Code '{dto.AssetCode}' already exists.");
        }

        if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
        {
            _logger.LogWarning("Create rejected: Department {DepartmentId} does not exist.", dto.DepartmentId);
            throw new BusinessRuleException($"Department with ID {dto.DepartmentId} does not exist.");
        }

        var entity = new Equipment
        {
            AssetCode = dto.AssetCode.Trim(),
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
            Brand = dto.Brand?.Trim(),
            Model = dto.Model?.Trim(),
            PurchaseDate = dto.PurchaseDate,
            Status = dto.Status,
            DepartmentId = dto.DepartmentId
        };

        var created = await _equipmentRepository.AddAsync(entity);

        _logger.LogInformation("Equipment created: {AssetCode} ({EquipmentId}).", created.AssetCode, created.EquipmentId);

        var full = await _equipmentRepository.GetByIdAsync(created.EquipmentId);
        return MapToDto(full!);
    }

    public async Task UpdateAsync(UpdateEquipmentDto dto)
    {
        ValidateRequiredFields(dto.AssetCode, dto.Name, dto.Category, dto.Status, dto.DepartmentId);
        ValidateStatusValue(dto.Status);

        var existing = await _equipmentRepository.GetByIdAsync(dto.EquipmentId);
        if (existing is null)
        {
            _logger.LogWarning("Update rejected: Equipment {EquipmentId} not found.", dto.EquipmentId);
            throw new BusinessRuleException($"Equipment with ID {dto.EquipmentId} was not found.");
        }

        if (await _equipmentRepository.AssetCodeExistsAsync(dto.AssetCode, excludeId: dto.EquipmentId))
        {
            _logger.LogWarning("Update rejected: AssetCode {AssetCode} already used by another item.", dto.AssetCode);
            throw new BusinessRuleException($"Asset Code '{dto.AssetCode}' is already used by another equipment.");
        }

        if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
        {
            _logger.LogWarning("Update rejected: Department {DepartmentId} does not exist.", dto.DepartmentId);
            throw new BusinessRuleException($"Department with ID {dto.DepartmentId} does not exist.");
        }

        if (existing.Status == "Retired" && dto.Status == "Assigned")
        {
            _logger.LogWarning("Update rejected: Retired equipment {EquipmentId} cannot be assigned.", dto.EquipmentId);
            throw new BusinessRuleException("Retired equipment cannot be assigned.");
        }

        if (existing.Status == "UnderMaintenance" && dto.Status == "Assigned")
        {
            _logger.LogWarning("Update rejected: Equipment {EquipmentId} under maintenance cannot be assigned.", dto.EquipmentId);
            throw new BusinessRuleException("Equipment under maintenance cannot be assigned.");
        }

        existing.AssetCode = dto.AssetCode.Trim();
        existing.Name = dto.Name.Trim();
        existing.Category = dto.Category.Trim();
        existing.Brand = dto.Brand?.Trim();
        existing.Model = dto.Model?.Trim();
        existing.PurchaseDate = dto.PurchaseDate;
        existing.Status = dto.Status;
        existing.DepartmentId = dto.DepartmentId;

        await _equipmentRepository.UpdateAsync(existing);

        _logger.LogInformation("Equipment updated: {EquipmentId}.", existing.EquipmentId);
    }

    public async Task RetireAsync(int id)
    {
        var existing = await _equipmentRepository.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogWarning("Retire rejected: Equipment {EquipmentId} not found.", id);
            throw new BusinessRuleException($"Equipment with ID {id} was not found.");
        }

        if (existing.Status == "Retired")
        {
            _logger.LogInformation("Equipment {EquipmentId} is already retired.", id);
            return;
        }

        existing.Status = "Retired";
        await _equipmentRepository.UpdateAsync(existing);

        _logger.LogInformation("Equipment retired: {EquipmentId}.", id);
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _equipmentRepository.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogWarning("Delete rejected: Equipment {EquipmentId} not found.", id);
            throw new BusinessRuleException($"Equipment with ID {id} was not found.");
        }

        await _equipmentRepository.DeleteAsync(id);

        _logger.LogInformation("Equipment deleted: {EquipmentId}.", id);
    }

    private static void ValidateRequiredFields(
        string assetCode, string name, string category, string status, int departmentId)
    {
        if (string.IsNullOrWhiteSpace(assetCode))
            throw new BusinessRuleException("Asset Code is required.");
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("Name is required.");
        if (string.IsNullOrWhiteSpace(category))
            throw new BusinessRuleException("Category is required.");
        if (string.IsNullOrWhiteSpace(status))
            throw new BusinessRuleException("Status is required.");
        if (departmentId <= 0)
            throw new BusinessRuleException("Department is required.");
    }

    private static void ValidateStatusValue(string status)
    {
        if (!ValidStatuses.Contains(status))
        {
            throw new BusinessRuleException(
                $"Status '{status}' is not valid. Allowed: {string.Join(", ", ValidStatuses)}.");
        }
    }

    private static EquipmentDto MapToDto(Equipment e) => new()
    {
        EquipmentId = e.EquipmentId,
        AssetCode = e.AssetCode,
        Name = e.Name,
        Category = e.Category,
        Brand = e.Brand,
        Model = e.Model,
        PurchaseDate = e.PurchaseDate,
        Status = e.Status,
        DepartmentId = e.DepartmentId,
        DepartmentName = e.Department?.Name ?? string.Empty
    };
}