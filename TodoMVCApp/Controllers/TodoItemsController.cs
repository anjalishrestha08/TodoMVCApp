using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TodoMVCApp.Data;
using TodoMVCApp.Models;

namespace TodoMVCApp.Controllers
{
    [Authorize]
    public class TodoItemsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TodoItemsController(AppDbContext context,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TodoItems
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            try
            {
                ViewData["TitleSortParam"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
                ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
                ViewData["StatusSortParam"] = sortOrder == "status" ? "status_desc" : "status";

                var userId = _userManager.GetUserId(User);
                var todos = _context.Todos.Where(t => t.UserId == userId);
                if (!string.IsNullOrEmpty(searchString))
                {
                    todos = todos.Where(t => t.Title.Contains(searchString) || (t.Description != null && t.Description.Contains(searchString)));
                }
                // Sorting logic
                switch (sortOrder)
                {
                    case "title_desc":
                        todos = todos.OrderByDescending(t => t.Title);
                        break;
                    case "date":
                        todos = todos.OrderBy(t => t.DueDate);
                        break;
                    case "date_desc":
                        todos = todos.OrderByDescending(t => t.DueDate);
                        break;
                    case "status":
                        todos = todos.OrderBy(t => t.IsDone);
                        break;
                    case "status_desc":
                        todos = todos.OrderByDescending(t => t.IsDone);
                        break;
                    default:
                        todos = todos.OrderBy(t => t.Title);
                        break;
                }
                return View(await todos.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading Todo List: " + ex.Message);
                TempData["ErrorMessage"] = "An error occured while loading your TODOs. Please try again later.";
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: TodoItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            var todoItem = await _context.Todos
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId==userId);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // GET: TodoItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TodoItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,DueDate,InProcess,IsDone")] TodoItem todoItem)
        {
            if (ModelState.IsValid)
            {
                todoItem.UserId = _userManager.GetUserId(User);
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (!todoItem.InProcess && todoItem.IsDone)
            {
                ModelState.AddModelError("", "A task cannot be marked as done unless it is in process.");
                return View(todoItem);
            }

            return View(todoItem);
        }

        // GET: TodoItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var todoItem = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (todoItem == null)
            {
                return NotFound();
            }
            return View(todoItem);
        }

        // POST: TodoItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,InProcess,IsDone")] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            // Ensure the task belongs to the current user
            var userId = _userManager.GetUserId(User);
            var existingItem = await _context.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Reassign user ID to avoid tampering
            todoItem.UserId = userId;


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (!todoItem.InProcess && todoItem.IsDone)
                {
                    ModelState.AddModelError("", "A task cannot be marked as done unless it is in process.");
                    return View(todoItem);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(todoItem);
        }

        // GET: TodoItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var todoItem = await _context.Todos
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // POST: TodoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var todoItem = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todoItem != null)
            {
                _context.Todos.Remove(todoItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TodoItemExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }
    }
}
