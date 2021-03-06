using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{
    public class ContattoInviato : Contatto
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Risultato sms numero cellulare primario")]
        public string Risultato1 { get; set; }
        [Display(Name = "Risultato sms numero cellulare secondario")]
        public string Risultato2 { get; set; }
        [Display(Name = "Testo sms inviato")]
        public string Testosms { get; set; }


        public DateTime DateSent { get; set; }
    }
}
