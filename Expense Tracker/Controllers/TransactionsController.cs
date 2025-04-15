using Expense_Tracker.Models;
using Expense_Tracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var transactions = await _context.Transactions
                .Include(t=>t.Category)
                .ToListAsync();
            return View(transactions);
        }

        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Title");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("TransactionId, CategoryId, Amount, Note, Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            //re-populating the dropdown list when the model state is invalid
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Title");
            return View(transaction);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Title");
            var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id ,[Bind("TransactionId, CategoryId, Amount, Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            //re-populating the dropdown list when the model state is invalid
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Title");
            return View(transaction);
        }
    }
}
