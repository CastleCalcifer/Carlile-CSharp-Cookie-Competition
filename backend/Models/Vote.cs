// Models/Vote.cs

namespace Carlile_Cookie_Competition.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public int CookieId { get; set; }
        public Cookie? Cookie { get; set; }
        public string? VoterId { get; set; } // optional
        public int Points { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
