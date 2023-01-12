namespace GitInsight.Entities.DTOS;

public record FileDTO{
    
    public string? identifier { get; set; }
    public IEnumerable<string>? changes { get; set; }
}