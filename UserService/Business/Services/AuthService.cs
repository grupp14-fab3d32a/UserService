using Business.Contracts.Requests;
using Business.Contracts.Responses;
using Business.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Services;
public class AuthService(UserManager<UserEntity> userManager, IConfiguration config) : IAuthService
{
  private readonly IConfiguration _config = config;
  private readonly UserManager<UserEntity> _userManager = userManager;

  public async Task<AuthResponse> SignInAsync(SignInRequest form)
  {
    var user = await _userManager.FindByEmailAsync(form.Email);
    if (user == null || !await _userManager.CheckPasswordAsync(user, form.Password))
    {
      return new() { IsSuccess = false, Message = "Invalid email or password." };
    }

    var token = await GenerateJwtToken(user);

    return new()
    {
      IsSuccess = true,
      Token = token
    };
  }

  public async Task<AuthResponse> RegisterAsync(SignUpRequest form)
  {
    var entity = Factories.AuthFactory.ToEntity(form);
    var result = await _userManager.CreateAsync(entity, form.Password);
    if (!result.Succeeded)
    {
      //var errors = string.Join("; ", result.Errors.Select(e => e.Description));
      //return new() { IsSuccess = false, Message = string.Join(",", result.Errors.Select(e => e.Description)) };
      return new() { IsSuccess = false, Errors = result.Errors };
    }

    await _userManager.AddToRoleAsync(entity, "User");

    return new() { IsSuccess = true, Message = "User registered successfully." };
  }

  public async Task<string> GenerateJwtToken(UserEntity user)
  {
    var claims = new List<Claim>
    {
      new (ClaimTypes.NameIdentifier, user.Id),
      new (ClaimTypes.Email, user.Email!),
      new (ClaimTypes.GivenName, user.FirstName),
      new (ClaimTypes.Surname, user.LastName),
    };

    var roles = await _userManager.GetRolesAsync(user);
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _config["Jwt:Issuer"],
      audience: _config["Jwt:Audience"],
      claims: claims,
      expires: DateTime.Now.AddHours(3),
      signingCredentials: creds
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
