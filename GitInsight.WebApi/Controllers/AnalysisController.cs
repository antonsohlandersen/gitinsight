using Microsoft.AspNetCore.Mvc;
using LibGit2Sharp;
using GitInsight;
using Octokit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using GitInsight.Entities;

namespace GitInsight.WebApi.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes")]
public class AnalysisController : ControllerBase
{
    private readonly RepositoryContext context;
    private readonly IGitFetcher fetcher;

    string githubApiKey = Program.githubApiKey;

    public AnalysisController(RepositoryContext context, IGitFetcher fetcher){
        this.context = context;
        this.fetcher = fetcher;
    }

    [HttpGet("{github_user}/{repository_name}/{command}")]
    public async Task<string> Get(string github_user, string repository_name, string command)
    {
        var repositoryLink = "https://github.com/" + github_user + "/" + repository_name + ".git";
        var currentPath = Directory.GetCurrentDirectory();

        var repositories = Directory.GetParent(currentPath) + "/Repositories/";
        if(!Directory.Exists(repositories)){
            Directory.CreateDirectory(repositories);
        }
        var repositoryLocation = repositories + repository_name;
        if(Directory.GetDirectories(repositories, repository_name).Length < 1){
            fetcher.cloneRepository(repositoryLink, repositoryLocation);
        } else {
            fetcher.pullRepository(repositoryLocation);
        }
        var repo = new LibGit2Sharp.Repository(repositoryLocation);
        var chosenCommand = Factory.getCommandAndIncjectDependencies(command, repo, context);
        chosenCommand.processRepo();
        var jsonString = chosenCommand.getJsonString();
        repo.Dispose();

        //deleteDirectory(repositoryLocation);
        
        return jsonString;
    }

    public static void deleteDirectory(string path)
    {
        foreach (var subdirectory in Directory.EnumerateDirectories(path))
        {
            deleteDirectory(subdirectory);
        }
        foreach (var fileName in Directory.EnumerateFiles(path))
        {
            var fileInfo = new FileInfo(fileName);
            fileInfo.Attributes = FileAttributes.Normal;
            fileInfo.Delete();
        }
        var dir = new DirectoryInfo(path);
        dir.Delete(true);
    }
}