namespace IndianArtVilla.Core.Exceptions;

public class UnauthorizedDomainException : DomainException
{
    public UnauthorizedDomainException(string message) : base(message) { }
}
