using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{
    public class Contatto
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nome completo")]
        public string Name { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Matricola")]
        public string Matricola { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Mansione")]
        public string Mansione { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Numero cellulare primario")]
        public string MobilePhone1 { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Numero cellulare secondario")]
        public string MobilePhone2 { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }
}
