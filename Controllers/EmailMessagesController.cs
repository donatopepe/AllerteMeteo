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
    public class EmailMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ElaborazioniDati _elaborazioniDati;
        private readonly ILogger<EmailMessagesController> _logger;

        public EmailMessagesController(ApplicationDbContext context, ElaborazioniDati elaborazioniDati, ILogger<EmailMessagesController> logger)
        {
            _context = context;
            _elaborazioniDati = elaborazioniDati;
            _logger = logger;
        }

        
        // GET: EmailMessages
        public async Task<IActionResult> Index(DateTime? searchDate)
        {
            //ViewData["SerachDate"] = searchDate;
            if (searchDate.HasValue)
            {
                _logger.LogInformation($"{ searchDate}");
                return View(await _context.EmailMessages.Where(s => s.DateSent.Date == searchDate.Value.Date).OrderByDescending(s => s.DateSent).ToListAsync());
            }
            return View(await _context.EmailMessages.OrderByDescending(s => s.DateSent).Take(50).ToListAsync());
        }

        
        // GET: EmailMessages/Details/5
        public async Task<IActionResult> Details(int? id, bool inAlarm)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["InAlarm"] = inAlarm;
            ViewBag.Bollettino = false;
            ViewBag.Allerta = false;
            var emailMessage = await _context.EmailMessages.Include(s => s.Bollettinos).Include(s => s.Allertas).FirstOrDefaultAsync(m => m.Id == id);


            if (emailMessage == null)
            {
                return NotFound();
            }


            var queryAllerta = emailMessage.Allertas.OrderBy(s => s.HourOfDay).ToList();
            if (queryAllerta.Count() > 0)
            {
                ViewBag.Allerta = true;
                ViewBag.GraficoAllerta = _elaborazioniDati.graficoAllerta(queryAllerta);
            }


            int[] max = new int[3];
            var query = emailMessage.Bollettinos.OrderByDescending(s => s.Scadh).ThenBy(s => s.Sito).ToList();
            if (query.Count > 0)
            {
                max[0] = query.Max(row => row.Wind10m_kmh);
                max[1] = query.Max(row => row.Wind32m_kmh);
                max[2] = query.Max(row => row.Wind57m_kmh);
            }
            

            ViewBag.max_vento = max.Max();
            ViewBag.Subject = emailMessage.Subject;

            List<Bollettino> items = _elaborazioniDati.bollettiniAllarme(query);
            if (query.Count() > 0)
            {
                ViewBag.Bollettino = true;
                ViewBag.Grafico = _elaborazioniDati.graficoBollettino(query, items);
            }

            if (inAlarm)
            {
                emailMessage.Bollettinos = items;
            }

           
            return View(emailMessage);
        }

        //// GET: EmailMessages/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: EmailMessages/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,MessageNumber,ToAddresses,FromAddresses,Subject,Content,DateSent")] EmailMessage emailMessage)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(emailMessage);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(emailMessage);
        //}

        //// GET: EmailMessages/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var emailMessage = await _context.EmailMessages.FindAsync(id);
        //    if (emailMessage == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(emailMessage);
        //}

        //// POST: EmailMessages/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,MessageNumber,ToAddresses,FromAddresses,Subject,Content,DateSent")] EmailMessage emailMessage)
        //{
        //    if (id != emailMessage.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(emailMessage);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!EmailMessageExists(emailMessage.Id))
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
        //    return View(emailMessage);
        //}

        //// GET: EmailMessages/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var emailMessage = await _context.EmailMessages
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (emailMessage == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(emailMessage);
        //}

        //// POST: EmailMessages/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var emailMessage = await _context.EmailMessages.FindAsync(id);
        //    _context.EmailMessages.Remove(emailMessage);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool EmailMessageExists(int id)
        //{
        //    return _context.EmailMessages.Any(e => e.Id == id);
        //}
    }
}
