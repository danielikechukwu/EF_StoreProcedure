using EF_StoreProcedure.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF_StoreProcedure.Data
{
    public class EF_StoreProcedureDbContext : DbContext
    {

        public EF_StoreProcedureDbContext(DbContextOptions<EF_StoreProcedureDbContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
    }
}
