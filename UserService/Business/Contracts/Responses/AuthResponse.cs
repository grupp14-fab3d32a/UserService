using Business.Models;

namespace Business.Contracts.Responses;

public class AuthResponse
{
  public string Token { get; set; } = null!;
  public string? Message { get; set; }
  public bool IsSuccess { get; set; }
}
