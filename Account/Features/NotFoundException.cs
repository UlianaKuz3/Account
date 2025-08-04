namespace Account.Features
{
    public class NotFoundException(string message) : Exception(message);

}