using User_AMS.Models;

namespace User_AMS.Data;

public static class DbSeeder
{
    public static void Seed(LoginDbContext db, Services.PasswordHasher hasher)
    {
        // roles/statuses los creaste por SQL; dejamos por si falta algo
        if (!db.Roles.Any())
        {
            db.Roles.AddRange(
                new Role { Name = "Administrator" },
                new Role { Name = "User" }
            );
            db.SaveChanges();
        }

        if (!db.Statuses.Any())
        {
            db.Statuses.AddRange(
                new Status { Name = "Active" },
                new Status { Name = "Inactive" }
            );
            db.SaveChanges();
        }

        // admin por defecto (admin/admin123)
        if (!db.Users.Any(u => u.Username == "admin"))
        {
            var adminRole = db.Roles.First(r => r.Name == "Administrator");
            var active = db.Statuses.First(s => s.Name == "Active");

            db.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = hasher.Hash("admin123"),
                RoleId = adminRole.Id,
                StatusId = active.Id,
                RegisteredAt = DateTime.UtcNow
            });
            db.SaveChanges();
        }
    }
}
