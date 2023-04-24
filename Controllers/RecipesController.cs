using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using recipes_app.Data;
using recipes_app.Models;

namespace recipes_app.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RecipesModels
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Recipes.Include(r => r.Author);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RecipesModels/Details/5
        public async Task<IActionResult> Details(string id)
        {
      
            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipesModel = await _context.Recipes
                .Include(r => r.Author)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipesModel == null)
            {
                return NotFound();
            }

            return View(recipesModel);
        }

        // GET: RecipesModels/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: RecipesModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeId,Title,Content,AuthorId")] RecipesModel recipesModel)
        {
            if(Request.Cookies["user"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                recipesModel.CreatedDate = DateTime.Now;
                recipesModel.AuthorId = Request.Cookies["user"];
                _context.Add(recipesModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
               
        }

        // GET: RecipesModels/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            if (Request.Cookies["user"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var recipesModel = await _context.Recipes.FindAsync(id);
            if (recipesModel == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "UserId", recipesModel.AuthorId);
            return View(recipesModel);
        }

        // POST: RecipesModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RecipeId,Title,Content,CreatedDate,AuthorId")] RecipesModel recipesModel)
        {
            if (id != recipesModel.RecipeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipesModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipesModelExists(recipesModel.RecipeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "UserId", recipesModel.AuthorId);
            return View(recipesModel);
        }

        // GET: RecipesModels/Delete/5
        public async Task<IActionResult> Delete(string id)
        {

            if (Request.Cookies["user"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipesModel = await _context.Recipes
                .Include(r => r.Author)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipesModel == null)
            {
                return NotFound();
            }

            return View(recipesModel);
        }

        // POST: RecipesModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Recipes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Recipes'  is null.");
            }
            var recipesModel = await _context.Recipes.FindAsync(id);
            if (recipesModel != null)
            {
                _context.Recipes.Remove(recipesModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipesModelExists(string id)
        {
          return (_context.Recipes?.Any(e => e.RecipeId == id)).GetValueOrDefault();
        }
    }
}
