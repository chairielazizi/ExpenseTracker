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

            //donat chart data - expense category
            var donatChartData = SelectedTransactions
                .Where(x => x.Category.TransactionType == "Expense")
                .GroupBy(y => y.Category.CategoryId)
                .Select(z => new  // instead using separate class, use anonymous class
                {
                    categoryTitleWithIcon = z.First().Category.Icon + " " + z.First().Category.Title,
                    amount = z.Sum(y => y.Amount),
                    formattedAmount = z.Sum(y => y.Amount).ToString("C", new CultureInfo("ms-MY"))
                })
                .OrderByDescending(z => z.amount)
                .ToList();

            // Debugging: Check the donat chart data
            foreach (var data in donatChartData)
            {
                Console.WriteLine($"Category: {data.categoryTitleWithIcon}, Amount: {data.amount}, Formatted Amount: {data.formattedAmount}");
            }

            ViewBag.DonatChartData = donatChartData;

            //spline chart data - income vs expense
            //income
            List<SplineChartData> incomeChartData = SelectedTransactions
                .Where(x => x.Category.TransactionType == "Income")
                .GroupBy(y => y.Date)
                .Select(z => new SplineChartData
                {
                    day = z.Key.ToString("dd-MMM"),
                    income = z.Sum(y => y.Amount),
                    expense = 0
                })
                .OrderBy(z => z.day)
                .ToList();
            //expense
            List<SplineChartData> expenseChartData = SelectedTransactions
                .Where(x => x.Category.TransactionType == "Expense")
                .GroupBy(y => y.Date)
                .Select(z => new SplineChartData
                {
                    day = z.Key.ToString("dd-MMM"),
                    income = 0,
                    expense = z.Sum(y => y.Amount)
                })
                .OrderBy(z => z.day)
                .ToList();
            //merge
            string[] last7days = Enumerable.Range(0, 7)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in last7days
                                      join income in incomeChartData on day equals income.day into gj1
                                      from income in gj1.DefaultIfEmpty()
                                      join expense in expenseChartData on day equals expense.day into gj2
                                      from expense in gj2.DefaultIfEmpty()
                                      select new SplineChartData
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense
                                      };

            //recent transactions
            List<Transaction> recentTransactions = await _context.Transactions
                .Include(x => x.Category) // for foreign key filled in
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToListAsync();
            ViewBag.RecentTransactions = recentTransactions;

            return View();
        }
    }
    public class SplineChartData
    {
        public string day;
        public decimal income;
        public decimal expense;

    }
}
