using System.ComponentModel.DataAnnotations;

namespace ExpensesApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        [Required]
        public string? Transaction_Description { get; set; }
        public DateTime? Date { get; set; }
        public string? Transaction_Type { get; set; }

    }
}
