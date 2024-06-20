using LaytonYSAClerk.Cli.Entities;
using Microsoft.EntityFrameworkCore;

namespace LaytonYSAClerk.Cli;

public class ChurchDbContext : DbContext
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Unit> MemberUnits { get; set; }

    public ChurchDbContext() { }
    public ChurchDbContext(DbContextOptions<ChurchDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Member>().ToTable("members");
        modelBuilder.Entity<Unit>().ToTable("units");
        
        modelBuilder.Entity<Member>()
           .HasKey(m => m.MemberId);

        modelBuilder.Entity<Unit>()
           .HasKey(u => u.UnitNumber);
        modelBuilder.Entity<Unit>()
          .HasIndex(u => u.MemberId);

        modelBuilder.Entity<Member>()
           .HasOne<Unit>(m => m.Unit)
           .WithOne(u => u.Member);
    }
}