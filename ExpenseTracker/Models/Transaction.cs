using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; } // Foreign key for Category
        public Category? Category { get; set; } // Navigation property - nullable

        [Column(TypeName = "decimal(18,2)")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount should be more than 0")]
        public decimal Amount { get; set; } // Changed to decimal

        [Column(TypeName = "nvarchar(100)")]
        public string? Note { get; set; } // Note can be null

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcon // To display category name with icon in the view
        {
            get
            {
                return Category == null ? "" : Category.Icon + " " + Category.Title;
            }
        }

        [NotMapped]
        public string? FormattedAmount // To display amount in according to transaction type
        {
            get
            {
                return Category == null ? "" : (Category.TransactionType == "Expense" ? "- " : "+ ") + Amount.ToString("RM 0.00");
            }
        }
    }
}
