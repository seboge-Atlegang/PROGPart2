using System.Reflection.Metadata;
using CMCS.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<ClaimRecord> Claims { get; set; }
        public DbSet<ClaimDocument> ClaimDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ClaimRecord entity
            builder.Entity<ClaimRecord>(b =>
            {
                b.HasKey(c => c.Id);
                b.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                b.HasMany(c => c.Document)
                 .WithOne(d => d.Claim)
                 .HasForeignKey(d => d.ClaimId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}



