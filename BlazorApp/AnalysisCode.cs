namespace BlazorApp;
using GitInsight.Entities.DTOS;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;

public sealed class AnalysisCode
{
    private static AnalysisCode instance = null;
    private static readonly object padlock = new object();
    public bool isItAuthor { get; set; }
    public bool isItFrequency { get; set; }
    public bool isItFork { get; set; }
    public bool isItFile { get; set; }
    public bool active { get; set; }
    public string? repository { get; set; }
    public AuthorDTO[] authorAnalysis { get; set; }
    public AuthorObject[] authorObjects { get; set; }
    public FrequencyDTO[] frequencyAnalysis { get; set; }
    public ForkDTO[] forkAnalysis { get; set; }
    public FileDTO[] fileAnalysis { get; set; }

    private AnalysisCode() 
    {
        authorAnalysis = new AuthorDTO[] {};
        authorObjects = new AuthorObject[] {};
        frequencyAnalysis = new FrequencyDTO[] {};
        forkAnalysis = new ForkDTO[] {}; 
        fileAnalysis = new FileDTO[] {};
    }

    public static AnalysisCode Instance 
    {
        get
        {
            lock(padlock)
            {
                if(instance == null)
                {
                    instance = new AnalysisCode();
                }
                return instance;
            }
        }
    }

    public void OnChange(string newRepository)
    {
        repository = newRepository;
    }

    public async Task getAuthorAnalysis(HttpClient client)
    {
        isItAuthor = true;
        isItFrequency = false;
        isItFork = false;
        isItFile = false;

        if(!active && repository != null) {
            active = true; 
            authorAnalysis = await client.GetFromJsonAsync<AuthorDTO[]>("https://localhost:7024/analysis/" + repository + "/author");
            active = false;
            authorObjects = convertToAuthorObjects();
        }
    }

    public async Task getFrequencyAnalysis(HttpClient client)
    {
        isItFrequency = true;
        isItAuthor = false;
        isItFork = false;
        isItFile = false;

        if(!active && repository != null) 
        {
            active = true;
            frequencyAnalysis = await
            client.GetFromJsonAsync<FrequencyDTO[]>("https://localhost:7024/analysis/" + repository + "/frequency");
            active = false;
        }
    }

    public async Task getForkAnalysis(HttpClient client)
    {
        isItFork = true;
        isItFrequency = false;
        isItAuthor = false;
        isItFile = false;

        if(!active && repository != null) 
        {
            active = true;
            forkAnalysis = await
            client.GetFromJsonAsync<ForkDTO[]>("https://localhost:7024/fork/" + repository);
            active = false;
        }
    }

    public async Task getFileAnalysis(HttpClient client)
    {
        isItFile = true;
        isItFork = false;
        isItFrequency = false;
        isItAuthor = false;

        if(!active && repository != null)
        {
            active = true;
            fileAnalysis = await 
            client.GetFromJsonAsync<FileDTO[]>("https://localhost:7024/analysis/" + repository + "/files");
            active = false;
        }
    }

    private AuthorObject[] convertToAuthorObjects() 
    {
        var authors = new AuthorObject[authorAnalysis.Length];
        for(int i = 0; i < authorAnalysis.Length; i++) 
        {
            AuthorObject author = new AuthorObject{author = authorAnalysis[i].author, frequency = authorAnalysis[i].frequencies.Sum(item => item.frequency)};
            authors[i] = author;
        }
        return authors;
    }

    public record AuthorObject 
    {
        public string? author {get; set;}
        public int frequency {get; set;}
    }
}