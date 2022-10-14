namespace IssueTracker.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message) {}
}
public class InvalidException : Exception
{
    public InvalidException(string message)
        : base(message) {}
}

public class UnavailableException : Exception
{
    public UnavailableException(string message)
        : base(message) {}
}