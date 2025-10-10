using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

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

        [Column("hasVoted")]
        public bool HasVoted { get; set; }

        // Foreign key to Cookie.id
        [Column("cookie_id")]
        public int? CookieId { get; set; }

        [ForeignKey("CookieId")]
        public Cookie? Cookie { get; set; }
        public string? PinHash { get; set; }

    }
}
