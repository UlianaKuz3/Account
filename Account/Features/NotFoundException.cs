namespace AccountService.Features
{
    public class NotFoundException(string message) : Exception(message)
    {
    }

}