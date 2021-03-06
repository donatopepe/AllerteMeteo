using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MeteoAlert.Entities;
using MeteoAlert.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace MeteoAlert.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailConfiguration;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailConfiguration = emailSettings.Value;
        }
        public List<EmailMessage> ReceiveEmail(int maxCount = 20)
        {
            using (var emailClient = new Pop3Client())
            {
                emailClient.CheckCertificateRevocation = false;
                emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                emailClient.Connect(_emailConfiguration.PopServer, _emailConfiguration.PopPort, MailKit.Security.SecureSocketOptions.Auto);// , _emailConfiguration.PopSSL=="false"?false:true);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfiguration.PopUsername, _emailConfiguration.PopPassword);

                List<EmailMessage> emails = new List<EmailMessage>();
                //for (int i = 0; i < emailClient.Count && i < maxCount; i++)
                //_logger.LogInformation("emailClient.Count: " + emailClient.Count + " maxCount: "+ maxCount);
                for (int i = emailClient.Count - 1; i >= (emailClient.Count - maxCount) && i >= 1; i--)
                {
                    //_logger.LogInformation("read email num: " + i);
                    var message = emailClient.GetMessage(i);
                    var emailMessage = new EmailMessage
                    {
                        Subject = message.Subject,
                        MessageNumber = i,
                        DateSent = message.Date.UtcDateTime
                    };
                    if (!string.IsNullOrEmpty(message.HtmlBody))
                    {
                        var sanitizer = new HtmlSanitizer();
                        //sanitizer.AllowedAttributes.Add("class");
                        var sanitized = sanitizer.Sanitize(message.HtmlBody);
                        emailMessage.Content = sanitized;
                        emailMessage.ContentType = "html";
                    }
                    else
                    {
                        emailMessage.Content = message.TextBody;
                        emailMessage.ContentType = "text";
                    }
                    MemoryStream mail = new MemoryStream();
                    message.WriteTo(mail);
                    emailMessage.OriginalMail.Mail = mail.ToArray();
                    emailMessage.OriginalMail.DateSent = emailMessage.DateSent;
                    emailMessage.OriginalMail.EmailMessage = emailMessage;

                    foreach (var attachment in message.Attachments)
                    {

                        using (MemoryStream stream = new MemoryStream())
                        {


                            string fileName = "attached-message.eml";
                            if (attachment is MessagePart)
                            {
                                fileName = attachment.ContentDisposition?.FileName;
                                var rfc822 = (MessagePart)attachment;

                                if (string.IsNullOrEmpty(fileName))



                                    rfc822.Message.WriteTo(stream);
                            }
                            else
                            {
                                var part = (MimePart)attachment;
                                fileName = part.FileName;


                                part.Content.DecodeTo(stream);
                            }
                            //_logger.LogInformation(">>>>>>>>>>>>> allegato " + fileName);
                            emailMessage.Attachments.Add(new Attachment
                            {
                                Content = stream.ToArray(),
                                FileName = fileName,
                                ContentType = attachment.ContentType.MediaType,
                                DateSent = emailMessage.DateSent

                            });
                        }
                    }
                    //emailMessage.ToAddresses.AddRange(message.To.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                    //emailMessage.FromAddresses.AddRange(message.From.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                    //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Mail:" + message.Subject);
                    //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>to Mail:" + message.To.Mailboxes.FirstOrDefault().Address);
                    //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>from Mail:" + message.From.Mailboxes.FirstOrDefault().Address);
                    emailMessage.ToAddresses = message.To.Mailboxes.FirstOrDefault().Address;
                    emailMessage.FromAddresses = message.From.Mailboxes.FirstOrDefault().Address;
                    emails.Add(emailMessage);
                    //_logger.LogInformation(">>>>>>>>>>>>> allegati n: " + emailMessage.Attachments.Count());
                }

                return emails;
            }
        }

        //public void Send(EmailMessage emailMessage)
        //{
        //    var message = new MimeMessage();
        //    message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
        //    message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

        //    message.Subject = emailMessage.Subject;
        //    //We will say we are sending HTML. But there are options for plaintext etc. 
        //    message.Body = new TextPart(TextFormat.Html)
        //    {
        //        Text = emailMessage.Content
        //    };

        //    //Be careful that the SmtpClient class is the one from Mailkit not the framework!
        //    using (var emailClient = new SmtpClient())
        //    {
        //        //The last parameter here is to use SSL (Which you should!)
        //        emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, true);

        //        //Remove any OAuth functionality as we won't be using it. 
        //        emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

        //        emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

        //        emailClient.Send(message);

        //        emailClient.Disconnect(true);
        //    }
        //}
        private string StripHTML(string source)
        {
            try
            {
                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch
            {
                //MessageBox.Show("Error");
                return source;
            }
        }
    }
}
