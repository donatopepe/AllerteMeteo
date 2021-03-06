using MeteoAlert.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Services
{
    public class AdoMeteoService : IAdoMeteoService
    {
        string _QRY = "";
        private readonly IMsSqlService mySqlDb;

        public AdoMeteoService(IMsSqlService mySqlDb)
        {
            this.mySqlDb = mySqlDb;
        }

        public List<MeteoViewModel> GetEventi()
        {
            bool _allarmeInCorso = isAllarmeInCorso();
            _QRY = "SELECT TOP 10 IDEvento, DataOra, Evento, NomeImmagine, ScaricheElettriche, ConteggioInvioSMS FROM Eventi ORDER BY IDEvento DESC";
            DataSet dataSet = mySqlDb.Query(_QRY);
            DataTable dt = dataSet.Tables[0];
            List<MeteoViewModel> ListaEventi = new List<MeteoViewModel>();
            foreach (DataRow EventiRow in dt.Rows)
            {
                MeteoViewModel Spesa = MeteoViewModel.FromDataRows(EventiRow);
                ListaEventi.Add(Spesa);
            }
            return ListaEventi;
        }
        public bool ImageIsDifferent(string filename)
        {
            _QRY = "SELECT TOP 1 NomeImmagine FROM Eventi ORDER BY IDEvento DESC";
            //DataSet dataSet = MyDb.Query(_QRY);
            DataSet dataSet = mySqlDb.Query(_QRY);
            DataTable dt = dataSet.Tables[0];
            //_oraEvento = (dt.Rows.Count > 0) ? dt.Rows[0][0].ToString() : "1900/01/01 00:00:00";
            //_oraEvento = (dt.Rows.Count > 0) ? dt.Rows[0][0].ToString() : "-";
            string file = (dt.Rows.Count > 0) ? dt.Rows[0][0].ToString() : "-";
            Console.WriteLine(file);
            return !String.Equals(file, filename);
        }
        public int ReadInvioSMS()
        {
            _QRY = "SELECT TOP 1 ConteggioInvioSMS FROM Eventi  WHERE DataOra > '" + DateTime.Now.AddHours(-1).ToString("yyyy - MM - dd HH: mm: ss")  + "' ORDER BY IDEvento DESC";
            DataSet dataSet = mySqlDb.Query(_QRY);
            DataTable dt = dataSet.Tables[0];
            int ConteggioInvioSMS = Convert.ToInt32(dt.Rows[0][0] == DBNull.Value ? 0 : dt.Rows[0][0]);
            Console.WriteLine("Ultimo conteggio invio SMS in db: "+ConteggioInvioSMS);
            return ConteggioInvioSMS;
        }
        public bool ReadMaxInvioSMS(int MaxSms = 0)
        {
            //_QRY = "SELECT * FROM Eventi  WHERE DataOra > '" + DateTime.Now.AddHours(-1).ToString("yyyy - MM - dd HH: mm: ss")  + "' AND ConteggioInvioSMS < "+ MaxSms.ToString() + " ORDER BY IDEvento DESC";
            
            _QRY = "SELECT COUNT(*) FROM Eventi WHERE DataOra > '" + DateTime.Now.AddHours(-1).ToString("yyyy - MM - dd HH: mm: ss") + "' AND ISNULL(ConteggioInvioSMS, 0) < " + MaxSms.ToString();

            Console.WriteLine(_QRY);
            DataSet dataSet = mySqlDb.Query(_QRY);
            DataTable dt = dataSet.Tables[0];

            //Console.WriteLine("Numero conteggio invio SMS sotto " + MaxSms +" verificati nell'ultima ora:"+ dt.Rows.Count);
            Console.WriteLine("Numero conteggio invio SMS sotto " + MaxSms + " verificati nell'ultima ora:" + dt.Rows[0][0]);
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }
        public string GetUltimoEventoLampinet(int nZona)
        {
            string _oraEvento = "";
            _QRY = "SELECT TOP 1 DataOra AS OraEvento FROM Eventi WHERE ScaricheElettriche >0 ORDER BY IDEvento DESC";
            DataSet dataSet = mySqlDb.Query(_QRY);
            DataTable dt = dataSet.Tables[0];
            _oraEvento = (dt.Rows.Count > 0) ? dt.Rows[0][0].ToString() : "-";

            return _oraEvento;
        }

        public int daUltimaSequenza()
        {
            _QRY = "SELECT " +
            "DATEDIFF(minute," +
            "(SELECT TOP 1 DataOra FROM Eventi WHERE ScaricheElettriche > 0 ORDER BY idEvento DESC)," +
            "getdate())" +
            "as differenzaInMinuti;";
            DataSet dataSet = mySqlDb.Query(_QRY);
            DataTable dt = dataSet.Tables[0];
            int diffInMinutes = Convert.ToInt32(dt.Rows[0][0] == DBNull.Value ? 0 : dt.Rows[0][0]);
            Console.WriteLine("Calcolata differenza in minuti da ultimo evento: " + diffInMinutes);
            return diffInMinutes;
        }

        public bool isAllarmeInCorso()
        {

            _QRY = "SELECT " +
            "TOP 1 * " +
            "FROM " +
            "SequenzeAllertaLampinet " +
            "ORDER BY " +
            "IDSequenza DESC";
            DataSet dataSet = mySqlDb.Query(_QRY);
            DataTable dt = dataSet.Tables[0];
            if (true)
            {
                if (dt.Rows[0]["DataOraInizioSequenza"]!= DBNull.Value && dt.Rows[0]["DataOraFineSequenza"] == DBNull.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
























    //public class AdoMeteoService : IMeteoService
    //{
    //    string _QRY = "";
    //    //private readonly ISqliteService MyDb;
    //    //public AdoMeteoService(ISqliteService MyDb)
    //    private readonly IMsSqlService mySqlDb;

    //    public AdoMeteoService(IMsSqlService mySqlDb)
    //    {
    //        //this.MyDb = MyDb;
    //        this.mySqlDb = mySqlDb;
    //    }

    //    public List<MeteoViewModel> GetEventi()
    //    {
    //        //_QRY = "SELECT IDEvento, DataOra, Evento, Perc, NomeImmagine, Zona1, Zona2, Zona3, Zona4, TotaleZone, ImgEsaminata FROM Eventi ORDER BY IDEvento DESC LIMIT 1";
    //        //_QRY = "SELECT IDEvento, DataOra, Evento, Perc, NomeImmagine, Zona1, Zona2, Zona3, Zona4, TotaleZone, null as ImgEsaminata FROM Eventi ORDER BY IDEvento DESC LIMIT 10";
    //        _QRY = "SELECT TOP 10 IDEvento, DataOra, Evento, NomeImmagine, Zona1, Zona2, Zona3, Zona4, TotaleZone, null as ImgEsaminata FROM Eventi ORDER BY IDEvento DESC";
    //        //DataSet dataSet = MyDb.Query(_QRY);
    //        DataSet dataSet = mySqlDb.Query(_QRY);
    //        DataTable dt = dataSet.Tables[0];
    //        List<MeteoViewModel> ListaEventi = new List<MeteoViewModel>();
    //        foreach (DataRow EventiRow in dt.Rows)
    //        {
    //            MeteoViewModel Spesa = MeteoViewModel.FromDataRows(EventiRow);
    //            ListaEventi.Add(Spesa);
    //        }
    //        return ListaEventi;
    //    }
    //    public bool ImageIsDifferent(string filename)
    //    {


    //        _QRY = "SELECT TOP 1 NomeImmagine FROM Eventi ORDER BY IDEvento DESC";
    //        //DataSet dataSet = MyDb.Query(_QRY);
    //        DataSet dataSet = mySqlDb.Query(_QRY);
    //        DataTable dt = dataSet.Tables[0];
    //        //_oraEvento = (dt.Rows.Count > 0) ? dt.Rows[0][0].ToString() : "1900/01/01 00:00:00";
    //        //_oraEvento = (dt.Rows.Count > 0) ? dt.Rows[0][0].ToString() : "-";
    //        string file = (dt.Rows.Count > 0) ? dt.Rows[0][0].ToString() : "-";
    //        Console.WriteLine(file);
    //        return !String.Equals(file , filename);
    //    }
    //    public string GetUltimoEventoLampinet(int nZona)
    //    {
    //        string _oraEvento = "";
    //        switch (nZona)
    //        {
    //            case 1:
    //                //_QRY = "SELECT DataOra AS OraEvento, Zona1 FROM Eventi WHERE Zona1 >0 ORDER BY IDEvento DESC LIMIT 1";
    //                _QRY = "SELECT TOP 1 DataOra AS OraEvento, Zona1 FROM Eventi WHERE Zona1 >0 ORDER BY IDEvento DESC";
    //                break;
    //            case 2:
    //                //_QRY = "SELECT DataOra AS OraEvento, Zona2 FROM Eventi WHERE Zona2 >0 ORDER BY IDEvento DESC LIMIT 1";
    //                _QRY = "SELECT TOP 1 DataOra AS OraEvento, Zona2 FROM Eventi WHERE Zona2 >0 ORDER BY IDEvento DESC";
    //                break;
    //            case 3:
    //                //_QRY = "SELECT DataOra AS OraEvento, Zona3 FROM Eventi WHERE Zona3 >0 ORDER BY IDEvento DESC LIMIT 1";
    //                _QRY = "SELECT TOP 1 DataOra AS OraEvento, Zona3 FROM Eventi WHERE Zona3 >0 ORDER BY IDEvento DESC";
    //                break;
    //        }
    //        //DataSet dataSet = MyDb.Query(_QRY);
    //        DataSet dataSet = mySqlDb.Query(_QRY);
    //        DataTable dt = dataSet.Tables[0];
    //        //_oraEvento = (dt.Rows.Count > 0) ? dt.Rows[0][0].ToString() : "1900/01/01 00:00:00";
    //        _oraEvento = (dt.Rows.Count > 0) ? dt.Rows[0][0].ToString() : "-";

    //        return _oraEvento;
    //    }
    //}
}
