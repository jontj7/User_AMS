using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using User_AMS.Controllers;
using User_AMS.Data;
using User_AMS.DTOs;
using User_AMS.Models;
using User_AMS.Services;
using Xunit;

public class UsersControllerTests
{
    private readonly LoginDbContext _db;
    private readonly PasswordHasher _hasher;

    public UsersControllerTests()
    {
        _db = TestDbContextFactory.Create();
        _hasher = new PasswordHasher();
    }

    // ✅ 1. Obtener lista de usuarios
    [Fact(DisplayName = "Obtener lista de usuarios")]
    public async Task GetAll_RetornaListaDeUsuarios()
    {
        var role = new Role { Name = "Administrator" };
        var status = new Status { Name = "Active" };
        _db.Roles.Add(role);
        _db.Statuses.Add(status);
        await _db.SaveChangesAsync();

        var user = new User
        {
            Username = "testuser",
            PasswordHash = _hasher.Hash("password123"),
            RoleId = role.Id,
            StatusId = status.Id
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var controller = new UsersController(_db, _hasher);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsAssignableFrom<IEnumerable<UserListItemDto>>(okResult.Value);
        Assert.Single(users);
    }

    // ✅ 2. Crear usuario correctamente
    [Fact(DisplayName = "Crear usuario correctamente")]
    public async Task Create_CreaNuevoUsuario()
    {
        var role = new Role { Name = "User" };
        var status = new Status { Name = "Active" };
        _db.Roles.Add(role);
        _db.Statuses.Add(status);
        await _db.SaveChangesAsync();

        var controller = new UsersController(_db, _hasher);

        var dto = new UserCreateDto("nuevo_usuario", "123456", role.Id, status.Id);

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(created.Value);
    }

    // ✅ 3. Actualizar estado del usuario
    [Fact(DisplayName = "Actualizar estado del usuario")]
    public async Task UpdateStatus_CambiaEstadoCorrectamente()
    {
        var role = new Role { Name = "User" };
        var active = new Status { Name = "Active" };
        var inactive = new Status { Name = "Inactive" };
        _db.Roles.Add(role);
        _db.Statuses.AddRange(active, inactive);
        await _db.SaveChangesAsync();

        var user = new User
        {
            Username = "usuario_estado",
            PasswordHash = _hasher.Hash("123"),
            RoleId = role.Id,
            StatusId = active.Id
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var controller = new UsersController(_db, _hasher);

        var dto = new UserUpdateStatusDto(inactive.Id);
        var result = await controller.UpdateStatus(user.Id, dto);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(inactive.Id, (await _db.Users.FindAsync(user.Id))!.StatusId);
    }

    // ✅ 4. Actualizar rol del usuario
    [Fact(DisplayName = "Actualizar rol del usuario")]
    public async Task UpdateRole_CambiaRolCorrectamente()
    {
        var role1 = new Role { Name = "User" };
        var role2 = new Role { Name = "Admin" };
        var status = new Status { Name = "Active" };
        _db.Roles.AddRange(role1, role2);
        _db.Statuses.Add(status);
        await _db.SaveChangesAsync();

        var user = new User
        {
            Username = "usuario_rol",
            PasswordHash = _hasher.Hash("123"),
            RoleId = role1.Id,
            StatusId = status.Id
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var controller = new UsersController(_db, _hasher);

        var dto = new UserUpdateRoleDto(role2.Id);
        var result = await controller.UpdateRole(user.Id, dto);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(role2.Id, (await _db.Users.FindAsync(user.Id))!.RoleId);
    }
}
