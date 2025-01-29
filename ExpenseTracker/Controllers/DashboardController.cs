using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            // transactions last 7 days
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;
            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category) // for foreign key filled in
                .Where(t => t.Date >= StartDate && t.Date <= EndDate)
                .ToListAsync();

            // Debugging: Check the transactions retrieved
            foreach (var transaction in SelectedTransactions)
            {
                Console.WriteLine($"TransactionId: {transaction.TransactionId}, Amount: {transaction.Amount}, Category: {transaction.Category?.TransactionType}");
            }

            //total income transactions
            decimal TotalIncome = SelectedTransactions
                .Where(x => x.Category.TransactionType == "Income")
                .Sum(t => t.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("RM 0.00", new CultureInfo("ms-MY"));

            //total expense transactions
            decimal TotalExpense = SelectedTransactions
                .Where(x => x.Category.TransactionType == "Expense")
                .Sum(t => t.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("RM 0.00");

            //balance
            decimal Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("RM 0.00");

            return View();
        }
    }
}
