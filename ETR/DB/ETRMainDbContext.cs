using ETR.Models.DBModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ETR.DB
{
    public class ETRMainDbContext : DbContext {
		public ETRMainDbContext(DbContextOptions<ETRMainDbContext> options)
		: base(options) {
		}
		public DbSet<BaseItem> BaseItems { get; set; }
		public DbSet<IdentityUser> Users { get; set; }
		public DbSet<IdentityRole> Roles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);
		}
		protected override void OnConfiguring(DbContextOptionsBuilder options) {
		//	options.use
		}
	}
}
