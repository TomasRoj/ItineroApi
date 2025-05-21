using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("expenses")]
    public class Expense
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Trip_Id { get; set; }
        public int? Category_Id { get; set; }
        public int paid_by_user_id { get; set; }
        public decimal Amount { get; set; }
        public string Currency_Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Receipt_image { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
