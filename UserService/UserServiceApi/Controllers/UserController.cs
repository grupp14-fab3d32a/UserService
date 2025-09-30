using Business.Contracts.Requests;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace UserServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IAuthService authService) : ControllerBase
{
  private readonly IAuthService _authService = authService;

  [HttpPost("signin")]
  public async Task<IActionResult> SignIn([FromBody] SignInRequest form)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var response = await _authService.SignInAsync(form);
    if (!response.IsSuccess)
      return BadRequest(new { response.Message });

    return Ok(response);
  }
  [HttpPost("signup")]
  public async Task<IActionResult> SignUp([FromBody] SignUpRequest form)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var response = await _authService.RegisterAsync(form);
    if (response.IsSuccess)
      return Ok(new { response.Message });

    if (response.Errors!.Any(e => e.Code == "DuplicateEmail"))
      return Conflict(new { response.Errors });

    return BadRequest(new { response.Errors });
  }

}
