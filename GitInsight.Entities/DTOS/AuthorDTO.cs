namespace GitInsight.Entities.DTOS;
public record AuthorDTO{
    public string author {get; set;}
    public IEnumerable<FrequencyDTO> frequencies {get; set;} = new List<FrequencyDTO>();
}