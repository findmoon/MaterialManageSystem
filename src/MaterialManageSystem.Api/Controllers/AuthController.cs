using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MaterialManageSystem.Api.DTOs;
using MaterialManageSystem.Core.Interfaces;

namespace MaterialManageSystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IConfiguration _configuration;

    public AuthController(
        IUserRepository userRepository,
        IEmployeeRepository employeeRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _employeeRepository = employeeRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ApiResponse<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return ApiResponse<LoginResponse>.Fail(400, "用户名和密码不能为空");
        }

        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            return ApiResponse<LoginResponse>.Fail(401, "用户名或密码错误");
        }

        if (!user.IsActive)
        {
            return ApiResponse<LoginResponse>.Fail(403, "用户已被禁用");
        }

        user.LastLoginAt = DateTime.Now;
        user.LastLoginIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        await _userRepository.UpdateAsync(user);

        var employee = await _employeeRepository.GetByIdAsync(user.EmployeeId);
        var token = GenerateJwtToken(user);

        return ApiResponse<LoginResponse>.Success(new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Name = employee?.Name ?? "",
            UserType = (int)user.UserType
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ApiResponse<bool>> Logout()
    {
        return ApiResponse<bool>.Success(true, "登出成功");
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ApiResponse<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return ApiResponse<RegisterResponse>.Fail(400, "用户名和密码不能为空");
        }

        if (request.Password.Length < 6)
        {
            return ApiResponse<RegisterResponse>.Fail(400, "密码长度至少为6位");
        }

        var existing = await _userRepository.GetByUsernameAsync(request.Username);
        if (existing != null)
        {
            return ApiResponse<RegisterResponse>.Fail(400, "用户名已存在");
        }

        var user = new Core.Entities.User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            EmployeeId = request.EmployeeId ?? 0,
            UserType = Core.Enums.UserType.Normal,
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = "System"
        };

        _userRepository.Add(user);
        await _userRepository.SaveChangesAsync();

        return ApiResponse<RegisterResponse>.Success(new RegisterResponse
        {
            UserId = user.Id,
            Username = user.Username
        }, "注册成功");
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<ApiResponse<LoginResponse>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return ApiResponse<LoginResponse>.Fail(401, "未授权");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse<LoginResponse>.Fail(404, "用户不存在");
        }

        var employee = await _employeeRepository.GetByIdAsync(user.EmployeeId);

        return ApiResponse<LoginResponse>.Success(new LoginResponse
        {
            Token = "",
            UserId = user.Id,
            Username = user.Username,
            Name = employee?.Name ?? "",
            UserType = (int)user.UserType
        });
    }

    private string GenerateJwtToken(Core.Entities.User user)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
        var issuer = _configuration["Jwt:Issuer"] ?? "MaterialManageSystem";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.UserType.ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }
}
