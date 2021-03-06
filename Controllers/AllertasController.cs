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
using MeteoAlert.Services;
using Microsoft.Extensions.Logging;

namespace MeteoAlert
{
    [Authorize(Roles = "Admin,Manager")]
    public class AllertasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ElaborazioniDati _elaborazioniDati;
        private readonly ILogger<AllertasController> _logger;

        public AllertasController(ApplicationDbContext context, ElaborazioniDati elaborazioniDati, ILogger<AllertasController> logger)
        {
            _context = context;
            _elaborazioniDati = elaborazioniDati;
            _logger = logger;
        }
 
        // GET: Allertas
      
        public async Task<IActionResult> Index()
        {
            DateTime ultimadata = _context.Allerta.OrderByDescending(s => s.DateSent).First().DateSent;
            var allerte = await _context.Allerta.Where(s => s.DateSent == ultimadata).ToListAsync();
            

            ViewBag.Grafico = _elaborazioniDati.graficoAllerta(allerte);
            
            foreach (var giorno in ViewBag.Grafico)
            {
                foreach (var ora in giorno)
                {
                    if (ora[2] == "Allerta")
                    {
                        _logger.LogInformation($"Rilevata allerta { ora[0]} { ora[1]}");
                    }
                }
            }

            return View(allerte.OrderBy(s => s.HourOfDay));
        }

        // GET: Allertas/Details/5
        
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allerta = await _context.Allerta
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allerta == null)
            {
                return NotFound();
            }

            return View(allerta);
        }

        //// GET: Allertas/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Allertas/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Data,Ora,Prectot3h,Cp1h,Ws10m,Ws32m1,Ws32m2,Ws57m1,Ws57m2,Sigww,DateSent")] Allerta allerta)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(allerta);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(allerta);
        //}

        //// GET: Allertas/Edit/5
        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var allerta = await _context.Allerta.FindAsync(id);
        //    if (allerta == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(allerta);
        //}

        //// POST: Allertas/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("Id,Data,Ora,Prectot3h,Cp1h,Ws10m,Ws32m1,Ws32m2,Ws57m1,Ws57m2,Sigww,DateSent")] Allerta allerta)
        //{
        //    if (id != allerta.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(allerta);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AllertaExists(allerta.Id))
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
        //    return View(allerta);
        //}

        //// GET: Allertas/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var allerta = await _context.Allerta
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (allerta == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(allerta);
        //}

        //// POST: Allertas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    var allerta = await _context.Allerta.FindAsync(id);
        //    _context.Allerta.Remove(allerta);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool AllertaExists(long id)
        {
            return _context.Allerta.Any(e => e.Id == id);
        }
    }
}
