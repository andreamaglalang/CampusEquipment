using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Services;
using CampusEquipment.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusEquipment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _service;

    public EquipmentController(IEquipmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] string? status,
        [FromQuery] int? departmentId)
    {
        var items = await _service.SearchAsync(search, category, status, departmentId);
        return Ok(new
        {
            success = true,
            message = "Equipment retrieved successfully.",
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
                message = $"Equipment with ID {id} was not found.",
                data = (object?)null
            });
        }

        return Ok(new
        {
            success = true,
            message = "Equipment retrieved successfully.",
            data = item
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Validation failed.",
                data = ModelState
            });
        }

        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.EquipmentId }, new
            {
                success = true,
                message = "Equipment created successfully.",
                data = created
            });
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message,
                data = (object?)null
            });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentDto dto)
    {
        if (id != dto.EquipmentId)
        {
            return BadRequest(new
            {
                success = false,
                message = "Route ID and body EquipmentId do not match.",
                data = (object?)null
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Validation failed.",
                data = ModelState
            });
        }

        try
        {
            await _service.UpdateAsync(dto);
            return Ok(new
            {
                success = true,
                message = "Equipment updated successfully.",
                data = (object?)null
            });
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message,
                data = (object?)null
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok(new
            {
                success = true,
                message = "Equipment deleted successfully.",
                data = (object?)null
            });
        }
        catch (BusinessRuleException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message,
                data = (object?)null
            });
        }
    }

    [HttpPost("{id:int}/retire")]
    public async Task<IActionResult> Retire(int id)
    {
        try
        {
            await _service.RetireAsync(id);
            return Ok(new
            {
                success = true,
                message = "Equipment retired successfully.",
                data = (object?)null
            });
        }
        catch (BusinessRuleException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message,
                data = (object?)null
            });
        }
    }
}