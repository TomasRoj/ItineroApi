using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("cities")]
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string best_time_to_visit { get; set; }
        public string Country { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public string? PhotoURL { get; set; }
    }
}
