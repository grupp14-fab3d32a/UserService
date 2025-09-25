using Business.Contracts.Requests;
using Business.Contracts.Responses;
using Data.Entities;

namespace Business.Interfaces;
public interface IAuthService
{
  Task<AuthResponse> SignInAsync(SignInRequest form);
  Task<AuthResponse> RegisterAsync(SignUpRequest form);
  Task<string> GenerateJwtToken(UserEntity user);
}
