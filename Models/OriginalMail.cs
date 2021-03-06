using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{
    public class OriginalMail
    {
        [Key]
        public Guid Id { get; set; }
        public byte[] Mail { get; set; }
        public DateTime DateSent { get; set; }
        public int EmailMessageId { get; set; }
        public EmailMessage EmailMessage { get; set; }

    }
}
