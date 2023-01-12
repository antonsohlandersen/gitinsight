namespace GitInsight.WebApi;
using LibGit2Sharp;

public class GitFetcher : IGitFetcher {
    public void cloneRepository(string repositoryPath, string newDir){
        Repository.Clone(repositoryPath, newDir);
    }

    public void pullRepository(string repositoryPath){
        var repository = new Repository(repositoryPath);
        var signature = new Signature("Lukas", "luel@itu.dk", DateTime.Now);
        Commands.Pull(repository, signature, null);
    }
}