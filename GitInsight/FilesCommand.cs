namespace GitInsight;
using LibGit2Sharp;
using GitInsight.Entities.DTOS;
using GitInsight.Entities;
using System.Text.Json;
using System.Text.Encodings.Web;

public class FilesCommand : AbstractCommand {

    int commitCounter;

    public List<FileDTO> commitChanges {get; set;} = new List<FileDTO>();

    public FilesCommand(Repository repo, RepositoryContext context) : base(repo, context){
        
    }

    public void CompareTrees(LibGit2Sharp.Commit c, LibGit2Sharp.Repository repo){
        commitCounter++;
        if(commitCounter>250){
            return;
        }
        if(c.Parents.ToArray().Length == 0){
            return;
        }
        List<string> changes = new List<string>();
        Tree commitTree = c.Tree; // Main Tree
        Commit parentCommit = c.Parents.First();
        Tree parentCommitTree = c.Parents.First().Tree; // Secondary Tree

        var patch = repo.Diff.Compare<Patch>(parentCommitTree, commitTree); // Difference

        foreach (var ptc in patch)
        {
            changes.Add(ptc.Status +" -> "+ptc.Path);
        }
        FileDTO fileDTO = new FileDTO {identifier = c.Author.Name + " on " + c.Author.When.DateTime.ToString(), changes = changes};
        commitChanges.Add(fileDTO);
        CompareTrees(parentCommit, repo);
    }

    public override void fetchData()
    {
        CompareTrees(libGitRepo.Head.Tip, libGitRepo);
    }

    public override string getJsonString()
    {
        var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        var jsonString = JsonSerializer.Serialize(commitChanges, options);

        return jsonString;
    }
}