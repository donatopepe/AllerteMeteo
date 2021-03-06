using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{







    public class MeteoViewModel
    {
        public int IDEvento { get; set; }
        public DateTime DataOra { get; set; }
        public string Evento { get; set; }
        public string NomeImmagine { get; set; }
        //public decimal Zona1 { get; set; }
        //public decimal Zona2 { get; set; }
        //public decimal Zona3 { get; set; }
        //public decimal Zona4 { get; set; }
        //public decimal TotaleZone { get; set; }
        //public byte[] imgEsaminata { get; set; }
        public bool ScaricheElettriche { get; set; }
        public int ConteggioInvioSMS { get; set; }



        public static MeteoViewModel FromDataRows(DataRow EventiRow)
        {

            //Metodo che utilizzo per la mappatura tra i dati letti dal Db e il ViewModel
            MeteoViewModel meteoViewModel = new MeteoViewModel
            {
                IDEvento = Convert.ToInt32(EventiRow["IDEvento"]),
                DataOra = Convert.ToDateTime(EventiRow["DataOra"].ToString()),
                Evento = EventiRow["Evento"].ToString(),
                NomeImmagine = EventiRow["NomeImmagine"].ToString(),
                //Zona1 = Convert.ToDecimal(EventiRow["Zona1"]),
                //Zona2 = Convert.ToDecimal(EventiRow["Zona2"]),
                //Zona3 = Convert.ToDecimal(EventiRow["Zona3"]),
                //Zona4 = Convert.ToDecimal(EventiRow["Zona4"]),
                //TotaleZone = Convert.ToDecimal(EventiRow["TotaleZone"])

                //imgEsaminata = (byte[])EventiRow["ImgEsaminata"],
                ScaricheElettriche = Convert.ToBoolean(EventiRow["ScaricheElettriche"] == DBNull.Value ? false : EventiRow["ScaricheElettriche"]),
                ConteggioInvioSMS = Convert.ToInt32(EventiRow["ConteggioInvioSMS"] == DBNull.Value ? 0 : EventiRow["ConteggioInvioSMS"])
            };
            return meteoViewModel;
        }
    }














    //public class MeteoViewModel
    //{
    //    public long IDEvento { get; set; }
    //    public DateTime DataOra { get; set; }
    //    public string Evento { get; set; }
    //    public decimal Zona1 { get; set; }
    //    public decimal Zona2 { get; set; }
    //    public decimal Zona3 { get; set; }
    //    public decimal Zona4 { get; set; }
    //    public decimal TotaleZone { get; set; }
    //    //public byte[] imgEsaminata { get; set; }



    //    public static MeteoViewModel FromDataRows(DataRow EventiRow)
    //    {

    //        //Metodo che utilizzo per la mappatura tra i dati letti dal Db e il ViewModel
    //        MeteoViewModel meteoViewModel = new MeteoViewModel
    //        {
    //            IDEvento = Convert.ToInt32(EventiRow["IDEvento"]),
    //            DataOra = Convert.ToDateTime(EventiRow["DataOra"].ToString()),
    //            Evento = EventiRow["Evento"].ToString(),
    //            Zona1 = Convert.ToDecimal(EventiRow["Zona1"]==DBNull.Value?0: EventiRow["Zona1"]),
    //            Zona2 = Convert.ToDecimal(EventiRow["Zona2"] == DBNull.Value ? 0 : EventiRow["Zona2"]),
    //            Zona3 = Convert.ToDecimal(EventiRow["Zona3"] == DBNull.Value ? 0 : EventiRow["Zona3"]),
    //            Zona4 = Convert.ToDecimal(EventiRow["Zona4"] == DBNull.Value ? 0 : EventiRow["Zona4"]),
    //            TotaleZone = Convert.ToDecimal(EventiRow["TotaleZone"] == DBNull.Value ? 0 : EventiRow["TotaleZone"])
    //            //imgEsaminata = (byte[])EventiRow["ImgEsaminata"]
    //        };
    //        return meteoViewModel;
    //    }
    //}
}
