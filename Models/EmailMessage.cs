using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Models
{
    [Serializable]
    public partial class EmailMessage
    {
        [Key]
        public int Id { get; set; }
        public EmailMessage()
        {
            this.Attachments = new List<Attachment>();
            SmsInviati = new List<ContattoInviato>();
            //Bollettinos = new List<Bollettino>();
            OriginalMail = new OriginalMail();
        }
        public int MessageNumber { get; set; }
        public string ToAddresses { get; set; }
        public string FromAddresses { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public DateTime DateSent { get; set; }
        public List<Attachment> Attachments { get; set; }
        public OriginalMail OriginalMail { get; set; }
        public List<ContattoInviato> SmsInviati { get; set; }
        public List<Bollettino> Bollettinos { get; set; }
        public List<Allerta> Allertas { get; set; }
    }

}
