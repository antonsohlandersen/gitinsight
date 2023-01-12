namespace GitInsight.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        string _conStr = @"
            Server=localhost,1433;
            Database=GitInsightDB4;
            User Id=SA;
            Password=<YourStrong@Passw0rd>;
            Trusted_Connection=False;
            Encrypt=False";

        var optionsBuilder = new DbContextOptionsBuilder<RepositoryContext>();
        optionsBuilder.UseSqlServer(_conStr);

        return new RepositoryContext(optionsBuilder.Options);
    }
}