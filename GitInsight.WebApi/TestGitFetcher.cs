namespace GitInsight.WebApi;
using LibGit2Sharp;

//If changes are made to the cloneRepository method, remember to delete the two TestRepo folders in the repository folder, as they are not updated otherwise.
public class TestGitFetcher : IGitFetcher {
    public void cloneRepository(string repositoryPath, string newDir){
        var repo = new Repository(Repository.Init(newDir));

        var commitDateTime1 = new DateTime(2022, 10, 01);
        var author1 = new Signature("mlth", "mlth@itu.dk", commitDateTime1);
        var commitDateTime2 = new DateTime(2022, 10, 01);
        var author2 = new Signature("aarv", "aarv@itu.dk", commitDateTime2);
        var commitDateTime3 = new DateTime(2022, 10, 02);
        var author3 = new Signature("aarv", "aarv@itu.dk", commitDateTime3);

        repo.Commit("First commit", author1, author1, new CommitOptions(){ AllowEmptyCommit = true });
        repo.Commit("Second commit", author2, author2, new CommitOptions(){ AllowEmptyCommit = true });
        repo.Commit("Third commit", author3, author3, new CommitOptions(){ AllowEmptyCommit = true });
    }

    public void pullRepository(string repositoryPath){

    }
}