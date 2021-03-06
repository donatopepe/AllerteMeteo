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
using Org.BouncyCastle.Crypto;
using Microsoft.Extensions.Logging;

namespace MeteoAlert
{
    [AllowAnonymous]
    public class BollettinoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ElaborazioniDati _elaborazioniDati;
        private readonly ILogger<BollettinoesController> _logger;

        public BollettinoesController(ApplicationDbContext context, ElaborazioniDati elaborazioniDati, ILogger<BollettinoesController> logger)
        {
            _context = context;
            _elaborazioniDati = elaborazioniDati;
            _logger = logger;
        }
        
        // GET: Bollettinoes
        public async Task<IActionResult> Index(bool inAlarm)
        {
            ViewData["InAlarm"] = inAlarm;
            DateTime ultimadata = _context.Bollettino.Include(s => s.EmailMessage).OrderByDescending(s => s.DateSent).First().DateSent;
            var bollettini = await _context.Bollettino.Where(s => s.DateSent == ultimadata).ToListAsync();

            var query = bollettini.OrderByDescending(s => s.Scadh).ThenBy(s => s.Sito).ToList();
            int[] max = new int[3];
            max[0] = bollettini.Max(row => row.Wind10m_kmh);
            max[1] = bollettini.Max(row => row.Wind32m_kmh);
            max[2] = bollettini.Max(row => row.Wind57m_kmh);

            ViewBag.max_vento = max.Max();
            ViewBag.Subject = query.FirstOrDefault().EmailMessage.Subject;

            _logger.LogInformation($"max value Wind10m_kmh: {max[0]} Wind32m_kmh: {max[1]} Wind57m_kmh: {max[2]} Max: {max.Max()} Subject email: {query.FirstOrDefault().EmailMessage.Subject}");



            List <Bollettino> items = _elaborazioniDati.bollettiniAllarme(query);

            ViewBag.Grafico = _elaborazioniDati.graficoBollettino(query, items);

            if (inAlarm)
            {
                //ViewBag.Grafico = myList;
                //return View(items);
                query = items;
            }


            return View(query);
        }
        
        // GET: Bollettinoes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bollettino = await _context.Bollettino
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bollettino == null)
            {
                return NotFound();
            }

            return View(bollettino);
        }

        //// GET: Bollettinoes/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Bollettinoes/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Sito,Latitudine,Longitudine,Elevazione,Scadh,Tp3h,Tp3h30mm,Cp1h,Cp1h20mm,Wind10m,Wind10m1544ms,Wind32m,Wind32m1667ms,Wind32m20ms,Wind57m,Wind57m1667ms,Wind57m20ms,Ww,Phenomena,DateSent")] Bollettino bollettino)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(bollettino);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(bollettino);
        //}

        //// GET: Bollettinoes/Edit/5
        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var bollettino = await _context.Bollettino.FindAsync(id);
        //    if (bollettino == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(bollettino);
        //}

        //// POST: Bollettinoes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("Id,Sito,Latitudine,Longitudine,Elevazione,Scadh,Tp3h,Tp3h30mm,Cp1h,Cp1h20mm,Wind10m,Wind10m1544ms,Wind32m,Wind32m1667ms,Wind32m20ms,Wind57m,Wind57m1667ms,Wind57m20ms,Ww,Phenomena,DateSent")] Bollettino bollettino)
        //{
        //    if (id != bollettino.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(bollettino);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BollettinoExists(bollettino.Id))
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
        //    return View(bollettino);
        //}

        //// GET: Bollettinoes/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var bollettino = await _context.Bollettino
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (bollettino == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(bollettino);
        //}

        //// POST: Bollettinoes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    var bollettino = await _context.Bollettino.FindAsync(id);
        //    _context.Bollettino.Remove(bollettino);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool BollettinoExists(long id)
        //{
        //    return _context.Bollettino.Any(e => e.Id == id);
        //}
    }
}
