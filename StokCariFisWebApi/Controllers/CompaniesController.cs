using CariWebApi.Application.DTOs;
using CariWebApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using CariWebApi.Application.DTOs.Auth;


namespace CariWebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/company")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _service;
    private readonly ICurrentUserService _currentUser;

    public CompaniesController(ICompanyService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    // GET /api/company
    [HttpGet] 
    public async Task<ActionResult<List<CompanyDto>>> GetAll()
    {
        var companies = await _service.GetAllAsync(_currentUser.UserId);
        return Ok(companies);
    }
    
    // GET /api/company/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetById(int id)
    {
        var company = await _service.GetByIdAsync(id);
        if (company == null)
        {
            return NotFound();
        }
        return Ok(company);
    }
    
    // POST /api/company/{id}/select
    [HttpPost("{id}/select")]
    public async Task<ActionResult<CreateCompanyResponseDto>> SelectCompany(int id)
    {
        var result = await _service.SelectCompanyAsync(id, _currentUser.UserId);
        return Ok(result);
    }

    // POST /api/company
    [HttpPost]
    public async Task<ActionResult<CreateCompanyResponseDto>> Create(CreateCompanyDto dto)
    {
        var result = await _service.CreateAsync(dto, _currentUser.UserId);
        return CreatedAtAction(nameof(GetById), new { id = result.Company.Id }, result);
    }
    
    // PUT /api/company/{id}
    [Authorize(Roles = "Owner")]
    [HttpPut("{id}")]
    public async Task<ActionResult<CompanyDto>> Update(int id, UpdateCompanyDto dto)
    {
        var company = await _service.UpdateAsync(id, dto);
        if (company == null)
        {
            return NotFound();
        }
        return Ok(company);
    }

    // DELETE /api/company/{id}
    [Authorize(Roles = "Owner")]
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
}