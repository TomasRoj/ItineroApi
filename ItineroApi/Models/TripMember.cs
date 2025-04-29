using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("trip_members")]
    public class TripMember
    {
        public int Id { get; set; }
        public int trip_id { get; set; }
        public int user_id { get; set; }
        public string Role { get; set; } = "member";
        public DateTime joined_at { get; set; } = DateTime.UtcNow;
    }
}
