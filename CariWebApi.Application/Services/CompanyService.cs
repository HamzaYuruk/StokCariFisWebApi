using AutoMapper;
using CariWebApi.Application.Constants;
using CariWebApi.Application.DTOs;
using CariWebApi.Application.Interfaces;
using CariWebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CariWebApi.Domain.Enums;
using CariWebApi.Application.DTOs.Auth;

namespace CariWebApi.Application.Services;
// userrole ve role adlı iki tablo oluştur
// hata mesajlarını ortak bir metoda topla
// seed et kütüphane vs 
// role enums kaldır
public class CompanyService : ICompanyService
{
    private readonly IRepository<Company> _repository;
    private readonly IRepository<UserCompanyRole> _userCompanyRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CompanyService> _logger;
    private readonly JwtService _jwtService;
    private readonly IRepository<Role> _roleRepository;
    
    public CompanyService(
        IRepository<Company> repository,
        IRepository<UserCompanyRole> userCompanyRepository,
        IRepository<User> userRepository,
        IRepository<Role> roleRepository,
        IMapper mapper,
        ILogger<CompanyService> logger,
        JwtService jwtService)
        
    {
        _repository = repository;
        _userCompanyRepository = userCompanyRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
        _jwtService = jwtService;
    }

    // şirketleri get yapma
    public async Task<List<CompanyDto>> GetAllAsync(int userId)
    {
        var companyIds = await _userCompanyRepository.Query()
            .Where(uc => uc.UserId == userId && uc.IsActive)
            .Select(uc => uc.CompanyId)
            .ToListAsync();

        var companies = await _repository.Query()
            .Where(c => companyIds.Contains(c.Id) && !c.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<CompanyDto>>(companies);
    }
    
    
    // id si verilen şirketi get 
    public async Task<CompanyDto?> GetByIdAsync(int id)
    {
        var company = await _repository.Query()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (company == null)
        {
            return null;
        }
        return _mapper.Map<CompanyDto>(company);
    }

    // şirket seçme-bir kullanıcının birden fazla şirketi olabilirdi
    public async Task<CreateCompanyResponseDto> SelectCompanyAsync(int companyId, int userId)
    {
        var userCompanyRole = await _userCompanyRepository.Query()
            .Include(uc => uc.Role)
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == companyId && uc.IsActive);

        if (userCompanyRole == null)
        {
            _logger.LogWarning("Yetkisiz şirket seçme denemesi: UserId {UserId}, CompanyId {CompanyId}", userId, companyId);
            throw new ValidationException(ErrorMessages.NoAccessToCompany);
        }

        var company = await _repository.GetByIdAsync(companyId);
        var user = await _userRepository.GetByIdAsync(userId);

        var newToken = _jwtService.GenerateToken(user!, companyId, userCompanyRole.Role!.Name);

        _logger.LogInformation("Kullanıcı şirket seçti: UserId {UserId}, CompanyId {CompanyId}, Role {Role}", userId, companyId, userCompanyRole.Role.Name);

        return new CreateCompanyResponseDto
        {
            Company = _mapper.Map<CompanyDto>(company),
            Token = newToken
        };
    }
    
    // şirket create
    public async Task<CreateCompanyResponseDto> CreateAsync(CreateCompanyDto dto, int userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ValidationException(ErrorMessages.CompanyNameRequired);
        }

        var company = _mapper.Map<Company>(dto);
        await _repository.AddAsync(company);
        await _repository.SaveChangesAsync();

        var ownerRole = await _roleRepository.Query().FirstOrDefaultAsync(r => r.Name == "Owner");
        
     
        var userCompanyRole = new UserCompanyRole
        {
            UserId = userId,
            CompanyId = company.Id,
            RoleId = ownerRole!.Id
        };
        
        await _userCompanyRepository.AddAsync(userCompanyRole);
        await _userCompanyRepository.SaveChangesAsync();

        _logger.LogInformation("Yeni şirket oluşturuldu: {CompanyId} - {CompanyName}, Owner: {UserId}", company.Id, company.Name, userId);

        var user = await _userRepository.GetByIdAsync(userId);
        var newToken = _jwtService.GenerateToken(user!, company.Id, "Owner");

        return new CreateCompanyResponseDto
        {
            Company = _mapper.Map<CompanyDto>(company),
            Token = newToken
        };
    }

    // şirket update
    public async Task<CompanyDto?> UpdateAsync(int id, UpdateCompanyDto dto)
    {
        
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ValidationException(ErrorMessages.CompanyNameRequired);
        }

        var company = await _repository.GetByIdAsync(id);
        if (company == null)
        {
            _logger.LogWarning("Güncellenmek istenen şirket bulunamadı: {CompanyId}", id);
            return null;
            
        }

        _mapper.Map(dto, company);

        _repository.Update(company);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Şirket güncellendi: {CompanyId} - {CompanyName}", company.Id, company.Name);
        
        return _mapper.Map<CompanyDto>(company);
    }

    // şirket delete
    public async Task<bool> DeleteAsync(int id)
    {
        var company = await _repository.GetByIdAsync(id);
        if (company == null)
        {
            _logger.LogWarning("Silinmek istenen şirket bulunamadı: {CompanyId}", id);
            return false;
        }

        // kaydı silmiyoruz IsDeleted true yapıyoruz
        company.IsDeleted = true;
        _repository.Update(company);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Şirket silindi : {CompanyId} - {CompanyName}", company.Id, company.Name);
        
        return true;
    }
}