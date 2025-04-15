using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("trip_members")]
    public class TripMember
    {
        public int id { get; set; }
        public int trip_id { get; set; }
        public int user_id { get; set; }
        public string role { get; set; }
        public DateOnly joined_at { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
