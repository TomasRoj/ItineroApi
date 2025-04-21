using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("friends")]

    public class Friend
    {
        public int Id { get; set; }
        public int User_Id { get; set; } //id of current user
        public int Friend_Id { get; set; } // ID of the friend user
        public string? profile_picture { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime updated_at { get; set; } = DateTime.UtcNow;
    }
}
