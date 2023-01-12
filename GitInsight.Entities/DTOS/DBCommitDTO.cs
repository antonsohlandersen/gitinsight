namespace GitInsight.Entities.DTOS;

public record DBLastCommitDTO(DateTime date);

public record DBReadCommitDTO(){

    public string author {get; set;}

    public DateTime date {get; set;}
};
