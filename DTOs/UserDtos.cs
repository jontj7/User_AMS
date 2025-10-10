namespace User_AMS.DTOs;

public record UserCreateDto(string Username, string Password, int RoleId, int StatusId);
public record UserUpdateRoleDto(int RoleId);
public record UserUpdateStatusDto(int StatusId);

public record UserListItemDto(
    int Id,
    string Username,
    string Role,
    string Status,
    DateTime RegisteredAt
);
