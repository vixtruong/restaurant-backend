using System.ComponentModel.DataAnnotations.Schema;


namespace Restaurant.Shared.Models
{
    public class Table
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public bool Available { get; set; }

        [NotMapped]
        public string? BookedBy { get; set; }

        public ICollection<TableHistory> TableHistories { get; set; } = new List<TableHistory>();
    }
}
