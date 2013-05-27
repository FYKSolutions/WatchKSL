using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using HtmlAgilityPack;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;



namespace SearchEngineSystem
{
    public class SearchEngine
    {
        private StringBuilder sBuilder;
        private string HtmlResult { get; set; }
        private List<SearchResult> SearchResults { get; set; }

        public string Keyword { get; set; }
        public string PriceMin { get; set; }
        public string PriceMax { get; set; }
        public string EmailAddress { get; set; }

        private string searchWord;
        private string url;
        private string emailContent;
        private WatchKSLEntities myDataContext;

        public SearchEngine ()
        {
            #region Initialize the form
            url = "http://www.ksl.com/index.php?nid=231&search=";
            SearchResults = new List<SearchResult>();
            sBuilder = new StringBuilder();
            myDataContext = new WatchKSLEntities();
            #endregion
        }

        public List<SearchResult> Search()
        {
            WebClient client = new WebClient();
            AssembleSearchWord();
            HtmlResult = client.DownloadString(url + searchWord);
            ParseHTML(HtmlResult);
            CustomerTransaction();
            return (SearchResults);
        }

        private void AssembleSearchWord()
        {
            string[] list = Keyword.Split(' ');
            searchWord = string.Join("+", list);

            if (PriceMin != null)
                searchWord += "&min_price=" + PriceMin;
            if (PriceMax != null)
                searchWord += "&max_price=" + PriceMax;
            searchWord += "&sort=1";

            #region unusedCode
            //searchWord += "&addisplay=%5BNOW-1HOURS+TO+NOW%5D&sort=1&userid=&markettype=sale&adsstate=&nocache=1&o_facetSelected=true&o_facetKey=ad+posted&o_facetVal=Last+Hour&viewSelect=list&viewNumResults=12&sort=1";
            //NameValueCollection postData = new NameValueCollection()
            //{
            //    {"search",searchWord}
            //}; 
            #endregion
        }
        private void ParseHTML(string strToParse)
        {
            //EmailContent="";
            //sBuilder.Clear();
            SearchResults.Clear();

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(strToParse);
            HtmlAgilityPack.HtmlNodeCollection detailBoxNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains( @class,'detailBox')]");

            if (detailBoxNodes != null)
            {

                foreach (HtmlNode detailBoxNode in detailBoxNodes)
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(detailBoxNode.OuterHtml.ToString());
                    HtmlAgilityPack.HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'adTitle')]");
                    SearchResult searchResult = new SearchResult();

                    if (titleNode != null)
                    {
                        doc.LoadHtml(titleNode.OuterHtml.ToString());
                        HtmlAgilityPack.HtmlNode linkNode = doc.DocumentNode.SelectSingleNode("//a[contains(@class,'listlink')]");
                        if (linkNode != null)
                        {
                            // sBuilder.Append(string.Format("{0}{1}", linkNode.InnerText.TrimStart(), Environment.NewLine));
                            // sBuilder.Append(string.Format("{0}{1}{2}","www.ksl.com/",linkNode.Attributes["href"].Value,Environment.NewLine));

                            searchResult.title = linkNode.InnerText.TrimStart();
                            searchResult.link = linkNode.Attributes["href"].Value;

                            doc.LoadHtml(detailBoxNode.OuterHtml.ToString());
                            HtmlAgilityPack.HtmlNode priceNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'price')]");
                            if (priceNode != null)
                            {
                                //sBuilder.Append(string.Format("{0}{1}{2}", "Price: ", priceNode.InnerText.TrimStart().TrimEnd().Remove(priceNode.InnerText.TrimStart().TrimEnd().Length - 2, 2)
                                //    , Environment.NewLine));                               
                                searchResult.price = priceNode.InnerText.TrimStart().TrimEnd().Remove(priceNode.InnerText.TrimStart().TrimEnd().Length - 2, 2);
                            }

