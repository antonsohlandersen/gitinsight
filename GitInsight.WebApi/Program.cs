namespace GitInsight.WebApi;
using GitInsight.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Identity.Web;

public class Program
{
    public static string githubApiKey;
    public static string connectionString;

    public static void Main(String[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        githubApiKey = builder.Configuration["Tokens:gitinsight"];
        connectionString = builder.Configuration["ConnectionString"];
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

        builder.Services.AddControllers();
        builder.Services.AddDbContext<RepositoryContext>(opt => opt.UseSqlServer(connectionString));
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IGitFetcher, GitFetcher>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin  
            .AllowCredentials());               // allow credentials 
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.MapControllers();

        //app.MapGet("/", () => githubApiKey);
        app.MapGet("/", () => connectionString);

        app.Run();
    }
}
