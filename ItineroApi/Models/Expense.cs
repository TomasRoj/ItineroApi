using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("expenses")]
    public class Expense
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TripId { get; set; }
        public int? CategoryId { get; set; }
        public int PaidByUserId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string ReceiptImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
