using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Data.Entities;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/user/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required.");

            // 2. Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email is already registered.");

            // 3. Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 4. Save user to database
            var user = new User
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                Name = request.Name
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 5. Return success response
            return Ok(new { Message = "User registered successfully", UserId = user.Id });
        }

        // POST: api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required.");

            // 2. Look up user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized(new { Message = "Invalid email or password" });

            // 3. Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized(new { Message = "Invalid email or password" });

            // 4. Return success response
            return Ok(new { Message = "Login successful", UserId = user.Id, user.Email, user.Name });
        }
    }

    // Request models
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
