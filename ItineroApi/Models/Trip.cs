using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("trips")]
    public class Trip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int creator_id { get; set; }
        public int Destination_city_id { get; set; }
        public DateTime Start_date { get; set; }
        public DateTime End_date { get; set; }
        public string? Description { get; set; }
        public bool Is_public { get; set; } = false;
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime updated_at { get; set; } = DateTime.UtcNow;
        public string? photoURL { get; set; } = null;
    }
}
