using Expense_Tracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Controllers
{
    public class TransactionsController : Controller
    {
        //here we need the dbContext
        private readonly Context _context;
        public TransactionsController(Context context)
        {
            _context = context;
        }
      public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return View(transactions);
        }
    }
}
