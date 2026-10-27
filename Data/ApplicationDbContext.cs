using CampusConnect.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<ItemReport> ItemReports => Set<ItemReport>();
    public DbSet<ClaimRequest> Claims => Set<ClaimRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ItemReport>().HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ClaimRequest>().HasOne(c => c.ItemReport).WithMany(r => r.Claims).HasForeignKey(c => c.ItemReportId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ClaimRequest>().HasOne(c => c.Claimant).WithMany().HasForeignKey(c => c.ClaimantUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
