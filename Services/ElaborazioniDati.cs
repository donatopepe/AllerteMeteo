using MeteoAlert.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Services
{
    public class ElaborazioniDati
    {
        private readonly ILogger<ElaborazioniDati> _logger;

        public ElaborazioniDati(ILogger<ElaborazioniDati> logger)
        {
            _logger = logger;
        }

        public List<List<string[]>> graficoAllerta(List<Allerta> allerte)
        {
            var query = allerte.OrderBy(s => s.HourOfDay).ToList();
            var myList = new List<List<string[]>>();

            if (query.Count() > 0)
            {
                List<Allerta> items = new List<Allerta>();//_context.Bollettino.Where(s => s.DateSent == ultimadata).OrderBy(s => s.Scadh).ToList()
                var maxdata = query.Max(s => s.HourOfDay);
                var mindata = query.Min(s => s.HourOfDay);

                //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>><<max sito:"+maxsito+" mindata:"+mindata+" maxdata:"+maxdata);
                ////disabilitato in data 20/05/2020 per allineamento con aeronautica ora calcolata da lei
                //string Tp3h30mm = "";
                //string Cp1h20mm = "";

                foreach (var item in query.OrderByDescending(s => s.HourOfDay))
                {
                    ////disabilitato in data 20/05/2020 per allineamento con aeronautica ora calcolata da lei
                    //if (
                    //        (!String.IsNullOrWhiteSpace(item.Ws32m2) && (Int32.Parse(item.Ws32m2) > 0)) ||
                    //        (!String.IsNullOrWhiteSpace(item.Ws57m2) && (Int32.Parse(item.Ws57m2) > 0)) ||
                    //        (
                    //            (
                    //                (!String.IsNullOrWhiteSpace(item.Ws10m) && (Int32.Parse(item.Ws10m) > 0)) |
                    //                (!String.IsNullOrWhiteSpace(item.Ws32m1) && (Int32.Parse(item.Ws32m1) > 0)) |
                    //                (!String.IsNullOrWhiteSpace(item.Ws57m1) && (Int32.Parse(item.Ws57m1) > 0))
                    //            ) &
                    //            (
                    //                //(!String.IsNullOrWhiteSpace(item.Cp1h) && (Int32.Parse(item.Cp1h) > 0)) |
                    //                (!String.IsNullOrWhiteSpace(Cp1h20mm) && (Int32.Parse(Cp1h20mm) > 0)) |
                    //                (!String.IsNullOrWhiteSpace(Tp3h30mm) && (Int32.Parse(Tp3h30mm) > 0)) |
                    //                (!String.IsNullOrWhiteSpace(item.Sigww) && (Int32.Parse(item.Sigww) > 0)) |
                    //                (!String.IsNullOrWhiteSpace(item.Phenomena) && (Int32.Parse(item.Phenomena) > 0))
                    //            )
                    //        )
                    //    )
                    {
                        items.Add(item);
                        //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>add:" + item.HourOfDay);
                    }
                    ////disabilitato in data 20/05/2020 per allineamento con aeronautica ora calcolata da lei
                    //if (!String.IsNullOrWhiteSpace(item.Ora) && (Int32.Parse(item.Ora) > 0) && ((Int32.Parse(item.Ora) % 3)==0))
                    ////if (!String.IsNullOrWhiteSpace(item.Prectot3h) && (Int32.Parse(item.Prectot3h) > 0))
                    //{
                    //    Tp3h30mm = item.Prectot3h;
                    //}
                    //Cp1h20mm = item.Cp1h;

                }
                System.TimeSpan diff = maxdata.Date.Subtract(mindata.Date);
                //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>TotalDays:" + diff.TotalDays + " " + (int)Math.Ceiling(diff.TotalDays));
                for (int j = (int)Math.Ceiling(diff.TotalDays); j >= 0; j--)
                {
                    var giorno = new List<string[]>();
                    var j_date = mindata.AddDays(j);


                    for (int i = 23; i >= 0; i--)
                    {
                        string tipobottone = "btn-success";
                        string titolopopup = "Nessuna allarme";
                        string testopopup = "Nessuna segnalazione";
                        var datadacercare = j_date.Date.AddHours(i);
                        var find = items.Where(s => s.HourOfDay == datadacercare);
                        //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>" + datadacercare + ">>>>>>" + find.Count());
                        if (find.Count() > 0)
                        {
                            tipobottone = "btn-danger";
                            titolopopup = "Allarme in ultima allerta";
                            testopopup = "Pericolo data: " + String.Join(" ", find.Select(s => s.HourOfDay).ToList());
                        }
                        giorno.Insert(0, new string[5] { datadacercare.ToString("dddd, dd MMMM yyyy"), i.ToString() + "h", titolopopup, testopopup, tipobottone });
                    }
                    myList.Insert(0, giorno);
                    //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>> giorno addizionato in lista " + giorno.Count());
                }
                //if (inAlarm)
                //{
                //    //ViewBag.Grafico = myList;
                //    //return View(items);
                //    query = items;
                //}
            }
            return myList;
        }


        public List<Bollettino> bollettiniAllarme(List<Bollettino> query)
        {
            List<Bollettino> items = new List<Bollettino>();


            if (query.Count() > 0)
            {

                var maxsito = query.Max(s => Convert.ToInt32(s.Sito));
                var minsito = query.Min(s => Convert.ToInt32(s.Sito));

                //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>><<max sito:"+maxsito+" mindata:"+mindata+" maxdata:"+maxdata);
                string[] Tp3h30mm = new string[maxsito + 1];
                string[] Cp1h20mm = new string[maxsito + 1];
                for (int i = 0; i < Tp3h30mm.Length; i++)
                {
                    Tp3h30mm[i] = "//";
                    Cp1h20mm[i] = "F";
                }
                foreach (var item in query.OrderByDescending(s => s.Scadh).ThenBy(s => s.Sito))
                {

                    if (
                            (item.Wind32m20ms != "F") ||
                            (item.Wind57m20ms != "F") ||
                            (
                                (
                                    (item.Wind10m1544ms != "F") |
                                    (item.Wind32m1667ms != "F") |
                                    (item.Wind57m1667ms != "F")
                                ) &
                                (
                                    //(item.Cp1h20mm != "F") |
                                    (Cp1h20mm[Convert.ToInt32(item.Sito)] != "F") |
                                    ((Tp3h30mm[Convert.ToInt32(item.Sito)] != "F") & (Tp3h30mm[Convert.ToInt32(item.Sito)] != "//")) |
                                    //(item.Ww != "0") |
                                    (item.Phenomena != "F")
                                )
                            )
                        )
                    {
                        items.Add(item);
                    }
                    if (!String.IsNullOrWhiteSpace(item.Scadh) && (Int32.Parse(item.Scadh) > 0) && ((Int32.Parse(item.Scadh) % 3) == 0))
                    //if (item.Tp3h30mm != "//")
                    {
                        Tp3h30mm[Convert.ToInt32(item.Sito)] = item.Tp3h30mm;
                    }

                    Cp1h20mm[Convert.ToInt32(item.Sito)] = item.Cp1h20mm;

                }
            }

            return (items);
        }




        public List<List<string[]>> graficoBollettino(List<Bollettino> query, List<Bollettino> items)
        {
            var myList = new List<List<string[]>>();

            if (query.Count() > 0)
            {

                var maxdata = query.Max(s => s.HourOfDay);
                var mindata = query.Min(s => s.HourOfDay);
                System.TimeSpan diff = maxdata.Date.Subtract(mindata.Date);
                for (int j = 0; j <= diff.TotalDays; j++)
                {
                    var giorno = new List<string[]>();
                    var j_date = mindata.AddDays(j);
                    var querygiorno = query.Where(s => s.HourOfDay.Date == j_date.Date);
                    var maxquerygiorno = querygiorno.Max(s => s.HourOfDay);
                    var minquerygiorno = querygiorno.Min(s => s.HourOfDay);
                    //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>ora min:" + minquerygiorno);
                    //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>ora max:" + maxquerygiorno);
                    for (int i = minquerygiorno.Hour; i <= maxquerygiorno.Hour; i++)
                    {
                        string tipobottone = "btn-success";
                        string titolopopup = "Nessuna allarme";
                        string testopopup = "Nessuna segnalazione";
                        //var giornosel = new DateTime(minquerygiorno.Year, minquerygiorno.Month, minquerygiorno.Day, 0, 0, 0);
                        //var datadacercare = giornosel.Date.AddHours(i);
                        var datadacercare = j_date.Date.AddHours(i);
                        var find = items.Where(s => s.HourOfDay == datadacercare);

                        //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>"+ datadacercare+"-->" + find.Count());
                        if ((find != null) && (find.Count() > 0))
                        {
                            tipobottone = "btn-danger";
                            titolopopup = "Allarme in ultimo bollettino";
                            testopopup = find.Count() + " segnalazioni Sito: " + String.Join(" ", find.Select(s => s.Sito).ToList());
                        }

                        giorno.Add(new string[5] { minquerygiorno.ToString("dddd, dd MMMM yyyy"), i.ToString() + "h", titolopopup, testopopup, tipobottone });
                    }
                    myList.Add(giorno);
                    //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>> giorno addizionato in lista " + giorno.Count());
                }

            }

            return myList;
        }

        public List<List<string[]>> graficoCompleto(List<Bollettino> querybollettino, List<Bollettino> items, List<Allerta> allerte)
        {
            _logger.LogInformation(">>>>>>>>>>>>>allerte in esame     " + allerte.Count);
            _logger.LogInformation(">>>>>>>>>>>>>bollettini in esame     " + items.Count);
            var myList = new List<List<string[]>>();
            var queryallerte = allerte.OrderBy(s => s.HourOfDay).ToList();
            if ((querybollettino.Count() > 0) || (queryallerte.Count() > 0))
            {
                var maxdata = new DateTime();
                var mindata = new DateTime();

                var maxdatabollettino = new DateTime();
                var mindatabollettino = new DateTime();
                if ((querybollettino.Count() > 0))
                {
                    maxdata = maxdatabollettino = querybollettino.Max(s => s.HourOfDay);
                    mindata = mindatabollettino = querybollettino.Min(s => s.HourOfDay);
                }

                var maxdataallerte = new DateTime();
                var mindataallerte = new DateTime();
                if (queryallerte.Count() > 0)
                {
                    maxdata = maxdataallerte = queryallerte.Max(s => s.HourOfDay);
                    mindata = mindataallerte = queryallerte.Min(s => s.HourOfDay);
                }

                if ((querybollettino.Count() > 0) && (queryallerte.Count() > 0))
                {
                    maxdata = (maxdatabollettino > maxdataallerte ? maxdatabollettino : maxdataallerte);
                    mindata = (mindatabollettino < mindataallerte ? mindatabollettino : mindataallerte);
                }
                //var oggi = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                var oggi = DateTime.Now.Date;
                mindata = (oggi > mindata ? oggi : mindata);
                //_logger.LogInformation(">>>>>>>>>>>>>data di oggi         "+oggi+"   data minima   "+mindata);
                List<Allerta> itemsallerte = new List<Allerta>();
                foreach (var item in queryallerte.OrderByDescending(s => s.HourOfDay))
                {
                    itemsallerte.Add(item);
                    //_logger.LogInformation(">>>>>>>>>>>>>allerta      " + item.HourOfDay);
                }

                System.TimeSpan diff = maxdata.Date.Subtract(mindata.Date);
                for (int j = 0; j <= diff.TotalDays; j++)
                {
                    var giorno = new List<string[]>();
                    var j_date = mindata.AddDays(j);
                    int minhour = 0;
                    int maxhour = 23;
                    
                    if (j_date.Date== mindata.Date) 
                    {
                        minhour = mindata.Hour;
                    }
                    if (j_date.Date == maxdata.Date)
                    {
                        maxhour = maxdata.Hour;
                    }
                    //_logger.LogInformation(">>>>>>>>>>>>>j_date        " + j_date + "   minhour   " + minhour + "   maxhour   " + maxhour);
                    for (int i = minhour; i <= maxhour; i++)
                    {
                        string tipobottone = "btn-success";
                        string titolopopup = "Nessuna allarme";
                        string testopopup = "Nessuna segnalazione";
                        //var giornosel = new DateTime(j_date.Year, j_date.Month, j_date.Day, 0, 0, 0);
                        //var datadacercare = giornosel.Date.AddHours(i);
                        var datadacercare = j_date.Date.AddHours(i);
                        var findbollettino = items.Where(s => s.HourOfDay == datadacercare);
                        var findallerta = itemsallerte.Where(s => s.HourOfDay == datadacercare);
                        //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>" + datadacercare + " allerte-->" + findallerta.Count()+" bollettini -->"+ findbollettino.Count());
                        
                        if ((findbollettino != null) && (findbollettino.Count() > 0))
                        {
                            tipobottone = "btn-danger";
                            titolopopup = "Allarme in ultimo bollettino";
                            testopopup = findbollettino.Count() + " segnalazioni Sito: " + String.Join(" ", findbollettino.Select(s => s.Sito).ToList());
                        }
                        if ((findallerta != null) && (findallerta.Count() > 0))
                        {
                            tipobottone = "btn-danger";
                            titolopopup = "Allarme in ultima allerta";
                            testopopup = "Pericolo data: " + String.Join(" ", findallerta.Select(s => s.HourOfDay).ToList());
                        }
                        if ((findbollettino != null) && (findbollettino.Count() > 0)&& (findallerta != null) && (findallerta.Count() > 0))
                        {
                            tipobottone = "btn-danger";
                            titolopopup = "Allarme in ultima allerta e ultimo bollettino";
                            testopopup = "Allerta: data: " + String.Join("Allerta ", findallerta.Select(s => s.HourOfDay).ToList());
                            testopopup = testopopup + "<br /> Bollettino: " + findbollettino.Count() + " segnalazioni Sito: " + String.Join(" ", findbollettino.Select(s => s.Sito).ToList());
                        }

                        giorno.Add(new string[5] { datadacercare.ToString("dddd, dd MMMM yyyy"), i.ToString() + "h", titolopopup, testopopup, tipobottone });
                    }
                    myList.Add(giorno);
                    //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>> giorno addizionato in lista " + giorno.Count());
                }

            }
            return myList;
        }


}
}
