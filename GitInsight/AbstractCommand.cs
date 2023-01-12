using System.Text.RegularExpressions;
using GitInsight.Entities.DTOS;

namespace GitInsight;
public abstract class AbstractCommand {
    public String repoID {get; init;}
    public Repository libGitRepo {get; init;}
    public RepositoryContext context {get; init;}
    public DBRepoRepository DBRepository {get; init;}

    public AbstractCommand(Repository repo, RepositoryContext context){
        if (!repo.Commits.Any()){
            throw new NoCommitsException("The repository contains no commits");
        }
        repoID = repo.Commits.LastOrDefault().Sha;
        libGitRepo = repo;
        this.context = context;
        DBRepository = new DBRepoRepository(context);
    }
    
    public void processRepo(){
        if (repoExists()){
            if (needsUpdate()){
                analyseRepoAndUpdate();
            }
        }
        else{
            createRepo();
        }
        fetchData();
    }

    public bool createRepo()
    {
        return Response.Created == DBRepository.Create(new DBRepositoryDTO{Id = repoID, state = libGitRepo.Commits.First().Sha, commits = getDBCommits()});
    }

    public ICollection<DBCommit> getDBCommits(){
        return (from c in libGitRepo.Commits
                        select new DBCommit{Id = c.Sha, author = c.Author.Name, date = c.Author.When.DateTime, repo = new DBRepository{Id = repoID, state = libGitRepo.Commits.First().Sha}}).ToList();
    }

    public void analyseRepoAndUpdate(){
        DBRepository.Update(new DBUpdateRepositoryDTO{Id = repoID, state = libGitRepo.Commits.First().Sha, commits = getDBCommits()});
    }
    public abstract void fetchData();
    public bool needsUpdate(){
        var entity = DBRepository.Read(repoID);
        if (entity is not null){
            if (libGitRepo.Commits.First().Sha == entity.state){
                return false;
            }
            return true;
        }
        return false;
    }

    public bool repoExists(){
        var entity = DBRepository.Read(repoID);
        if (entity is null){
            return false;
        }
        return true;
    }
    public abstract string getJsonString();
}
