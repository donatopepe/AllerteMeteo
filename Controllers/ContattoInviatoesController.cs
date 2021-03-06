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
    public class ContattoInviatoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContattoInviatoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ContattoInviatoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ContattoInviato.OrderByDescending(s=>s.DateSent).ToListAsync());
        }

        // GET: ContattoInviatoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contattoInviato = await _context.ContattoInviato
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contattoInviato == null)
            {
                return NotFound();
            }

            return View(contattoInviato);
        }

        //// GET: ContattoInviatoes/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: ContattoInviatoes/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Risultato1,Risultato2,Testosms,DateSent,Name,Matricola,Mansione,MobilePhone1,MobilePhone2,Email")] ContattoInviato contattoInviato)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(contattoInviato);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(contattoInviato);
        //}

        //// GET: ContattoInviatoes/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var contattoInviato = await _context.ContattoInviato.FindAsync(id);
        //    if (contattoInviato == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(contattoInviato);
        //}

        //// POST: ContattoInviatoes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Risultato1,Risultato2,Testosms,DateSent,Name,Matricola,Mansione,MobilePhone1,MobilePhone2,Email")] ContattoInviato contattoInviato)
        //{
        //    if (id != contattoInviato.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(contattoInviato);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ContattoInviatoExists(contattoInviato.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(contattoInviato);
        //}

        //// GET: ContattoInviatoes/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var contattoInviato = await _context.ContattoInviato
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (contattoInviato == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(contattoInviato);
        //}

        //// POST: ContattoInviatoes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var contattoInviato = await _context.ContattoInviato.FindAsync(id);
        //    _context.ContattoInviato.Remove(contattoInviato);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ContattoInviatoExists(int id)
        //{
        //    return _context.ContattoInviato.Any(e => e.Id == id);
        //}
    }
}
