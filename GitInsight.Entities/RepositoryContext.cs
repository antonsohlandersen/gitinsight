namespace GitInsight.Entities;
using Microsoft.EntityFrameworkCore;

public class RepositoryContext : DbContext
{
    public DbSet<DBCommit> CommitData { get; set; }

    public DbSet<DBRepository> RepoData { get; set; }

    public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options){
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DBCommit>(entity =>
            {
                entity.Property(e => e.author).HasMaxLength(100).IsRequired();
                entity.Property(e => e.date).IsRequired();
                entity.HasOne(s => s.repo).WithMany(s => s.commits);
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<DBRepository>(entity =>
            {
                entity.Property(e => e.state).HasMaxLength(50).IsRequired();
                entity.HasMany(s => s.commits).WithOne(s => s.repo);
                entity.HasKey(e => e.Id);
            });
        }
}