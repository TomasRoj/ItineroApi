using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("attractions")]
    public class Attraction
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public int? CityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? EstimatedVisitTime { get; set; }
        public decimal? EntranceFee { get; set; }
        public string CurrencyCode { get; set; }
        public string OpeningHours { get; set; }
        public string Website { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
