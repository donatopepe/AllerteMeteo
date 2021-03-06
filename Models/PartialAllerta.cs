using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{
    public partial class Allerta
    {
        
        public DateTime DateConvertion(string Input)
        {
            DateTime DateValue = DateTime.ParseExact(Input, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            return DateValue;
        }

        [NotMapped]
        [Display(Name = "Scad[data]")]
        public DateTime HourOfDay
        {
            get
            {

                DateTime DateValue = DateConvertion(this.Data);

                DateTime oracalcolata = DateValue.AddHours(Int32.Parse(Ora)).ToLocalTime();
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
    }
}
