using Microsoft.EntityFrameworkCore;
using User_AMS.Models;

namespace User_AMS.Data;

public class LoginDbContext : DbContext
{
    public LoginDbContext(DbContextOptions<LoginDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Status> Statuses => Set<Status>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map to lowercase table names (roles, statuses, users)
        modelBuilder.Entity<Role>().ToTable("roles").HasKey(x => x.Id);
        modelBuilder.Entity<Role>().Property(x => x.Name).HasMaxLength(50).IsRequired();

        modelBuilder.Entity<Status>().ToTable("statuses").HasKey(x => x.Id);
        modelBuilder.Entity<Status>().Property(x => x.Name).HasMaxLength(50).IsRequired();

        modelBuilder.Entity<User>().ToTable("users").HasKey(x => x.Id);
        modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
        modelBuilder.Entity<User>().Property(x => x.Username).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<User>().Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Status).WithMany().HasForeignKey(u => u.StatusId);
    }
}
