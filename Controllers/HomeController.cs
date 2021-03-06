using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MeteoAlert.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using MailKit.Net.Pop3;
using MailKit;
using MimeKit;
using MeteoAlert.Services;
using MeteoAlert.Data;
using Microsoft.EntityFrameworkCore;

namespace MeteoAlert.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ElaborazioniDati _elaborazioniDati;
        private readonly IAdoMeteoService servizioMeteo;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, ElaborazioniDati elaborazioniDati, IAdoMeteoService servizioMeteo)
        {
            _context = context;
            _elaborazioniDati = elaborazioniDati;
            _logger = logger;
            this.servizioMeteo = servizioMeteo;
        }



        // GET: Allertas
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            DateTime ultimadatabollettino = _context.Bollettino.Include(s => s.EmailMessage).OrderByDescending(s => s.DateSent).First().DateSent;
            var bollettini = await _context.Bollettino.Where(s => s.DateSent == ultimadatabollettino).ToListAsync();

            var query = bollettini.OrderByDescending(s => s.Scadh).ThenBy(s => s.Sito).ToList();

            int[] max = new int[3];
            if (query.Count > 0)
            {
                max[0] = bollettini.Max(row => row.Wind10m_kmh);
                max[1] = bollettini.Max(row => row.Wind32m_kmh);
                max[2] = bollettini.Max(row => row.Wind57m_kmh);
            }
           

            ViewBag.max_vento = max.Max();
            ViewBag.Subject = query.FirstOrDefault().EmailMessage.Subject;

            List<Bollettino> items = _elaborazioniDati.bollettiniAllarme(query);
            
            DateTime ultimadataallerta = _context.Allerta.OrderByDescending(s => s.DateSent).First().DateSent;

            var allerte = new List<Allerta>();

            //secondo indicazioni AM, un messaggio di allerta precedente inoltrato, se non confermato al momento del ricevimento del nuovo bollettino è da considerarsi superato.
            //Pertanto il portale deve recepire tale indicazione aggiornando il sinottico eliminando le condizioni di allerta precedentemente comunicate e non confermate al momento del ricevimento del nuovo bollettino.
            //if ((ultimadataallerta.Date == ultimadatabollettino.Date)&&(ultim
            _logger.LogInformation("ultimadatabollettino " + ultimadatabollettino);
            //_logger.LogInformation("ultimadatabollettino-1 " + ultimadatabollettino.AddHours(-1));
            //_logger.LogInformation("ultimadatabollettino+1 " + ultimadatabollettino.AddHours(+1));
            _logger.LogInformation("ultimadataallerta " + ultimadataallerta);
            if ((ultimadataallerta >= ultimadatabollettino.AddHours(-1)) && (ultimadataallerta <= ultimadatabollettino.AddHours(+1)))
            {
                _logger.LogInformation(">>>>>>>>>>> cerco allerte in ultimadataallerta " + ultimadataallerta);
                allerte = await _context.Allerta.Where(s => s.DateSent == ultimadataallerta).ToListAsync();
            }
            

            ViewBag.Grafico = _elaborazioniDati.graficoCompleto(query, items, allerte);

            foreach (var giorno in ViewBag.Grafico)
            {
                foreach (var ora in giorno)
                {
                    if (ora[2] == "Allerta")
                    {
                        _logger.LogInformation($"Rilevata allerta {ora[0]} {ora[1]}");
                    }
                }
            }

            //return View();
            for (int i = 1; i < 4; i++)
            {
                string oraUltEvento = servizioMeteo.GetUltimoEventoLampinet(i);
                ViewData["UltEventoZona" + i] = oraUltEvento;
            }
            /*
             * PER NON MODIFICARE IL VIEW MODEL MI APPOGGIO AL VOLO
             * AL VIEW DATA.
             * TODO: MODIFICA DEL METEOVIEWMODEL PER AGGANCIARE LA TABELLA
             * DELLE SEQUENZE
             */
            ViewData["isAllarmeInCorso"] = servizioMeteo.isAllarmeInCorso();
            List<MeteoViewModel> meteoViewModels = servizioMeteo.GetEventi();
            return View(meteoViewModels);     //RICHIAMA LA VIEW INDEX IN /HOME
            //ViewBag.Title = "Lampinet - Ultime 10 rilevazioni";
            //return View("Eventi", meteoViewModels);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
