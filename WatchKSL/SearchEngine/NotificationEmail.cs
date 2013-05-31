using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchEngineSystem
{
    public class NotificationEmail
    {
        private SearchRequest searchRequest;

        public NotificationEmail(SearchRequest searchRequest)
        {
            this.searchRequest = searchRequest;
        }

        public void SendMailViaHotmail()
        {
            if (ConfigurationManager.AppSettings["serverEmail"] == null && ConfigurationManager.AppSettings["serverEmailPassword"] == null)
            {
                MessageBox.Show("No email server configuration application settings found.");
                return;
            }

            if (searchRequest.SearchResultsFound.Count > 0)
            {
                try
                {
                    SmtpClient SmtpServer = new SmtpClient("smtp.live.com");

                    var mail = new MailMessage();
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["serverEmail"]);
                    mail.To.Add(searchRequest.OriginatingSearchUser.EmailAddress);
                    mail.Subject = "New search results found for you!";
                    
                    //mail.IsBodyHtml = false;
                    //mail.Body = AssembleEmailContentAsText(searchRequest.SearchResultsFound);

                    mail.IsBodyHtml = true;
                    mail.Body = AssembleEmailContentAsHtml(searchRequest.SearchResultsFound);

                    SmtpServer.Port = 587;
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["serverEmail"], ConfigurationManager.AppSettings["serverEmailPassword"]);
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
                catch (SystemException e)
                {
                    MessageBox.Show("OOPS email can't be sent via hotmail.com! \n" + e.ToString());
                }
            }
        }

        public void SendMailViaGMail()
        {
            if (searchRequest.SearchResultsFound.Count > 0)
            {
                try
                {
                    var fromAddress = new MailAddress(ConfigurationManager.AppSettings["serverEmail"], "FYK Solutions");
                    var toAddress = new MailAddress(searchRequest.OriginatingSearchUser.EmailAddress, "Client");
                    string fromPassword = ConfigurationManager.AppSettings["serverEmailPassword"];
                    string subject = "Chen's list for " + searchRequest.GetFormattedKeywordString()
                           + " Price Range From $" + searchRequest.PriceMin + " To $" + searchRequest.PriceMax;
                    
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };

                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = AssembleEmailContentAsText(searchRequest.SearchResultsFound)
                    })
                    {
                        smtp.Send(message);
                    }
                }
                catch (SystemException e)
                {
                    MessageBox.Show("OOPS email can't be sent via gmail.com! \n" + e.ToString());
                }
            }
        }

        private string AssembleEmailContentAsText(List<SearchResult> searchResults)
        {
            StringBuilder sBuilder = new StringBuilder();

            foreach (SearchResult searchResult in searchResults)
            {
                sBuilder.Append(string.Format("{0}{1}", searchResult.Title, Environment.NewLine));
                sBuilder.Append(string.Format("{0}{1}{2}", "www.ksl.com/", searchResult.Link, Environment.NewLine));
                sBuilder.Append(string.Format("{0}{1}{2}", "Price: ", searchResult.Price, Environment.NewLine));
                sBuilder.Append(string.Format("{0}{1}{2}{3}", "Description: ", searchResult.Description, Environment.NewLine, Environment.NewLine));
            }

            return sBuilder.ToString();
        }

        private string AssembleEmailContentAsHtml(List<SearchResult> searchResults)
        {
            string brTag = "<br />";
            StringBuilder sBuilder = new StringBuilder();

            foreach (SearchResult searchResult in searchResults)
            {
                sBuilder.Append(string.Format("{0}{1}", searchResult.Title, brTag));
                sBuilder.Append(string.Format("{0}{1}{2}", "www.ksl.com/", searchResult.Link, brTag));
                sBuilder.Append(string.Format("{0}{1}{2}", "Price: ", searchResult.Price, brTag));
                sBuilder.Append(string.Format("{0}{1}{2}{3}", "Description: ", searchResult.Description, brTag, brTag));
            }

            return sBuilder.ToString();
        }


    }
}
