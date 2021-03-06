using MeteoAlert.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Services
{
    public class SendSmsMail
    {
        private readonly ILogger<SendSmsMail> _logger;
        private readonly IEmailService _emailservice;
        private readonly IConfiguration Configuration;
        private readonly IEmailSender _emailSender;
        private readonly AllarmiSMS AllarmiSMS;
        
        public SendSmsMail(IEmailService emailservice, IConfiguration configuration, IEmailSender emailSender, ILogger<SendSmsMail> logger, AllarmiSMS allarmiSMS)
        {
            Configuration = configuration;
            _emailservice = emailservice;
            _emailSender = emailSender;           
            _logger = logger;
            AllarmiSMS = allarmiSMS;
        }
        public async void Send(List<ContattoInviato> smsinviati, List<Rubrica> rubrica, string testosms, string testomail)
        {

            if (rubrica == null)
            {
                _logger.LogInformation(">>>>>>>>>> rubrica invio sms vuota");
            }
            else
            {
                _logger.LogInformation(">>>>>>>>>> rubrica invio sms piena: " + rubrica.Count());

                foreach (Rubrica contatto in rubrica)
                {
                    _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>sms a:" + @contatto.Name);

                    string risultato1 = "assente";
                    string risultato2 = "assente";


                    if (!String.IsNullOrEmpty(contatto.MobilePhone1))
                        risultato1 = AllarmiSMS.ComponiInviaSMS(testosms, contatto.MobilePhone1);
                    if (!String.IsNullOrEmpty(contatto.MobilePhone2))
                        risultato2 = AllarmiSMS.ComponiInviaSMS(testosms, contatto.MobilePhone2);
                    if (!String.IsNullOrEmpty(contatto.Email))
                    {
                        await _emailSender.SendEmailTextAsync(contatto.Email, testosms, testomail);
                        //await _emailSender.SendEmailAsync(contatto.Email, testosms, testosms);
                    }
                    smsinviati.Add(new ContattoInviato
                    {
                        DateSent = DateTime.Now,
                        Name = contatto.Name,
                        Mansione = contatto.Mansione,
                        Matricola = contatto.Matricola,
                        MobilePhone1 = contatto.MobilePhone1,
                        MobilePhone2 = contatto.MobilePhone2,
                        Email = contatto.Email,
                        Risultato1 = risultato1,
                        Risultato2 = risultato2,
                        Testosms = testosms

                    });

                    //if (!String.IsNullOrEmpty(testomail) && !String.IsNullOrEmpty(contatto.Mansione) && (contatto.Mansione.ToLower().Contains("mail")))
                    //{
                    //    risultato1 = "assente";
                    //    risultato2 = "assente";
                    //    if (!String.IsNullOrEmpty(contatto.MobilePhone1))
                    //        risultato1 = AllarmiSMS.ComponiInviaSMS(testomail, contatto.MobilePhone1);
                    //    if (!String.IsNullOrEmpty(contatto.MobilePhone2))
                    //        risultato2 = AllarmiSMS.ComponiInviaSMS(testomail, contatto.MobilePhone2);

                    //    smsinviati.Add(new ContattoInviato
                    //    {
                    //        DateSent = DateTime.Now,
                    //        Name = contatto.Name,
                    //        Mansione = contatto.Mansione,
                    //        Matricola = contatto.Matricola,
                    //        MobilePhone1 = contatto.MobilePhone1,
                    //        MobilePhone2 = contatto.MobilePhone2,
                    //        Email = contatto.Email,
                    //        Risultato1 = risultato1,
                    //        Risultato2 = risultato2,
                    //        Testosms = testomail

                    //    });
                    //}
                }
            }

        }
    }
}
