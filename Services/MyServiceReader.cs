using MeteoAlert.Data;
using MeteoAlert.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MeteoAlert.Services
{
    public class MyServiceReader : BackgroundService
    {

        private readonly ILogger<MyServiceReader> _logger;
        private readonly IEmailService _emailservice;
        private readonly IConfiguration Configuration;
        private readonly IEmailSender _emailSender;
        private readonly ElaborazioniDati _elaborazioniDati;
        private readonly AllarmiSMS AllarmiSMS;
        private readonly SendSmsMail _sendSmsMail;
        private List<Rubrica> rubrica;


        public MyServiceReader(IEmailService emailservice, IConfiguration configuration, IEmailSender emailSender, ILogger<MyServiceReader> logger, ElaborazioniDati elaborazioniDati, AllarmiSMS allarmiSMS, SendSmsMail sendSmsMail)
        {
            Configuration = configuration;
            _emailservice = emailservice;
            _emailSender = emailSender;
            _elaborazioniDati = elaborazioniDati;
            _logger = logger;
            AllarmiSMS = allarmiSMS;
            _sendSmsMail = sendSmsMail;
        }


        private bool EmailMessageExists(EmailMessage message, ApplicationDbContext _context)
        {
            if (CheckConnection(_context))
            {
                var responso = _context.EmailMessages.Include(d => d.OriginalMail).Where(d => d.OriginalMail.DateSent == message.DateSent).Any(e => e.OriginalMail.Mail == message.OriginalMail.Mail);
                _logger.LogInformation(">>>>>>>>> controllo esistenza email in database: "+ responso);
                return responso;
                //return _context.EmailMessages.Include(d => d.OriginalMail).Any(e => e.OriginalMail.Mail == message.OriginalMail.Mail);
                //return _context.EmailMessages.Include(d => d.OriginalMail).Any(e => e.Id == message.Id);
            }
            else
            {
                _logger.LogInformation(">>>>>>>>> controllo esistenza email in database ma database offline ritorno che non esiste");
                return false;
            }
        }
        private string ReadCSVFile(MemoryStream csv_file)
        {
            DataTable csvData = new DataTable();
            string jsonString = string.Empty;
            try
            {
                using (Microsoft.VisualBasic.FileIO.TextFieldParser csvReader = new TextFieldParser(csv_file))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields;
                    string primariga = "";
                    bool tableCreated = false;
                    while (tableCreated == false)
                    {
                        colFields = csvReader.ReadFields();
                        primariga = string.Join("", colFields).ToLower().Trim();
                        //_logger.LogInformation("Prima riga:" + primariga+":");
                        string colprec = "";
                        //int i = 0;
                        Console.Write("Colonne csv:");
                        foreach (string column in colFields)
                            if (!string.IsNullOrWhiteSpace(column))
                            {
                                //Console.Write(i++);

                                string col = "";
                                if (Char.IsLetter(column[0]))
                                {
                                    colprec = column;
                                    col = column;
                                }
                                else
                                {
                                    col = colprec + column;
                                }
                                string nomecolonna = Regex.Replace(col, @"[^0-9a-zA-Z]+", "").Trim();
                                DataColumn datecolumn = new DataColumn(nomecolonna.Substring(0, 1).ToUpper() + nomecolonna.Substring(1).ToLower());
                                Console.Write(datecolumn.ColumnName + " ");


                                csvData.Columns.Add(datecolumn);
                            }
                        
                        tableCreated = true;
                    }
                    while (!csvReader.EndOfData)
                    {

                        string[] rigaletta = csvReader.ReadFields();


                        if (primariga != string.Join("", rigaletta).ToLower().Trim())
                        {
                            string[] riga = new string[csvData.Columns.Count];
                            Array.Copy(rigaletta, riga, csvData.Columns.Count);
                            csvData.Rows.Add(riga);
                        }
                        else
                        {
                            //_logger.LogInformation(">>>>>>>>>>>>>>controllo salto riga:"+ string.Join("", rigaletta) +":");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                
                _logger.LogInformation($"Error:Parsing CSV { ex.Message}");
                return "Error:Parsing CSV";
            }
            //if everything goes well, serialize csv to json 
            jsonString = JsonConvert.SerializeObject(csvData);

            return jsonString;
        }
        //private async void SendSmsMail(EmailMessage message, List<Rubrica> rubrica, string testosms, string testomail)
        //{

        //    if (rubrica == null)
        //    {
        //        _logger.LogInformation(">>>>>>>>>> rubrica invio sms vuota");
        //    }
        //    else
        //    {
        //        _logger.LogInformation(">>>>>>>>>> rubrica invio sms piena: "+ rubrica.Count());

        //        foreach (Rubrica contatto in rubrica)
        //        {
        //            _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>sms a:" + contatto.Name);

        //            string risultato1 = "assente";
        //            string risultato2 = "assente";


        //            if (!String.IsNullOrEmpty(contatto.MobilePhone1))
        //                risultato1 = AllarmiSMS.ComponiInviaSMS(testosms, contatto.MobilePhone1);
        //            if (!String.IsNullOrEmpty(contatto.MobilePhone2))
        //                risultato2 = AllarmiSMS.ComponiInviaSMS(testosms, contatto.MobilePhone2);
        //            if (!String.IsNullOrEmpty(contatto.Email))
        //            {
        //                await _emailSender.SendEmailTextAsync(contatto.Email, testosms, message.Content);
        //                //await _emailSender.SendEmailAsync(contatto.Email, testosms, testosms);
        //            }
        //            message.SmsInviati.Add(new ContattoInviato
        //            {
        //                DateSent = DateTime.Now,
        //                Name = contatto.Name,
        //                Mansione = contatto.Mansione,
        //                Matricola = contatto.Matricola,
        //                MobilePhone1 = contatto.MobilePhone1,
        //                MobilePhone2 = contatto.MobilePhone2,
        //                Email = contatto.Email,
        //                Risultato1 = risultato1,
        //                Risultato2 = risultato2,
        //                Testosms = testosms

        //            });

        //            if (!String.IsNullOrEmpty(testomail) && (contatto.Mansione.ToLower().Contains("mail")))
        //            {
        //                risultato1 = "assente";
        //                risultato2 = "assente";
        //                if (!String.IsNullOrEmpty(contatto.MobilePhone1))
        //                    risultato1 = AllarmiSMS.ComponiInviaSMS(testomail, contatto.MobilePhone1);
        //                if (!String.IsNullOrEmpty(contatto.MobilePhone2))
        //                    risultato2 = AllarmiSMS.ComponiInviaSMS(testomail, contatto.MobilePhone2);

        //                message.SmsInviati.Add(new ContattoInviato
        //                {
        //                    DateSent = DateTime.Now,
        //                    Name = contatto.Name,
        //                    Mansione = contatto.Mansione,
        //                    Matricola = contatto.Matricola,
        //                    MobilePhone1 = contatto.MobilePhone1,
        //                    MobilePhone2 = contatto.MobilePhone2,
        //                    Email = contatto.Email,
        //                    Risultato1 = risultato1,
        //                    Risultato2 = risultato2,
        //                    Testosms = testomail

        //                });
        //            }
        //        }
        //    }

        //}
        public static bool CheckConnection(ApplicationDbContext context)
        {

            try
            {
                //context.Database.OpenConnection();
                //context.Database.CloseConnection();
                return context.Database.CanConnect();
            }
            catch (SqlException)
            {
                return false;
            }

            
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Generate a random number  
            Random random = new Random();
            int randomLessThan10 = random.Next(10);
            //await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>MyServiceReader is starting.");

            //_logger.LogInformation(AllarmiSMS.ComponiInviaSMS("Prova invio sms","393283387195"));

            stoppingToken.Register(() => _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>MyServiceReader is stopping."));


            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                    _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>MyServiceReader is doing background work.");

                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                    //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>MyServiceReader connection:" + Configuration.GetConnectionString("DefaultConnection"));

                    ApplicationDbContext context = new ApplicationDbContext(optionsBuilder.Options);

                    if (!CheckConnection(context))
                    {
                        _logger.LogInformation(">>>>>>>>>> Database non accessibile");
                    }
                    else
                    {
                        rubrica = context.Rubrica.Where(s => s.Sms == true).ToList();
                        _logger.LogInformation(">>>>>>>>>> rubrica invio sms: " + rubrica.Count());

                    }

                    foreach (EmailMessage message in _emailservice.ReceiveEmail())
                    {

                        //var rubricaprova = new List<Rubrica>();
                        //var contattoprova = new Rubrica();
                        //contattoprova.Name = "Sms di prova";
                        //contattoprova.MobilePhone1 = "3283387195";
                        //rubricaprova.Add(contattoprova);
                        //_sendSmsMail.Send(new List<ContattoInviato>(), rubricaprova, message.Content, message.Content);
                        //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>sms di prova:" + contattoprova.MobilePhone1 + message.Content);

                        _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Mail:" + message.Subject);
                        _logger.LogInformation(">>>>>>>>>>>>> allegati n: " + message.Attachments.Count());
                        if ((message.FromAddresses == "helpdesk@meteoam.it") && (message.Subject.ToLower().Contains("allerta") || message.Subject.ToLower().Contains("bollettino")))
                        {
                            if (!EmailMessageExists(message, context))
                            {
                                if (message.Subject.ToLower().Contains("allerta") && !message.Subject.ToLower().Contains(".csv"))
                                {
                                    
                                    string testosms = "Controllare la mail: meteo.taranto@arcelormittal.com con oggetto: " + message.Subject;
                                    string testomail = !String.IsNullOrEmpty(message.Content) ? message.Content.Replace("**This Message originated from a Non-ArcelorMittal source**", "").Trim() : "";
                                    _logger.LogInformation("Rilevato allarme in allerta meteo " + testosms);
                                    //SendSmsMail(message, rubrica, testosms, testomail);
                                    _sendSmsMail.Send(message.SmsInviati, rubrica, testosms, testomail);
                                }




                                foreach (Attachment allegato in message.Attachments)
                                {
                                    //_logger.LogInformation("analizzo " + allegato.FileName);
                                    if (allegato.FileName.ToLower().Contains(".csv"))
                                    {
                                        //_logger.LogInformation("analizzo csv "+ allegato.FileName);
                                        using (var file = new MemoryStream(allegato.Content))
                                        {
                                            string jsonCsv = ReadCSVFile(file);
                                            if (message.Subject.ToLower().Contains("bollettino"))
                                            {
                                                message.Bollettinos = (List<Bollettino>)JsonConvert.DeserializeObject(jsonCsv, (typeof(List<Bollettino>)));
                                                message.Bollettinos.ForEach(s => s.DateSent = message.DateSent);
                                                message.Bollettinos.ForEach(s => s.EmailMessage = message);

                                                List<Bollettino> items = _elaborazioniDati.bollettiniAllarme(message.Bollettinos);
                                                var Grafico = _elaborazioniDati.graficoBollettino(message.Bollettinos, items);

                                                bool presenzaAllerte = false;
                                                foreach (var giorno in Grafico)
                                                {
                                                    foreach (var ora in giorno)
                                                    {
                                                        if (ora[2] == "Allerta")
                                                        {
                                                            _logger.LogInformation("Rilevato allarme in bollettino " + ora[1] + " " + ora[2]);
                                                            presenzaAllerte = true;
                                                        }
                                                    }
                                                }
                                                if (presenzaAllerte)
                                                {
                                                    string testosms = "Controllare sito meteoalert.gruppoilva.int e la mail: meteo.taranto@arcelormittal.com con oggetto: " + message.Subject;
                                                    string testomail = !String.IsNullOrEmpty(message.Content) ? message.Content.Replace("**This Message originated from a Non-ArcelorMittal source**", "").Trim() : "";
                                                    //SendSmsMail(message, rubrica, testosms, testomail);
                                                    _sendSmsMail.Send(message.SmsInviati, rubrica, testosms, testomail);
                                                }
                                            }

                                            if (message.Subject.ToLower().Contains("allerta"))
                                            {
                                                message.Allertas = (List<Allerta>)JsonConvert.DeserializeObject(jsonCsv, (typeof(List<Allerta>)));
                                                message.Allertas.ForEach(s => s.DateSent = message.DateSent);
                                                message.Allertas.ForEach(s => s.EmailMessage = message);
                                                ////disabilitato in data 20/05/2020 per allineamento con aeronautica ora calcolata da lei
                                                //    var Grafico = _elaborazioniDati.graficoAllerta(message.Allertas);
                                                //    bool presenzaAllerte = false;
                                                //    foreach (var giorno in Grafico)
                                                //    {
                                                //        foreach (var ora in giorno)
                                                //        {
                                                //            if (ora[2] == "Allerta")
                                                //            {
                                                //                _logger.LogInformation("Rilevato allarme in allerta " + ora[1] + " " + ora[2]);
                                                //                presenzaAllerte = true;
                                                //            }
                                                //        }
                                                //    }
                                                //    if (presenzaAllerte)
                                                //    {
                                                //        string testosms = "Controllare sito meteoalert.gruppoilva.int e la mail: meteo.taranto@arcelormittal.com con oggetto: " + message.Subject;
                                                //        string testomail = !String.IsNullOrEmpty(message.Content) ? message.Content.Replace("**This Message originated from a Non-ArcelorMittal source**", "").Trim() : "";
                                                //        SendSmsMail(message, context, testosms, testomail);
                                                //    }
                                            }
                                        }

                                    }
                                }

                                //if (
                                //   !(
                                //        message.Subject.ToLower().Contains(".csv") &&
                                //          message.Subject.ToLower().Contains("allerta")
                                //     )
                                //   )
                                if (CheckConnection(context))
                                {
                                    context.Add(message);
                                    await context.SaveChangesAsync();
                                    //context.SaveChanges();
                                    // _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>save Mail:" + message.Subject);
                                }

                            }
                            else
                            {
                                // _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>già esiste Mail:" + message.Subject);
                            }
                        }
                    }
                    if (CheckConnection(context)) await context.DisposeAsync();


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing task.");
                }


            }





            _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>MyServiceReader background task is stopping.");
        }
    }
}
