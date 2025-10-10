using User_AMS.Models;

namespace User_AMS.Services;

public interface IJwtService
{
    string CreateToken(User user, string roleName);
}
