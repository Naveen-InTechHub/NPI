using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPI.Data.Context;
using NPI.Data.Entities;
using NPI.Shared.DTOs;

namespace NPI.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly NpiDbContext _db;

        public AuthController(NpiDbContext db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = _db.Users
                    .FirstOrDefault(u => u.UserName == request.Username && u.Password == request.Password);

                if (user == null)
                    return Unauthorized(new ApiResponse<LoginResponse> { Success = false, Message = "Invalid username or password." });

                var role = _db.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.Role.Name)
                    .FirstOrDefault();
                LoginResponse response = new LoginResponse
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Role = role
                };

                return Ok(new ApiResponse<LoginResponse> { Success = true, Data = response} );
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<LoginResponse> { Success = false, Message = ex.Message });
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

}
