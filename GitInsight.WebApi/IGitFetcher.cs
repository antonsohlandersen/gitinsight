
public interface IGitFetcher {
    public void cloneRepository(string repositoryPath, string newDir);

    public void pullRepository(string repositoryPath);
}