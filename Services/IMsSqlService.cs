using System;
using System.Data;
using System.Drawing;

namespace MeteoAlert.Services
{
    public interface IMsSqlService
    {
        DataSet Query(string QRY);

        double ReturnDoubleFromDataReader(string QRY);

        void ExecuteCommand(string QRY);

        //void ExecuteQueryMeteo(string QRY, DateTime DataOra, string Evento, double Zona1, double Zona2, double Zona3, double Zona4, double TotaleZone, Bitmap ImgEsaminata, string NomeFile);
        void ExecuteQueryMeteo(string QRY, DateTime DataOra, string Evento, Bitmap ImgEsaminata, string NomeFile, int ScaricheElettriche, int ConteggioInvioSMS);

    }
}