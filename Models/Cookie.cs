using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carlile_Cookie_Competition.Models
{
    [Table("Cookie")]
    public class Cookie
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("cookie_name")]
        public string CookieName { get; set; } = "";

        [Column("score")]
        public int Score { get; set; }

        [Column("baker_name")]
        public string BakerName { get; set; } = "";

        [Column("year")]
        public int Year { get; set; }

        [Column("image")]
        public string Image { get; set; } = "";

        [Column("creative_points")]
        public int CreativePoints { get; set; }

        [Column("presentation_points")]
        public int PresentationPoints { get; set; }

        public ICollection<Baker>? Bakers { get; set; }

        public Cookie(string cookieName, int year, string image, string bakerName)
        {
            CookieName = cookieName;
            Year = year;
            Image = image;
            BakerName = bakerName;
            Score = 0;
            CreativePoints = 0;
            PresentationPoints = 0;
        }
    }
}
