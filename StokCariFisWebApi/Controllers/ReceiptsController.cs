using CariWebApi.Application.DTOs;
using CariWebApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CariWebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/receipts")]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _service;

    public ReceiptsController(IReceiptService service)
    {
        _service = service;
    }

    // GET /api/receipts
    [HttpGet]
    public async Task<ActionResult<List<ReceiptDto>>> GetAll(int page = 1, int pageSize = 10)
    {
        var receipts = await _service.GetAllAsync(page, pageSize);
        return Ok(receipts);
    }

    // GET /api/receipts/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ReceiptDto>> GetById(int id)
    {
        var receipt = await _service.GetByIdAsync(id);
        if (receipt == null)
        {
            return NotFound();
        }
        return Ok(receipt);
    }
    
    // POST /api/receipts
    [HttpPost]
    public async Task<ActionResult<ReceiptDto>> Create(CreateReceiptDto dto)
    {
        var receipt = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = receipt.Id }, receipt);
    }

    // POST /api/receipts/{id}/details
    [HttpPost("{id}/details")]
    public async Task<ActionResult<ReceiptDto>> AddDetail(int id, AddReceiptDetailDto dto)
    {
        var receipt = await _service.AddDetailAsync(id, dto);
        if (receipt == null)
        {
            return NotFound();
        }
        return Ok(receipt);
    }

    // PUT /api/receipts/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ReceiptDto>> Update(int id, CreateReceiptDto dto)
    {
        var receipt = await _service.UpdateAsync(id, dto);
        if (receipt == null)
        {
            return NotFound();
        }
        return Ok(receipt);
    }

    // DELETE /api/receipts/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
    
    // POST /api/receipts/{id}/approve
    [HttpPost("{id}/approve")]
    public async Task<ActionResult<ReceiptDto>> Approve(int id)
    {
        var receipt = await _service.ApproveAsync(id);
        if (receipt == null)
        {
            return NotFound();
        }
        return Ok(receipt);
    }
}