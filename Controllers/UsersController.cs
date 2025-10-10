using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User_AMS.Data;
using User_AMS.DTOs;
using User_AMS.Models;
using User_AMS.Services;

namespace User_AMS.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Administrator")]
public class UsersController : ControllerBase
{
    private readonly LoginDbContext _db;
    private readonly PasswordHasher _hasher;

    public UsersController(LoginDbContext db, PasswordHasher hasher)
    {
        _db = db; _hasher = hasher;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserListItemDto>>> GetAll()
    {
        var data = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Status)
            .OrderByDescending(u => u.Id)
            .Select(u => new UserListItemDto(
                u.Id,
                u.Username,
                u.Role!.Name,
                u.Status!.Name,
                u.RegisteredAt
            )).ToListAsync();

        return Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] UserCreateDto dto)
    {
        if (await _db.Users.AnyAsync(x => x.Username == dto.Username))
            return Conflict(new { message = "Username already exists" });

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = _hasher.Hash(dto.Password),
            RoleId = dto.RoleId,
            StatusId = dto.StatusId,
            RegisteredAt = DateTime.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = user.Id }, new { user.Id });
    }

    [HttpPut("{id:int}/role")]
    public async Task<ActionResult> UpdateRole(int id, [FromBody] UserUpdateRoleDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.RoleId = dto.RoleId;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] UserUpdateStatusDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.StatusId = dto.StatusId;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
