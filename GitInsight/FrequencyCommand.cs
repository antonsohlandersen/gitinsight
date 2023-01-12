namespace GitInsight;
using LibGit2Sharp;
using GitInsight.Entities.DTOS;
using GitInsight.Entities;
using System.Text.Json;

public class FrequencyCommand : AbstractCommand {

    public ICollection<FrequencyDTO> frequencies = new List<FrequencyDTO>();

    public FrequencyCommand(Repository repo, RepositoryContext context) : base(repo, context){
        
    }
    public override void fetchData()
    {
        IEnumerable<DBReadCommitDTO> commits = DBRepository.ReadAllCommits(repoID)!;
        frequencies = (from c in commits
                    group c by c.date.Date into group1
                    select new FrequencyDTO{date = group1.Key, frequency = group1.Count()}).OrderBy(x => x.date.Date).ToList();
    }
    public override string getJsonString()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(frequencies, options);
        return jsonString;
    }
}
