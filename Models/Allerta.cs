using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{
    public partial class Allerta
    {
        [Key]
        public long Id { get; set; }
        public string Data { get; set; }
        public string Ora { get; set; }
        [Display(Name = "Precipitazioni totali 3 h precedente > 30 mm", Prompt = "Precipitazioni totali 3 h precedente > 30 mm", Description = "Precipitazioni totali 3 h precedente > 30 mm")]
        public string Prectot3h { get; set; }
        [Display(Name = "Precipitazioni convettive 1 h precedente > 20 mm", Prompt = "Precipitazioni convettive 1 h precedente > 20 mm", Description = "Precipitazioni convettive 1 h precedente > 20 mm")]
        public string Cp1h { get; set; }
        [Display(Name = "Vento a 10 m >= 55 km/h", Prompt = "Vento a 10 m >= 55 km/h", Description = "Vento a 10 m >= 55 km/h")]
        public string Ws10m { get; set; }
        [Display(Name = "Vento a 32 m >= 60 km/h", Prompt = "Vento a 32 m >= 60 km/h", Description = "Vento a 32 m >= 60 km/h")]
        public string Ws32m1 { get; set; }
        [Display(Name = "Vento a 32 m >= 72 km/h", Prompt = "Vento a 32 m >= 72 km/h", Description = "Vento a 32 m >= 72 km/hh")]
        public string Ws32m2 { get; set; }
        [Display(Name = "Vento a 57 m >= 60 km/h", Prompt = "Vento a 57 m >= 60 km/h", Description = "Vento a 57 m >= 60 km/h")]
        public string Ws57m1 { get; set; }
        [Display(Name = "Vento a 57 m >= 72 km/h", Prompt = "Vento a 57 m >= 72 km/h", Description = "Vento a 57 m >= 72 km/h")]
        public string Ws57m2 { get; set; }
        [Display(Name = "Presenza di temporali e rovesci (Sigww)", Prompt = "Presenza di temporali e rovesci (Sigww)", Description = "Presenza di temporali e rovesci (Sigww)")]
        public string Sigww { get; set; }
        [Display(Name = "Presenza di temporali e rovesci (Phenomena)", Prompt = "Presenza di temporali e rovesci (Phenomena)", Description = "Presenza di temporali e rovesci (Phenomena)")]
        public string Phenomena { get; set; }


        public EmailMessage EmailMessage { get; set; }
        public DateTime DateSent { get; set; }
    }
}
