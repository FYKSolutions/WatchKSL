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
        private string SearchUrl;
        protected WebClient client = new WebClient();
        private WatchKSLEntities myDataContext;

        public SearchEngine()
        {
            myDataContext = new WatchKSLEntities();
            SearchUrl = "http://www.ksl.com/index.php?nid=231&search=";
        }

        public void ProcessSearchRequests(List<SearchRequest> searchRequests)
        {
            foreach (SearchRequest searchRequest in searchRequests)
            {
                ProcessSearchRequest(searchRequest);
            }
        }

        public void ProcessSearchRequest(SearchRequest searchRequest)
        {
            string HtmlResult = string.Empty;

            try
            {
                searchRequest.SearchResultsHtml = client.DownloadString(string.Format("{0}{1}{2}", SearchUrl, searchRequest.GetFormattedKeywordString(), "&sort=1"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to download search result html for website: " + ex.Message);
            }

            try
            {
                searchRequest.SearchResultsFound = ParseHTML(searchRequest.SearchResultsHtml);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to parse search result html: " + ex.Message);
            }

            try
            {
                //ProcessCustomerTransaction(searchRequest);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to complete customer transaction: " + ex.Message);
            }
        }

        private List<SearchResult> ParseHTML(string strToParse)
        {
            List<SearchResult> searchResults = new List<SearchResult>();

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

                            searchResult.Title = linkNode.InnerText.TrimStart();
                            searchResult.Link = linkNode.Attributes["href"].Value;

                            doc.LoadHtml(detailBoxNode.OuterHtml.ToString());
                            HtmlAgilityPack.HtmlNode priceNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'price')]");
                            if (priceNode != null)
                            {
                                //sBuilder.Append(string.Format("{0}{1}{2}", "Price: ", priceNode.InnerText.TrimStart().TrimEnd().Remove(priceNode.InnerText.TrimStart().TrimEnd().Length - 2, 2)
                                //    , Environment.NewLine));                               
                                searchResult.Price = priceNode.InnerText.TrimStart().TrimEnd().Remove(priceNode.InnerText.TrimStart().TrimEnd().Length - 2, 2);
                            }

                            HtmlAgilityPack.HtmlNode descNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'adDesc')]");
                            if (descNode != null)
                            {
                                //sBuilder.Append(string.Format("{0}{1}{2}{3}", "Description: ", descNode.InnerText.TrimStart(), Environment.NewLine, Environment.NewLine));                                
                                searchResult.Description = descNode.InnerText.TrimStart();
                            }

                            searchResults.Add(new SearchResult
                            {
                                Title = searchResult.Title,
                                Link = searchResult.Link,
                                Price = searchResult.Price,
                                Description = searchResult.Description
                            });
                        }
                        //EmailContent = sBuilder.ToString();
                    }
                }
            }

            return searchResults;
        }

        public void ProcessCustomerTransaction(SearchRequest searchRequest)
        {
            string customerEmail = searchRequest.OriginatingSearchUser.EmailAddress;

            ///See if the customer is in the database, if not add her
            if (!myDataContext.Customers.Any(o => o.Email == customerEmail))
            {
                myDataContext.Customers.Add(new Customer { Email = searchRequest.OriginatingSearchUser.EmailAddress });
                myDataContext.SaveChanges();
            }

            Customer customer = (from c in myDataContext.Customers
                                 where c.Email == customerEmail
                                 select c).FirstOrDefault();

            ///Check if the keyword and price range is in the database. If not, add it. 
            if (!customer.SearchQueues.Any(o => o.Keyword == searchRequest.GetFormattedKeywordString() && o.PriceMin == Convert.ToDouble(searchRequest.PriceMin)
                    && o.PriceMax == Convert.ToDouble(searchRequest.PriceMax)))
            {
                customer.SearchQueues.Add(new SearchQueue
                {
                    PriceMax = Convert.ToDouble(searchRequest.PriceMax)
                    ,
                    PriceMin = Convert.ToDouble(searchRequest.PriceMin)
                    ,
                    Keyword = searchRequest.GetFormattedKeywordString()
                    ,
                    QueueDate = DateTime.Now
                    ,
                    Status = true
                });
            }

            ///Now add search results to customer 
            //Check if the search result is already in the database. If not, add them. 
            List<SearchResult> itemsToDelete = new List<SearchResult>(); //first define a set for items to delete
            foreach (var resultItem in searchRequest.SearchResultsFound)
            {
                // you can use this syntax of linq method if (customer.SearchResults.Where( c=>c.Description==searchResult.description)!=null)
                if (!customer.SearchResults.Any(c => c.Description == resultItem.Description))
                {
                    customer.SearchResults.Add(new DataAccess.SearchResult
                    {
                        Description = resultItem.Description,
                        Title = resultItem.Title,
                        CreatedDate = DateTime.Now
                    });
                }
                else
                {
                    itemsToDelete.Add(resultItem);
                }

            }

            foreach (var item in itemsToDelete)
            {
                searchRequest.SearchResultsFound.Remove(item);
            }

            myDataContext.SaveChanges();
        }
    }
}
