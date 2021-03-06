using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{
    public partial class Bollettino
    {
        [NotMapped]
        [Display(Name = "Scad[data]")]
        public DateTime HourOfDay
        {
            get
            {
                int orainizio = 0;
                if ((this.DateSent.Hour >= 0) & (this.DateSent.Hour < 12))
                {
                    orainizio = 0;
                }
                else
                {
                    orainizio = 12;
                }


                DateTime oracalcolata = this.DateSent.Date.AddHours(orainizio + Int32.Parse(Scadh)).ToLocalTime();
                return oracalcolata;



            }
        }
        [NotMapped]
        [Display(Name = "Data locale mail")]
        public DateTime DateSentLocal
        {
            get
            {
         
                return DateSent.ToLocalTime();



            }
        }
        public int Wind10m_dir
        {
            get
            {
                string stringBeforeChar = Wind10m.Substring(0, Wind10m.IndexOf("/"));
                int dir_gradi = 0;
                bool dir_ok = Int32.TryParse(stringBeforeChar, out dir_gradi);
                return dir_gradi;
            }
        }
        public int Wind10m_kmh
        {
            get
            {
                string stringAfterChar = Wind10m.Substring(Wind10m.IndexOf("/")+1);
                int vel_nodi = 0;
                bool vel_ok = Int32.TryParse(stringAfterChar, out vel_nodi);
                double  vel_kmh = vel_nodi * 1.852;

                //_logger.LogInformation(Wind10m+" converto Wind10m " + stringAfterChar + " m/s da " + vel_nodi + " a km/h " + vel_kmh);
                return (int)Math.Round(vel_kmh);
            }
        }
        [Display(Name = "Wind10m [°dir]/[km/h]")]
        public string Wind10m_dir_kmh
        {
            get
            {

                return Wind10m_dir + "/"+ Wind10m_kmh;
            }
        }

        public int Wind32m_dir
        {
            get
            {
                string stringBeforeChar = Wind32m.Substring(0, Wind32m.IndexOf("/"));
                int dir_gradi = 0;
                bool dir_ok = Int32.TryParse(stringBeforeChar, out dir_gradi);
                return dir_gradi;
            }
        }
        public int Wind32m_kmh
        {
            get
            {
                string stringAfterChar = Wind32m.Substring(Wind32m.IndexOf("/") + 1);
                int vel_nodi = 0;
                bool vel_ok = Int32.TryParse(stringAfterChar, out vel_nodi);
                double vel_kmh = vel_nodi * 3.6;
                
                //_logger.LogInformation("converto Wind32m " + stringAfterChar + " m/s da " + vel_nodi + " a km/h " + vel_kmh);
                return (int)Math.Round(vel_kmh);
            }
        }
        [Display(Name = "Wind32m [°dir]/[km/h]")]
        public string Wind32m_dir_kmh
        {
            get
            {

                return Wind32m_dir + "/" + Wind32m_kmh;
            }
        }

        public int Wind57m_dir
        {
            get
            {
                string stringBeforeChar = Wind57m.Substring(0, Wind57m.IndexOf("/"));
                int dir_gradi = 0;
                bool dir_ok = Int32.TryParse(stringBeforeChar, out dir_gradi);
                return dir_gradi;
            }
        }
        public int Wind57m_kmh
        {
            get
            {
                string stringAfterChar = Wind57m.Substring(Wind57m.IndexOf("/") + 1);
                int vel_nodi = 0;
                bool vel_ok = Int32.TryParse(stringAfterChar, out vel_nodi);
                double vel_kmh = vel_nodi * 3.6;
                //_logger.LogInformation("converto Wind57m "+ stringAfterChar + " m/s da " + vel_nodi + " a km/h " + vel_kmh);
                return (int)Math.Round(vel_kmh);
            }
        }
        [Display(Name = "Wind57m [°dir]/[km/h]")]
        public string Wind57m_dir_kmh
        {
            get
            {

                return Wind57m_dir + "/" + Wind57m_kmh;
            }
        }

    }
}
