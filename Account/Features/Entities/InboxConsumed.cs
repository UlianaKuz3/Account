namespace AccountServices.Features.Entities
{
    public sealed class InboxConsumed
    {
        public Guid MessageId { get; set; }      
        public string? Handler { get; set; } = null;
        public DateTime ProcessedAt { get; set; }   
    }
}
