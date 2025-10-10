using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User_AMS.Data;
using User_AMS.DTOs;
using User_AMS.Services;

namespace User_AMS.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginDbContext _db;
    private readonly PasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public AuthController(LoginDbContext db, PasswordHasher hasher, IJwtService jwt)
    {
        _db = db; _hasher = hasher; _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { message = "Username and password are required." });

        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Status)
            .FirstOrDefaultAsync(u => u.Username == req.Username);

        if (user is null || !_hasher.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid username or password." });

        if (string.Equals(user.Status?.Name, "Inactive", StringComparison.OrdinalIgnoreCase))
            return Unauthorized(new { message = "Inactive user. Contact the administrator." });

        var token = _jwt.CreateToken(user, user.Role?.Name ?? "User");
        return Ok(new LoginResponse(token, user.Username, user.Role?.Name ?? "", user.Status?.Name ?? ""));
    }
}
