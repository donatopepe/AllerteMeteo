using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{
    public partial class Bollettino
    {
        [Key]
        public long Id { get; set; }

        //Colonne csv:Sito Latitudine Longitudine Elevazione Scadh Tp3h Tp3h30mm Cp1h Cp1h20mm Wind10m Wind10m1544ms Wind32m Wind32m1667ms Wind32m20ms Wind57m Wind57m1667ms Wind57m20ms Ww Phenomena 
        public string Sito { get; set; }
        public string Latitudine { get; set; }
        public string Longitudine { get; set; }
        public string Elevazione { get; set; }
        [Display(Name = "Scad[h]")]
        public string Scadh { get; set; }
        public string Tp3h { get; set; }
        [Display(Name = ">30mm")]
        public string Tp3h30mm { get; set; }
        public string Cp1h { get; set; }
        [Display(Name = ">20mm")]
        public string Cp1h20mm { get; set; }
        public string Wind10m { get; set; }
        [Display(Name = ">15.44m/s")]
        public string Wind10m1544ms { get; set; }
        public string Wind32m { get; set; }
        [Display(Name = ">16.67m/s")]
        public string Wind32m1667ms { get; set; }
        [Display(Name = ">20m/s")]
        public string Wind32m20ms { get; set; }
        public string Wind57m { get; set; }
        [Display(Name = ">16.67m/s")]
        public string Wind57m1667ms { get; set; }
        [Display(Name = ">20m/s")]
        public string Wind57m20ms { get; set; }
        public string Ww { get; set; }
        public string Phenomena { get; set; }

        public EmailMessage EmailMessage { get; set; }
        public DateTime DateSent { get; set; }
    }
}
