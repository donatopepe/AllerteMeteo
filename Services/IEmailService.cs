using MeteoAlert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Services
{
    public interface IEmailService
    {
        //void Send(EmailMessage emailMessage);
        List<EmailMessage> ReceiveEmail(int maxCount = 20);
    }
}
