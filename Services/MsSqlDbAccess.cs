using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MeteoAlert.Services
{
    public class MsSqlDbAccess : IMsSqlService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MsSqlDbAccess> _logger;
        public MsSqlDbAccess(IConfiguration configuration, ILogger<MsSqlDbAccess> logger)
        {
            this.configuration = configuration;
            _logger = logger;
        }
        
        public void ExecuteCommand(string QRY)
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];
            
            using (SqlConnection _conn = new SqlConnection(connectionString))
            {
                _conn.Open();
                using (SqlCommand _cmd = new SqlCommand(QRY, _conn))
                {
                    _cmd.ExecuteNonQuery();
                }
            }
        }
        public DataSet Query(string QRY)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand _cmd = new SqlCommand(QRY, conn))
                {
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        dataSet.Tables.Add(dataTable);
                        dataTable.Load(_reader);
                        return dataSet;
                    }
                }
            }
            return dataSet;
        }




        public void ExecuteQueryMeteo(string QRY, DateTime DataOra, string Evento, Bitmap ImgEsaminata, string NomeFile, int ScaricheElettriche, int ConteggioInvioSMS)
        {
            try
            {
                /* Per pter archiviare la bitmap in sqlite, devo prima convertirla in un array di byte
                 * Appoggio l'immagine in uno stream in memoria ed utilizzo il metodo GetBuffer
                 * dello stream per convertire l'immagine in un array di byte
                 */
                MemoryStream stream = new MemoryStream();
                Bitmap bitmap = new Bitmap(ImgEsaminata);
                bitmap.Save(stream, ImageFormat.Bmp);
                byte[] ImgTobyteArray = stream.GetBuffer();

                string connectionString = configuration["ConnectionStrings:DefaultConnection"];
                using (SqlConnection _conn = new SqlConnection(connectionString))
                {
                    _conn.Open();
                    using (SqlCommand _cmd = new SqlCommand(QRY, _conn))
                    {

                        _cmd.Parameters.Add("@DataOra", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        _cmd.Parameters.Add("@Evento", SqlDbType.Text).Value = Evento;
                        //_cmd.Parameters.Add("@Zona1", SqlDbType.Real).Value = Zona1;
                        //_cmd.Parameters.Add("@Zona2", SqlDbType.Real).Value = Zona2;
                        //_cmd.Parameters.Add("@Zona3", SqlDbType.Real).Value = Zona3;
                        //_cmd.Parameters.Add("@Zona4", SqlDbType.Real).Value = Zona4;
                        //_cmd.Parameters.Add("@TotaleZone", SqlDbType.Real).Value = TotaleZone;
                        _cmd.Parameters.Add("@ImgEsaminata", SqlDbType.VarBinary).Value = ImgTobyteArray;
                        _cmd.Parameters.Add("@NomeImmagine", SqlDbType.Text).Value = NomeFile;
                        _cmd.Parameters.Add("@ScaricheElettriche", SqlDbType.Int).Value = ScaricheElettriche;
                        _cmd.Parameters.Add("@ConteggioInvioSMS", SqlDbType.Int).Value = ConteggioInvioSMS;
                        //_logger.LogInformation("query " + ConteggioInvioSMS);
                        _cmd.ExecuteNonQuery();
                        

                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "ExecuteQueryMeteo");
            }
            catch (Exception ex1)
            {
                _logger.LogError(ex1, "ExecuteQueryMeteo");
            }

        }
        public double ReturnDoubleFromDataReader(string QRY)
        {
            string _ConnectionString = configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection _conn = new SqlConnection(_ConnectionString))
            {
                _conn.Open();
                using (SqlCommand _comm = new SqlCommand(QRY, _conn))
                {
                    using (SqlDataReader _reader = _comm.ExecuteReader())
                    {
                        if (_reader.HasRows)
                        {
                            _reader.Read();
                            double _myDbl = Convert.ToDouble(_reader[0]);
                            return _myDbl;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
        }

    }
}