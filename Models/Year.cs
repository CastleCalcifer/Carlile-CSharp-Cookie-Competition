using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carlile_Cookie_Competition.Models
{
    [Table("Year")]
    public class YearRecord
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("year")]
        public int YearNumber { get; set; }

        [Column("resultsViewable")]
        public bool ResultsViewable { get; set; } = false;
    }
}
