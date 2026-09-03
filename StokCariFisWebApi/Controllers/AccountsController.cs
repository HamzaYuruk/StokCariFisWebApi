using CariWebApi.Application.DTOs;
using CariWebApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CariWebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _service;

    public AccountsController(IAccountService service)
    {
        _service = service;
    }

    // GET /api/accounts
    [HttpGet]
    [Authorize(Roles = "Owner,User")]
    public async Task<ActionResult<List<AccountDto>>> GetAll(string? search, int page = 1, int pageSize = 10)
    {
        var accounts = await _service.GetAllAsync(search, page, pageSize);
        return Ok(accounts);
    }

    // GET /api/accounts/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Owner,User")]
    public async Task<ActionResult<AccountDto>> GetById(int id)
    {
        var account = await _service.GetByIdAsync(id);
        if (account == null)
        {
            return NotFound();
        }
        return Ok(account);
    }

    // POST /api/accounts
    [HttpPost]
    [Authorize(Roles = "Owner,User")]
    public async Task<ActionResult<AccountDto>> Create(CreateAccountDto dto)
    {
        var account = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    // PUT /api/accounts
    [HttpPut("{id}")]
    [Authorize(Roles = "Owner,User")]
    public async Task<ActionResult<AccountDto>> Update(int id, UpdateAccountDto dto)
    {
        var account = await _service.UpdateAsync(id, dto);
        if (account == null)
        {
            return NotFound();
        }
        return Ok(account);
    }

    // DELETE /api/accounts
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
    
    
    // POST /api/accounts/{id}/link-user
    [HttpPost("{id}/link-user")]
    [Authorize(Roles = "Owner,User")]
    public async Task<IActionResult> LinkUser(int id, LinkAccountUserDto dto)
    {
        var linked = await _service.LinkUserAsync(id, dto);
        if (!linked)
        {
            return NotFound();
        }
        return NoContent();
    }

    // GET /api/accounts/my-account
    [HttpGet("/api/my-account")]
    public async Task<ActionResult<MyAccountDto>> GetMyAccount()
    {
        var account = await _service.GetMyAccountAsync();
        if (account == null)
        {
            return NotFound();
        }
        return Ok(account);
    }
}