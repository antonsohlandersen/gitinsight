[Serializable]
public class NoCommitsException : Exception
{
    public string StudentName { get; }

    public NoCommitsException(){
        
    }

    public NoCommitsException(string message)
        : base(message) { }
}