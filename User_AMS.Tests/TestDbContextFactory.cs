using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using User_AMS.Data;

public static class TestDbContextFactory
{
    public static LoginDbContext Create()
    {
        var options = new DbContextOptionsBuilder<LoginDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Cada prueba con DB única
            .Options;

        return new LoginDbContext(options);
    }
}
