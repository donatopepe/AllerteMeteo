using MeteoAlert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Services
{
    public interface IAdoMeteoService
    {
        List<MeteoViewModel> GetEventi();

        string GetUltimoEventoLampinet(int nZona);

        bool ImageIsDifferent(string filename);

        int ReadInvioSMS();

        bool ReadMaxInvioSMS(int MaxSms);

        int daUltimaSequenza();

        bool isAllarmeInCorso();

    }
}
