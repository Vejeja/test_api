using Microsoft.EntityFrameworkCore;

namespace test_api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserState> UserStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserGroup)
                .WithMany()
                .HasForeignKey(u => u.UserGroupId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserState)
                .WithMany()
                .HasForeignKey(u => u.UserStateId);

            modelBuilder.Entity<UserGroup>()
                .HasIndex(ug => ug.Code)
                .IsUnique();
        }
    }
}