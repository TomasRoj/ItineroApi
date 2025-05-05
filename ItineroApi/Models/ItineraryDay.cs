using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("itinerary_days")]
    public class ItineraryDay
    {
        public int Id { get; set; }
        public int trip_id { get; set; }
        public DateTime date { get; set; }
        public string? description { get; set; }  
    }
}
