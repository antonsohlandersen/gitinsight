namespace GitInsight.Tests;
using LibGit2Sharp;
using System.IO;
using GitInsight;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class AbstractCommandTest : IDisposable
{
    private readonly RepositoryContext context;
    private readonly DBRepoRepository repoRepository;

    Repository repo;
    string path = @".\test-repo2\";

    public AbstractCommandTest(){
        Repository.Init(path);
        repo = new Repository(path);
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<RepositoryContext>();
        builder.UseSqlite(connection);
        context = new RepositoryContext(builder.Options);
        context.Database.EnsureCreated();
        repoRepository = new DBRepoRepository(context);
        //Adding first DBRepo to DB
    }

    [Fact]
    public void needsUpdate_should_return_true_after_new_commit_to_repo(){
        var commitDateTime1 = DateTimeOffset.Now;
        var author1 = new Signature("mlth", "mlth@itu.dk", commitDateTime1);
        var commitDateTime2 = DateTimeOffset.Now;
        var author2 = new Signature("aarv", "aarv@itu.dk", commitDateTime2);
        //Act
        repo.Commit("First commit", author1, author1, new CommitOptions(){ AllowEmptyCommit = true });
        var firstCommand = new FrequencyCommand(repo, context);
        firstCommand.processRepo();
        repo.Commit("Second commit", author2, author2, new CommitOptions(){ AllowEmptyCommit = true });
        var secondCommand = new FrequencyCommand(repo, context);
        secondCommand.needsUpdate().Should().BeTrue();
    }

    [Fact]
    public void needsUpdate_should_return_false_after_no_new_commit_to_repo(){
        var commitDateTime1 = DateTimeOffset.Now;
        var author1 = new Signature("mlth", "mlth@itu.dk", commitDateTime1);
        var commitDateTime2 = DateTimeOffset.Now;
        var author2 = new Signature("aarv", "aarv@itu.dk", commitDateTime2);

        //Act
        repo.Commit("First commit", author1, author1, new CommitOptions(){ AllowEmptyCommit = true });
        var firstCommand = new FrequencyCommand(repo, context);
        firstCommand.processRepo();
        var secondCommand = new FrequencyCommand(repo, context);
        secondCommand.needsUpdate().Should().BeFalse();
    }

    [Fact]
    public void getDBCommits_should_return_two_DBCommits(){
        var commitDateTime1 = DateTime.Now.Date;
        var author1 = new Signature("mlth", "mlth@itu.dk", commitDateTime1);
        var commitDateTime2 = DateTime.Now.Date;
        var author2 = new Signature("aarv", "aarv@itu.dk", commitDateTime2);
        repo.Commit("First commit", author1, author1, new CommitOptions(){ AllowEmptyCommit = true });
        var firstCommitId = repo.Commits.First().Sha;
        repo.Commit("Second commit", author2, author2, new CommitOptions(){ AllowEmptyCommit = true });
        var secondCommitId = repo.Commits.First().Sha;
        var command = new FrequencyCommand(repo, context);
        var dbcommits = command.getDBCommits();
        dbcommits.Should().BeEquivalentTo(new List<DBCommit>(){
            new DBCommit{Id = firstCommitId, author = "mlth", date = commitDateTime1.Date, repo = new DBRepository{
                                                                            Id = firstCommitId, 
                                                                            state = repo.Commits.First().Sha}},
            new DBCommit{Id = secondCommitId, author = "aarv", date = commitDateTime2.Date, repo = new DBRepository{
                                                                            Id = firstCommitId, 
                                                                            state = repo.Commits.First().Sha}}
            });
    }

    [Fact]
    public void needsUpdate_should_return_false_if_no_repo_exists_in_the_database(){
        var commitDateTime1 = DateTimeOffset.Now;
        var author1 = new Signature("mlth", "mlth@itu.dk", commitDateTime1);

        //Act
        repo.Commit("First commit", author1, author1, new CommitOptions(){ AllowEmptyCommit = true });
        var firstCommand = new FrequencyCommand(repo, context);
        firstCommand.needsUpdate().Should().BeFalse();
    }

    public void Dispose(){
        repo.Dispose();
        DeleteReadOnlyDirectory(path);
    }

    public void DeleteReadOnlyDirectory(string directoryPath)
    {
        foreach (var subdirectory in Directory.EnumerateDirectories(directoryPath)) 
        {
            DeleteReadOnlyDirectory(subdirectory);
        }
        foreach (var fileName in Directory.EnumerateFiles(directoryPath))
        {
            var fileInfo = new FileInfo(fileName);
            fileInfo.Attributes = FileAttributes.Normal;
            fileInfo.Delete();
        }
        Directory.Delete(directoryPath, true);
    }
}