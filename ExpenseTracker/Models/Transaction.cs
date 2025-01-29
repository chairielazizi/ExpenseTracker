using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int CategoryId { get; set; } // Foreign key for Category
        public Category? Category { get; set; } // Navigation property - nullable

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } // Changed to decimal

        [Column(TypeName = "nvarchar(100)")]
        public string? Note { get; set; } // Note can be null

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
