using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MeteoAlert.Data;
using MeteoAlert.Models;
using Microsoft.AspNetCore.Authorization;

namespace MeteoAlert
{
    [Authorize(Roles = "Admin,Manager")]
    public class RubricasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RubricasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rubricas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rubrica.ToListAsync());
        }

        // GET: Rubricas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rubrica = await _context.Rubrica
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rubrica == null)
            {
                return NotFound();
            }

            return View(rubrica);
        }

        // GET: Rubricas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rubricas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sms,Name,Matricola,Mansione,MobilePhone1,MobilePhone2,Email")] Rubrica rubrica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rubrica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rubrica);
        }

        // GET: Rubricas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rubrica = await _context.Rubrica.FindAsync(id);
            if (rubrica == null)
            {
                return NotFound();
            }
            return View(rubrica);
        }

        // POST: Rubricas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Sms,Name,Matricola,Mansione,MobilePhone1,MobilePhone2,Email")] Rubrica rubrica)
        {
            if (id != rubrica.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rubrica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RubricaExists(rubrica.Id))
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
            return View(rubrica);
        }

        // GET: Rubricas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rubrica = await _context.Rubrica
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rubrica == null)
            {
                return NotFound();
            }

            return View(rubrica);
        }

        // POST: Rubricas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rubrica = await _context.Rubrica.FindAsync(id);
            _context.Rubrica.Remove(rubrica);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RubricaExists(int id)
        {
            return _context.Rubrica.Any(e => e.Id == id);
        }
    }
}
