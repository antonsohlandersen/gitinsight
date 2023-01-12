namespace GitInsight;
using Octokit;
using GitInsight.Entities.DTOS;
using System.Text.Json;

public class ForkCommand {

    GitHubClient client;
    Octokit.Credentials credentials;

    public async Task<IEnumerable<ForkDTO>> analyzeForks(string githubApiKey, string username, string repoName){
        var productInformation = new ProductHeaderValue("luel");
        if(githubApiKey == null || githubApiKey.Length == 0){
            client = new GitHubClient(productInformation);
        } else {
            credentials = new Octokit.Credentials(githubApiKey);
            client = new GitHubClient(productInformation) { Credentials = credentials };
        }

        IReadOnlyList<Octokit.Repository> allForks = await client.Repository.Forks.GetAll(username, repoName);
        return from f in allForks select new ForkDTO{url = f.Url};
    }

    public async Task<string> getJsonString(string githubApiKey, string username, string repoName)
    {
        var forks = await analyzeForks(githubApiKey, username, repoName);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(forks, options);
        return jsonString;
    }
}