using System.ComponentModel.DataAnnotations;

namespace GitInsight.Entities;

public record DBRepository
{
    public string Id { get; set; }

    public string state { get; set; }

    public ICollection<DBCommit> commits {get; set;}
}