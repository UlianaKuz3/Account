namespace AccountServices.Features
{
    public class NotFoundException(string message) : Exception(message);

}