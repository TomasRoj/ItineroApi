using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("users")]
    public class User
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string password_hash { get; set; }
        public string? profile_picture { get; set; }
        public string preferedcurrency { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime updated_at { get; set; } = DateTime.UtcNow;

    }
}
