using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("custom_destinations")]   
    public class CustomDestination
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
