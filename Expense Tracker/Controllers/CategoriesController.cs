﻿using System.Reflection;
using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Controllers
{
    public class CategoriesController : Controller
    {

        //here we need the db context
        private readonly Context _context;

        public CategoriesController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Create([Bind("CategoryId, Title, Icon, Type")] Category category)
        {
            if(ModelState.IsValid)
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult>Edit(int id, [Bind("CategoryId, Title, Icon, Type")] Category category)
        {
            if(ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


    }
}
