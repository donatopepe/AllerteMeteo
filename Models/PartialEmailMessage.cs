using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{
    public partial class EmailMessage
    {
        [NotMapped]
        [Display(Name = "Data locale")]
        public DateTime DateSentLocal
        {
            get
            {
                return DateSent.ToLocalTime();
            }
        }

    }
}
