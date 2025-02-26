using Microsoft.EntityFrameworkCore;
using MvcCubos.Models;

namespace MvcCubos.Data
{
    public class CubosContext: DbContext
    {
        public CubosContext(DbContextOptions<CubosContext> options) : base(options) { }

        public DbSet<Cubo> Cubos { get; set; }
    }
}
