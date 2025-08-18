using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServices.Features.Entities
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }                 
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "char(50)")]
        public string? Type { get; set; } = null;
        [Column(TypeName = "char(50)")]
        public string? RoutingKey { get; set; } = null;
        [Column(TypeName = "char(50)")]
        public string? Payload { get; set; } = null;  
        public int Attempts { get; set; }
        public bool Processed { get; set; } = false;
        public DateTime? PublishedAt { get; set; }   
    }
}
