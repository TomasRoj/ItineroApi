using System.ComponentModel.DataAnnotations.Schema;

namespace ItineroApi.Models
{
    [Table("expense_categories")]
    public class ExpenseCategory
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
