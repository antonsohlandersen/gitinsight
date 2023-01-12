namespace GitInsight.Entities.DTOS;

public record DBRepositoryDTO{
    public string Id {get; set;}

    public string state {get; set;}

    public ICollection<DBCommit> commits {get; set;}

}

public record DBUpdateRepositoryDTO{
    public string Id {get; set;}

    public string state {get; set;}

    public ICollection<DBCommit> commits {get; set;}

}

public record DBReadRepositoryDTO{

    public string state {get; set;}

    public ICollection<DBCommit> commits {get; set;}

}

