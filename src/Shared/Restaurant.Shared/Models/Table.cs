using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Shared.Models
{
    public class Table
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public bool Available { get; set; }

        public ICollection<TableHistory> TableHistories { get; set; } = new List<TableHistory>();
    }
}