                            HtmlAgilityPack.HtmlNode descNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'adDesc')]");
                            if (descNode != null)
                            {
                                //sBuilder.Append(string.Format("{0}{1}{2}{3}", "Description: ", descNode.InnerText.TrimStart(), Environment.NewLine, Environment.NewLine));                                
                                searchResult.description = descNode.InnerText.TrimStart();
                            }

                            SearchResults.Add(new SearchResult
                            {
                                title = searchResult.title,
                                link = searchResult.link,
                                price = searchResult.price,
                                description = searchResult.description
                            });
                        }
                        //EmailContent = sBuilder.ToString();
                    }
                }
            }
        }
        public void SendMail()
        {
            if (SearchResults.Count != 0)
            {
                AssembleEmailContent();
                try
                {
                    var fromAddress = new MailAddress(ConfigurationManager.AppSettings["serverEmail"], "FYK Solutions");
                    var toAddress = new MailAddress(EmailAddress, "Client");
                    string fromPassword = ConfigurationManager.AppSettings["serverEmailPassword"];
                    string subject = "Chen's list for " + Keyword
                           + " Price Range From $" + PriceMin + " To $" + PriceMax;


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
                        Body = emailContent
                    })
                    {
                        smtp.Send(message);
                    }
                }
                catch (SystemException e)
                {
                    MessageBox.Show("OOPS email can't be sent! \n" + e.ToString());
                }
            }
        }
        private void AssembleEmailContent()
        {
            sBuilder.Clear();
            emailContent = "";
            foreach (SearchResult searchResult in SearchResults)
            {
                sBuilder.Append(string.Format("{0}{1}", searchResult.title, Environment.NewLine));
                sBuilder.Append(string.Format("{0}{1}{2}", "www.ksl.com/", searchResult.link, Environment.NewLine));
                sBuilder.Append(string.Format("{0}{1}{2}", "Price: ", searchResult.price, Environment.NewLine));
                sBuilder.Append(string.Format("{0}{1}{2}{3}", "Description: ", searchResult.description, Environment.NewLine, Environment.NewLine));
            }
            emailContent = sBuilder.ToString();
        }

        public void CustomerTransaction()
        {
            string customerEmail = EmailAddress;

            ///See if the customer is in the database, if not add her
            if (!myDataContext.Customers.Any(o => o.Email == customerEmail))
            {
                myDataContext.Customers.Add(new Customer { Email = EmailAddress });
                myDataContext.SaveChanges();
            }

            Customer customer = (from c in myDataContext.Customers
                                 where c.Email == customerEmail
                                 select c).FirstOrDefault();

            ///Check if the keyword and price range is in the database. If not, add it. 
            if (!customer.SearchQueues.Any(o=> o.Keyword==Keyword && o.PriceMin==Convert.ToDouble(PriceMin)
                    && o.PriceMax==Convert.ToDouble(PriceMax)))
            {
                customer.SearchQueues.Add(new SearchQueue 
                    {PriceMax=Convert.ToDouble(PriceMax)
                    ,PriceMin=Convert.ToDouble(PriceMin)
                    ,Keyword=Keyword
                    ,QueueDate=DateTime.Now
                    ,Status=true
                    });
            }



            ///Now add search results to customer 
            //Check if the search result is already in the database. If not, add them. 

            List<SearchResult> itemsToDelete = new List<SearchResult>(); //first define a set for items to delete
            foreach (var resultItem in SearchResults)
            {
                // you can use this syntax of linq method if (customer.SearchResults.Where( c=>c.Description==searchResult.description)!=null)
                if (!customer.SearchResults.Any(c => c.Description == resultItem.description))
                {
                    customer.SearchResults.Add(new DataAccess.SearchResult
                    {
                        Description = resultItem.description,
                        Title = resultItem.title,
                        CreatedDate = DateTime.Now
                    });
                }
                else
                {
                    itemsToDelete.Add(resultItem);
                }

            }
            foreach (var item in itemsToDelete) SearchResults.Remove(item);
            myDataContext.SaveChanges();
        }
    }    
}
