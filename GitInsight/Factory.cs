namespace GitInsight;
public class Factory{
    
    public static AbstractCommand getCommandAndIncjectDependencies(string mode, Repository repo, RepositoryContext context){
        switch (mode){
            case "frequency":
                return new FrequencyCommand(repo, context);
            case "author":
                return new AuthorCommand(repo, context);
            case "files":
                return new FilesCommand(repo, context);
            default:
                throw new NotImplementedException();
        }
    }

}