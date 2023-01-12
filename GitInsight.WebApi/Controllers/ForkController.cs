using Microsoft.AspNetCore.Mvc;
using LibGit2Sharp;
using GitInsight;
using Octokit;
using GitInsight.Entities;

namespace GitInsight.WebApi.Controllers;

// Repository identifier example
// https://localhost:7024/forks/Mlth/BDSAProject

[ApiController]
[Route("[controller]")]
public class ForkController : ControllerBase
{

    string githubApiKey = Program.githubApiKey;

    [HttpGet("{github_user}/{repository_name}")]
    public async Task<string> Get(string github_user, string repository_name)
    {
        var repositoryLink = "https://github.com/" + github_user + "/" + repository_name + ".git";
        var forkCommand = new ForkCommand();
        var jsonString =  await forkCommand.getJsonString(githubApiKey, github_user, repository_name);
        return jsonString;
        //deleteDirectory(repositoryLocation);

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