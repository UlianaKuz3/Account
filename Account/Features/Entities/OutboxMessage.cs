namespace AccountServices.Features.Entities
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }                 
        public DateTime CreatedAt { get; set; }     
        public string? Type { get; set; } = null; 
        public string? RoutingKey { get; set; } = null;
        public string? Payload { get; set; } = null;  
        public string? CorrelationId { get; set; }
        public string? CausationId { get; set; }
        public int Attempts { get; set; }
        public bool Processed { get; set; } = false;
        public DateTime? PublishedAt { get; set; }   
        public string? LastError { get; set; }    
    }
}
