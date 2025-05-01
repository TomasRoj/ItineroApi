using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("expense_splits")]
    public class ExpenseSplit
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public bool IsSettled { get; set; }
        public DateTime? SettledAt { get; set; }
        public int trip_ID {  get; set; } 
    }
}
