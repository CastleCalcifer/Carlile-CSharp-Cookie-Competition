using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carlile_Cookie_Competition.Models
{
    [Table("Year")]
    public class YearRecord
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Year")]
        public int YearNumber { get; set; }

        [Column("ResultsViewable")]
        public bool ResultsViewable { get; set; } = false;
    }
}
