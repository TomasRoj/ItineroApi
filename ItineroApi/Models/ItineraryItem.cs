using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("itinerary_items")]
    public class ItineraryItem
    {
        public int Id { get; set; }
        public int ItineraryDayId { get; set; }
        public int? AttractionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CustomLocation { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string EstimatedTime { get; set; }
        public int? SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
