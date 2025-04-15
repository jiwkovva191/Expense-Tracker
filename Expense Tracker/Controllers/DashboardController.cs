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

            //prepare data for the doughnut chart
            ViewBag.ExpenseCategories = expenceByCategory.Select(e=>e.Category).ToList(); //labels
            ViewBag.ExpenseAmounts = expenceByCategory.Select(e => e.TotalAmount).ToList(); //data



            //line chart - income vs expense
            //income

            var dailyIncome = SelectedTransactions
                .Where(t=>t.Category.Type=="Income")
                .GroupBy(t=>t.Date.Date)
                .Select(group=> new
                {
                    Date = group.Key,
                    TotalAmount = group.Sum(t => t.Amount)
                }) .ToList();

            var dailyExpense = SelectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Date.Date)
                .Select(group => new
                { 
                    Date = group.Key,
                    TotalAmount = group.Sum(t=>t.Amount)
                
                }).ToList();

            //generate a list for the last 30 days
            var last30Days = Enumerable.Range(0, 30)
                .Select(i => DateTime.Today.AddDays(-i))
                .ToList();
            //prepare data for the line chart
            var incomeData = last30Days
                .Select(date => dailyIncome.FirstOrDefault(d => d.Date == date)?.TotalAmount ?? 0)
                .ToList();

            var expenseData = last30Days
                .Select(date => dailyExpense.FirstOrDefault(d => d.Date == date)?.TotalAmount ?? 0)
                .ToList();

            ViewBag.Dates = last30Days.Select(d=>d.ToString("dd/MM/yyyy")).ToList();//x-axis
            ViewBag.IncomeData = incomeData; //y-axis
            ViewBag.ExpenseData = expenseData; //y-axis

            return View();
        }
    }
}
