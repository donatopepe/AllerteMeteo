using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{
    public class Rubrica : Contatto
    {
        [Key]
        public int Id { get; set; }


        [Display(Name = "Abilitazione invio sms")]
        public bool Sms { get; set; }

    }
}
