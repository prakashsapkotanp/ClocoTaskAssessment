using ArtistManagementSystem.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtistManagementSystem.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
            // inject service dependencies here
        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ArtistModel> Artists { get; set; }
        public DbSet<MusicModel> Songs { get; set; }
        public DbSet<UserRoleModel> UserRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>().Property(u => u.Gender).HasConversion<string>();
            modelBuilder.Entity<ArtistModel>().Property(a => a.Gender).HasConversion<string>();
            modelBuilder.Entity<MusicModel>().Property(m => m.Genre).HasConversion<string>();
            modelBuilder.Entity<RoleModel>().Property(r => r.RoleName).HasConversion<string>();

            modelBuilder.Entity<UserModel>().ToTable("user");
            modelBuilder.Entity<ArtistModel>().ToTable("artist");
            modelBuilder.Entity<MusicModel>().ToTable("music");
            modelBuilder.Entity<RoleModel>().ToTable("role");
        }
    }
}
