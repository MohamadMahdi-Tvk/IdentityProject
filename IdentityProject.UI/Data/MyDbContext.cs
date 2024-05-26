using IdentityProject.UI.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityProject.UI.Data
{
    public class MyDbContext : IdentityDbContext<User, Role, string>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        /*protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityUserLogin<string>>().HasKey(p => new { p.ProviderKey, p.LoginProvider });
            builder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            builder.Entity<IdentityUserToken<string>>().HasKey(p => new { p.UserId, p.LoginProvider, p.Name });

            builder.Entity<User>().Ignore(p => p.NormalizedEmail);
        }*/
    }
}
