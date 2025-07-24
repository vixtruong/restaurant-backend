using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Shared.Models
{
    public class TableHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TableId { get; set; }

        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public User User { get; set; } = null!;

        public Table Table { get; set; } = null!;
    }
}
