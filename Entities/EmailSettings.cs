using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Entities
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpSSL { get; set; }
        public string SenderName { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string PopServer { get; set; }
        public int PopPort { get; set; }
        public string PopSSL { get; set; }
        public string PopUsername { get; set; }
        public string PopPassword { get; set; }
    }
}
