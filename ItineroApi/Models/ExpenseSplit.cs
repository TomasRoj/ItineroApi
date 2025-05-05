using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("expense_splits")]
    public class ExpenseSplit
    {
        public int Id { get; set; }
        public int Expense_id { get; set; }
        public int User_Id { get; set; }
        public decimal Amount { get; set; }
        public bool Is_Settled { get; set; }
        public DateTime? Settled_At { get; set; }
        public int trip_ID {  get; set; } 
    }
}
