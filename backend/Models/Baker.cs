using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carlile_Cookie_Competition.Models
{
    [Table("Baker")]
    public class Baker
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("baker_name")]
        public string BakerName { get; set; } = "";

        [Column("has_voted")]
        public bool HasVoted { get; set; }

        // Foreign key to Cookie.id
        [Column("cookie_id")]
        public int? CookieId { get; set; }

        [ForeignKey("CookieId")]
        public Cookie? Cookie { get; set; }

        [Column("pin_hash")]
        public string? PinHash { get; set; }

        public Baker() { }

        public Baker(string bakerName, int? cookieId = null)
        {
            BakerName = bakerName;
            CookieId = cookieId;
            HasVoted = false;
            PinHash = null;
        }
    }
}
