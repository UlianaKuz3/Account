namespace AccountServices.Features.Entities
{
    public sealed class InboxDeadLetter
    {
        public Guid MessageId { get; set; }          
        public string? Handler { get; set; } = null;
        public DateTime ReceivedAt { get; set; }  
        public string? Payload { get; set; } = null;
        public string? Error { get; set; } = null;  
    }
}
