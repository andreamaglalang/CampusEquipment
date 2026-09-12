using CampusEquipment.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusEquipment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentController(IDepartmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(new
        {
            success = true,
            message = "Departments retrieved successfully.",
            data = items
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound(new
            {
                success = false,
                message = $"Department with ID {id} was not found.",
                data = (object?)null
            });
        }

        return Ok(new
        {
            success = true,
            message = "Department retrieved successfully.",
            data = item
        });
    }
}