using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

public class AllarmiSMS
{
    private readonly ILogger<AllarmiSMS> _logger;

    public string xmlns = "http://tempuri.org/";

    public AllarmiSMS(ILogger<AllarmiSMS> logger)
    {
        _logger = logger;
    }

    static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
    {
        var lines = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        List<string> darecon = new List<string>();
        foreach (string t in lines)
        {
            if (!String.IsNullOrWhiteSpace(t.Trim()))
            {
                darecon.Add(t.Trim());
            }
        }
        var reconstituted = String.Join(Environment.NewLine, darecon);
        for (int i = 0; i < reconstituted.Length; i += maxChunkSize)
            yield return reconstituted.Substring(i, Math.Min(maxChunkSize, reconstituted.Length - i));
    }

    public string ComponiInviaSMS(string testo, string cellulare)
    {
        if (!String.IsNullOrEmpty(testo) && !String.IsNullOrEmpty(cellulare))
        {
            IEnumerable<string> mytesto = ChunksUpto(testo.Trim(), 160);
            //_logger.LogInformation(mytesto.Count());
            string responso = "";
            foreach (string testosms in mytesto)
            {
                _logger.LogInformation(testosms);
                DateTime oggi = DateTime.Now.Date;
                string stringaXML = "<?xml version= \"1.0\" encoding= \"UTF-8\" ?>" +
                    "<XML>" +
                    "<TIPO_INVIO>SMS</TIPO_INVIO>" +
                    "<PREFIX_INTERN>true</PREFIX_INTERN>" +
                    "<OPERATORE>xxxx</OPERATORE>" +
                    "<SERVIZIO>xxxx</SERVIZIO>" +
                    "<MAM>ILVA@MAM</MAM>" +
                    "<DATA>" + oggi.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + "</DATA>" +
                    "<ORA>" + oggi.ToString("hh:mm", CultureInfo.InvariantCulture) + "</ORA>" +
                    "<TESTO>" + testosms.Trim() + "</TESTO>" +
                    "<DATA_INVIO/>" +
                    "<ORA_INVIO/>" +
                    "<INVIA_CON_ALIAS/>" +
                    "<ALIAS/>" +
                    "<PERIODO_VALIDITA_MINUTI/>" +
                    "<EL_CONTATTI>";

                stringaXML += "<N>" + cellulare.Trim() + "</N>";

                stringaXML += "</EL_CONTATTI></XML>";
                //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>><<<XML send sms");
                //_logger.LogInformation(stringaXML);
                byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(stringaXML);
                string vByte = Convert.ToBase64String(plainTextBytes);
                string soapMessage = "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                    "<soap:Body>" +
                    "<SendSMS xmlns=\"" + xmlns + "\">" +
                    "<sUtente>xxxxxx</sUtente>" +
                    "<sPassword>xxxxxx</sPassword>" +
                    "<vByte>" + vByte + "</vByte>" +
                    "<sCompany>xxxxxxx</sCompany> " +
                    "</SendSMS>" +
                    "</soap:Body>" +
                    "</soap:Envelope>";
                responso = responso + Invia(soapMessage) + " ";
            }
            return responso;
        }

        return "Errore dati sms assenti";


    }
    public string Invia(string soap)
    {
        string ritorna;
        try
        {
            //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>><<<Soap send sms");
            //_logger.LogInformation(soap);
            string url = "https://smservice.net.vodafone.it/wssmsgateway/smsgatewaysendsms.asmx";
            HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            webRequest.ContentType = "text/xml";
            WebProxy myProxy = new WebProxy();
            Uri newUri = new Uri("");
            myProxy.Address = newUri;
            webRequest.Proxy = myProxy;
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(soap);
            webRequest.ContentLength = bytes.Length;
            using (Stream writeStream = webRequest.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            string soapResult = sr.ReadToEnd();
            XNamespace ns = xmlns;
            XDocument xmlResult = XDocument.Parse(soapResult);
            //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>><<<Risultato xml send sms");
            //_logger.LogInformation(xmlResult);
            //var obj = from risiltato in xmlResult.Descendants(ns + "SendSMSResponse")
            string risultato = xmlResult.Descendants(ns + "SendSMSResult").First().Value;
            string messaggio = xmlResult.Descendants(ns + "ErrorMessage").First().Value;
            if (!String.IsNullOrEmpty(messaggio)) risultato = risultato + ": " + messaggio;
            ritorna = risultato;
            //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>><<<Risultato send sms");
            //_logger.LogInformation(ritorna);
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            // se vi sono errori va a scrivere un file .txt nella 
            // stessa directory dove si trova il file .aspx
            string logFile = "log_sms.txt";
            string fullLogFilePath = logFile;
            StreamWriter sw = new StreamWriter(fullLogFilePath, true);
            sw.WriteLine(DateTime.Now + ": " + ex.Message);
            sw.Close();
            ritorna = ex.Message;
        }
        return ritorna;
    }


}
