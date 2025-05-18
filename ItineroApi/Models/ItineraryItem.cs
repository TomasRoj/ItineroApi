using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("itinerary_items")]
    public class ItineraryItem
    {
        public int id { get; set; }
        public int itinerary_day_id { get; set; }
        public int? attraction_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string custom_location { get; set; }
        public decimal? latitude { get; set; }
        public decimal? longitude { get; set; }
        public string estimatedtime { get; set; }
        public int? sort_order { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
