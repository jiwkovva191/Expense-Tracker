using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly Context _context;
        public DashboardController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //actions in the last 30 days
            DateTime StartDate = DateTime.Today.AddDays(-30);
            DateTime EndDate = DateTime.Today;
            List<Transaction> SelectedTransactions = await  _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            //total income
            double TotalIncome = SelectedTransactions
                .Where(t => t.Category.Type == "Income")
                .Sum(t => t.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString();

            //total expense
            double TotalExpense = SelectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .Sum(t => t.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString();

            //balance
            double TotalBalance = TotalIncome - TotalExpense;
            ViewBag.TotalBalance = TotalBalance.ToString();

            //doughnut chart - expense by category
            var expenceByCategory = SelectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.Title)
                .Select(group => new
                {
                    Category = group.Key,
                    TotalAmount = group.Sum(t => t.Amount)
                }).ToList();

            //prepare data for the chart
            ViewBag.ExpenseCategories = expenceByCategory.Select(e=>e.Category).ToList(); //labels
            ViewBag.ExpenseAmounts = expenceByCategory.Select(e => e.TotalAmount).ToList(); //data

            return View();
        }
    }
}
