using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("attractions")]
    public class Attraction
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public int? city_id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? estimated_visit_time { get; set; }
        public decimal? Entrance_fee { get; set; }
        public string Currency_code { get; set; }
        public string Opening_hours { get; set; }
        public string Website { get; set; }
        public string Photo_url { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
