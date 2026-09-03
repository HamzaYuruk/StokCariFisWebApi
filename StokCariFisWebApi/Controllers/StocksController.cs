using CariWebApi.Application.DTOs;
using CariWebApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CariWebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/stocks")]
public class StocksController : ControllerBase
{
    private readonly IStockService _service;

    public StocksController(IStockService service)
    {
        _service = service;
    }

    // GET /api/stocks
    [HttpGet]
    [Authorize(Roles = "Owner,User")]
    public async Task<ActionResult<List<StockDto>>> GetAll(string? search, int page = 1, int pageSize = 10)
    {
        var stocks = await _service.GetAllAsync(search, page, pageSize);
        return Ok(stocks);
    }

    // GET /api/stocks/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Owner,User")]
    public async Task<ActionResult<StockDto>> GetById(int id)
    {
        var stock = await _service.GetByIdAsync(id);
        if (stock == null)
        {
            return NotFound();
        }
        return Ok(stock);
    }

    // POST /api/stocks
    [HttpPost]
    [Authorize(Roles = "Owner,User")]
    public async Task<ActionResult<StockDto>> Create(CreateStockDto dto)
    {
        var stock = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = stock.Id }, stock);
    }

    // PUT /api/stocks/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Owner,User")]
    public async Task<ActionResult<StockDto>> Update(int id, UpdateStockDto dto)
    {
        var stock = await _service.UpdateAsync(id, dto);
        if (stock == null)
        {
            return NotFound();
        }
        return Ok(stock);
    }

    // DELETE /api/stocks/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner,User")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}