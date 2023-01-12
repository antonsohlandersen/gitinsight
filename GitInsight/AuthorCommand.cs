namespace GitInsight;
using LibGit2Sharp;
using GitInsight.Entities.DTOS;
using GitInsight.Entities;
using System.Text.Json;
using System.Text.Encodings.Web;

public class AuthorCommand : AbstractCommand {

    public IEnumerable<AuthorDTO> authors {get; set;} = new List<AuthorDTO>();

    public AuthorCommand(Repository repo, RepositoryContext context) : base(repo, context){
        
    }

    public override void fetchData()
    {
        IEnumerable<DBReadCommitDTO> commits = DBRepository.ReadAllCommits(repoID);
        authors = from c in commits
                    group c by c.author into group1
                    select new AuthorDTO{author = group1.Key, frequencies = new List<FrequencyDTO>().Concat(
                        from c in group1
                        group c by c.date.Date into group2
                        select new FrequencyDTO {date = group2.Key, frequency = group2.Count()})};
    }

    public override string getJsonString()
    {
        var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        var jsonString = JsonSerializer.Serialize(authors, options);

        return jsonString;
    }
}