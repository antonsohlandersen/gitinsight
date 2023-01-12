namespace GitInsight.Entities.Tests;

public class DBRepoRepositoryTests : IDisposable
{
    private readonly RepositoryContext context;
    private readonly DBRepoRepository repository;

    public DBRepoRepositoryTests(){
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<RepositoryContext>();
        builder.UseSqlite(connection);
        context = new RepositoryContext(builder.Options);
        context.Database.EnsureCreated();
        DateTime date1 = DateTime.MinValue;
        repository = new DBRepoRepository(context);
        //Adding first DBRepo to DB
        repository.Create(new DBRepositoryDTO{Id = "Mlth/RepositoryName", state = "state", commits = new List<DBCommit>{new DBCommit{Id = "Id", author = "Mlth", date = date1, repo = new DBRepository{Id = "Mlth/RepositoryName", state = "state"}}}});
        context.SaveChanges();
    }

    [Fact]
    public void CreateNewRepoReturnsCreatedResponse(){
        //Arrange
        var repositoryDTO = new DBRepositoryDTO{Id = "Mlth/SecondRepositoryName", state = "state2"};

        //Act
        var response = repository.Create(repositoryDTO);

        //Assert
        Assert.Equal(Response.Created, response);
    }

    [Fact]
    public void CreateTwoIdenticalReposReturnsConflict(){
        var conflictRepositoryDTO = new DBRepositoryDTO{Id = "Mlth/RepositoryName", state = "state"};

        Response secondResponse = repository.Create(conflictRepositoryDTO);

        Assert.Equal(Response.Conflict, secondResponse);
    }

    [Fact]
    public void ReadValidRepoReturnsCorrectRepo(){
        var dto = repository.Read("Mlth/RepositoryName");
        Assert.Equal("state", dto.state);
        Assert.Equal("Id", dto.commits.First().Id);
    }

    [Fact]
    public void ReadInvalidRepoReturnsNull(){
        var dto = repository.Read("Mlth/InvalidRepositoryName");

        Assert.Null(dto);
    }
    
    [Fact]
    public void UpdateValidRepoReturnsUpdated(){
        var newDTO = new DBUpdateRepositoryDTO{Id = "Mlth/RepositoryName", state= "newState", commits = new List<DBCommit>{new DBCommit{Id = "id", author = "Mlth", date = DateTime.MinValue, repo = new DBRepository{Id = "Mlth/RepositoryName", state = "newState"}},
                                                                                                                new DBCommit{Id = "id2", author = "Mlth", date = DateTime.Now, repo = new DBRepository{Id = "Mlth/RepositoryName", state = "newState"}}

        }};
        Response response = repository.Update(newDTO);
        var entity = context.RepoData.Find(newDTO.Id);
        Assert.Equal(Response.Updated, response);
        Assert.Equal("newState", entity.state);
        Assert.Equal(2, entity.commits.Count());
        Assert.Equal("id2", entity.commits.Last().Id);
    }

    [Fact]
    public void read_last_commit_returns_latest_commit(){
        var entity = repository.ReadLastCommit(new DBRepositoryDTO {Id = "Mlth/RepositoryName"});
        Assert.Equal(DateTime.MinValue, entity.date);
        var newDate = DateTime.Now;
        var newDTO = new DBUpdateRepositoryDTO{Id = "Mlth/RepositoryName", state= "newState", commits = new List<DBCommit>{new DBCommit{Id = "id", author = "Mlth", date = DateTime.MinValue, repo = new DBRepository{Id = "Mlth/RepositoryName", state = "newState"}},
                                                                                                                new DBCommit{Id = "id2", author = "Mlth", date = newDate, repo = new DBRepository{Id = "Mlth/RepositoryName", state = "newState"}}
        }};
        repository.Update(newDTO);
        entity = repository.ReadLastCommit(new DBRepositoryDTO {Id = "Mlth/RepositoryName"});
        Assert.Equal(newDate, entity.date);
    }
    
    [Fact]
    public void UpdateInvalidRepoReturnsNotFound(){
        var newDTO = new DBUpdateRepositoryDTO{Id = "Mlth/InvalidRepositoryName", state= "newState"};
        Response response = repository.Update(newDTO);

        Assert.Equal(Response.NotFound, response);
    }

    public void Dispose() {
        context.Dispose();
    }

    //Test that correct commits are retrieved when reading, updating, and creating
}