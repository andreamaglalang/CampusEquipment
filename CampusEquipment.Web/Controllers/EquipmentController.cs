using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Services;
using CampusEquipment.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CampusEquipment.Web.Controllers;

public class EquipmentController : Controller
{
    private readonly IEquipmentService _equipmentService;
    private readonly IDepartmentService _departmentService;

    public EquipmentController(
        IEquipmentService equipmentService,
        IDepartmentService departmentService)
    {
        _equipmentService = equipmentService;
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index(string? search, string? category, string? status, int? departmentId)
    {
        var items = await _equipmentService.SearchAsync(search, category, status, departmentId);

        ViewBag.Search = search;
        ViewBag.Category = category;
        ViewBag.Status = status;
        ViewBag.DepartmentId = departmentId;
        ViewBag.Departments = new SelectList(
            await _departmentService.GetAllAsync(),
            "DepartmentId",
            "Name");

        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _equipmentService.GetByIdAsync(id);
        if (item is null) return NotFound();
        return View(item);
    }

    public async Task<IActionResult> Create()
    {
        await LoadDepartmentsAsync();
        return View(new CreateEquipmentDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEquipmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync(dto.DepartmentId);
            return View(dto);
        }

        try
        {
            await _equipmentService.CreateAsync(dto);
            TempData["Success"] = $"Equipment '{dto.AssetCode}' was created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessRuleException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadDepartmentsAsync(dto.DepartmentId);
            return View(dto);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _equipmentService.GetByIdAsync(id);
        if (item is null) return NotFound();

        var dto = new UpdateEquipmentDto
        {
            EquipmentId = item.EquipmentId,
            AssetCode = item.AssetCode,
            Name = item.Name,
            Category = item.Category,
            Brand = item.Brand,
            Model = item.Model,
            PurchaseDate = item.PurchaseDate,
            Status = item.Status,
            DepartmentId = item.DepartmentId
        };

        await LoadDepartmentsAsync(dto.DepartmentId);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateEquipmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync(dto.DepartmentId);
            return View(dto);
        }

        try
        {
            await _equipmentService.UpdateAsync(dto);
            TempData["Success"] = $"Equipment '{dto.AssetCode}' was updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessRuleException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadDepartmentsAsync(dto.DepartmentId);
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Retire(int id)
    {
        try
        {
            await _equipmentService.RetireAsync(id);
            TempData["Success"] = "Equipment was retired successfully.";
        }
        catch (BusinessRuleException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _equipmentService.DeleteAsync(id);
            TempData["Success"] = "Equipment was deleted successfully.";
        }
        catch (BusinessRuleException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDepartmentsAsync(int? selectedId = null)
    {
        var departments = await _departmentService.GetAllAsync();
        ViewBag.Departments = new SelectList(departments, "DepartmentId", "Name", selectedId);
    }
}