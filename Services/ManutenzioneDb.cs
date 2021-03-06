using MeteoAlert.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeteoAlert.Services
{
    public class ManutenzioneDb : BackgroundService
    {
        private readonly ILogger<ManutenzioneDb> _logger;
        private readonly IConfiguration Configuration;
        private readonly IMsSqlService mySqlDb;


        public ManutenzioneDb(IConfiguration configuration, ILogger<ManutenzioneDb> logger, IMsSqlService mySqlDb)
        {
            Configuration = configuration;
            this.mySqlDb = mySqlDb;
            _logger = logger;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>ManutenzioneDb is starting.");
            stoppingToken.Register(() => _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>ManutenzioneDb is stopping."));
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>ManutenzioneDb is doing background work.");

                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                    //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>ManutenzioneDb connection:" + Configuration.GetConnectionString("DefaultConnection"));


                    ApplicationDbContext context = new ApplicationDbContext(optionsBuilder.Options);


                    if (!CheckConnection(context))
                    {
                        _logger.LogInformation(">>>>>>>>>> Database non accessibile");
                    }
                    else
                    {


                        var ultimadata = context.EmailMessages.OrderByDescending(s => s.DateSent).Take(1).FirstOrDefault().DateSent;
                        var primadata = ultimadata.AddMonths(-6);
                        _logger.LogInformation("Ultima data in db " + ultimadata.ToString());
                        _logger.LogInformation("Cancello dati prima del " + primadata.ToString());

                        await context.EmailMessages
                            .Include(s => s.OriginalMail)
                            .Include(s => s.Bollettinos)
                            .Include(s => s.Allertas)
                            .Include(s => s.SmsInviati)
                            .Include(s => s.Attachments)
                            .Where(w => w.DateSent < primadata)
                            .ForEachAsync(s =>
                          {
                              context.Remove(s.OriginalMail);
                              s.Allertas.ForEach(w => context.Remove(w));
                              s.Bollettinos.ForEach(w => context.Remove(w));
                              s.SmsInviati.ForEach(w => context.Remove(w));
                              s.Attachments.ForEach(w => context.Remove(w));
                              context.Remove(s);
                          });

                        context.SaveChanges();

                        string _dataOraRif = primadata.ToString("yyyy-MM-dd HH:mm:ss");
                        try
                        {
                            

                            var _QRY = "DELETE FROM Eventi WHERE DataOra < '" + _dataOraRif + "'";
                            mySqlDb.ExecuteCommand(_QRY);

                            _logger.LogInformation("Eseguito script pulizia eventi lampi");

                            try
                            {


                                var _QRY2 = "DELETE FROM ContattoInviato WHERE DateSent < '" + _dataOraRif + "'";
                                mySqlDb.ExecuteCommand(_QRY2);

                                _logger.LogInformation("Eseguito script pulizia sms inviati");
                            }
                            catch (SqlException ex)
                            {
                                _logger.LogError(ex, "Errore pulizia sms inviati");
                            }


                        }
                        catch (SqlException ex)
                        {
                            _logger.LogError(ex, "Errore pulizia eventi lampi");
                        }



                    }


                    if (CheckConnection(context)) await context.DisposeAsync();

                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing task.");
                }


            }





            _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>ManutenzioneDb background task is stopping.");
        }
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
    }
}
