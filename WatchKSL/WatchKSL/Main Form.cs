﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net.Mail;
using System.Configuration;
using DataAccess;
using SearchEngineSystem;
using System.Threading;


namespace WatchKSL
{
    public partial class Form1 : Form
    {
        //private StringBuilder sBuilder;
        //private string HtmlResult { get; set; }
        private List<SearchEngineSystem.SearchResult> SearchResults {get;set;}

        public string SearchWord { get; set; }
        public string Url { get; set; }
        //public string EmailContent { get; set; }
        public WatchKSLEntities MyDataContext {get;set;}
        public System.Timers.Timer timer;
                    

        public Form1()
        {
            #region Initialize the form
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
            Url = "http://www.ksl.com/index.php?nid=231&search=";
            SearchResults = new List<SearchEngineSystem.SearchResult>();
            //sBuilder = new StringBuilder();
            MyDataContext=new WatchKSLEntities();            
            #endregion
        }
        private void TimerLoop()
        {
            timer = new System.Timers.Timer(120000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerTick); // Everytime timer ticks, TimerTick will be called
            timer.Interval = 120000;
            timer.Enabled = true;                       // Enable the timer
            timer.Start();  
        }


        private void buttonStart_Click(object sender, EventArgs e)
        {
            AssembleSearchWord();
            webBrowser1.Navigate(Url + SearchWord);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.Document.GetElementById("search_form").InvokeMember("click");
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonPHPPosting_Click(object sender, EventArgs e)
        {
            SearchEngine searchEngine = new SearchEngine();
            searchEngine.Keyword = textBoxKeyword.Text.ToString();
            searchEngine.PriceMax = textBoxPriceMax.Text.ToString();
            searchEngine.PriceMin = textBoxPriceMin.Text.ToString();
            searchEngine.EmailAddress = textBoxEmail.Text.ToString();
            
            SearchResults =searchEngine.Search();
            searchEngine.SendMail();
            listBox1.DataSource = null;
            listBox1.DataSource = SearchResults;
            listBox1.DisplayMember = "title";

        }

        void TimerTick(object sender, EventArgs e)
        {
            List<SearchQueue> searchItem= new List<SearchQueue>();

            searchItem=(from o in MyDataContext.SearchQueues
                       where o.Status==true
                       select o).ToList();
            foreach (var item in searchItem)
            {
                //or you can use ParameterizedThreadStart instead
                //Thread t = new Thread(()=>SearchJob(item.Keyword,item.PriceMin,item.PriceMax,item.Customer.Email)); 
               // t.Start();

                SearchJob(item.Keyword, item.PriceMin, item.PriceMax, item.Customer.Email);
            }
        }

        void SearchJob(string keyword, Nullable<double> priceMin, Nullable<double> priceMax, string email)
        {
            SearchEngine searchEngine=new SearchEngine();
            searchEngine.Keyword = keyword;
            searchEngine.PriceMax = priceMax.ToString();
            searchEngine.PriceMin = priceMin.ToString();
            searchEngine.EmailAddress = email;
            SearchResults = searchEngine.Search();
            searchEngine.SendMail();
        }

        //public void SearchEngine()
        //{
        //    WebClient client = new WebClient();
        //    AssembleSearchWord();
        //    HtmlResult = client.DownloadString(Url + SearchWord);
        //    ParseHTML(HtmlResult);
        //    listBox1.DataSource = null;
        //    listBox1.DataSource = SearchResults;
        //    listBox1.DisplayMember = "title";

        //    CustomerTransaction();
        //}

        private void AssembleSearchWord()
        {
            string[] list = textBoxKeyword.Text.Split(' ');
            SearchWord = string.Join("+", list);

            if (textBoxPriceMin != null)
                SearchWord += "&min_price=" + textBoxPriceMin.Text.ToString();
            if (textBoxPriceMax != null)
                SearchWord += "&max_price=" + textBoxPriceMax.Text.ToString();
            SearchWord += "&sort=1";
            #region unusedCode
            //searchWord += "&addisplay=%5BNOW-1HOURS+TO+NOW%5D&sort=1&userid=&markettype=sale&adsstate=&nocache=1&o_facetSelected=true&o_facetKey=ad+posted&o_facetVal=Last+Hour&viewSelect=list&viewNumResults=12&sort=1";
            //NameValueCollection postData = new NameValueCollection()
            //{
            //    {"search",searchWord}
            //}; 
            #endregion
        }

        private void buttonStartTimer_Click(object sender, EventArgs e)
        {
            TimerLoop();
        }

        //private void ParseHTML( string strToParse)
        //{
        //    //EmailContent="";
        //    //sBuilder.Clear();
        //    SearchResults.Clear();

        //    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
        //    htmlDoc.LoadHtml(strToParse);
        //    HtmlAgilityPack.HtmlNodeCollection detailBoxNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains( @class,'detailBox')]");

        //    if (detailBoxNodes != null)
        //    {

        //        foreach (HtmlNode detailBoxNode in detailBoxNodes)
        //        {
        //            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //            doc.LoadHtml(detailBoxNode.OuterHtml.ToString());
        //            HtmlAgilityPack.HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'adTitle')]");
        //            SearchResult searchResult=new SearchResult();
                         
        //            if (titleNode != null)
        //            {
        //                doc.LoadHtml(titleNode.OuterHtml.ToString());
        //                HtmlAgilityPack.HtmlNode linkNode = doc.DocumentNode.SelectSingleNode("//a[contains(@class,'listlink')]");
        //                if (linkNode != null)
        //                {
        //                   // sBuilder.Append(string.Format("{0}{1}", linkNode.InnerText.TrimStart(), Environment.NewLine));
        //                   // sBuilder.Append(string.Format("{0}{1}{2}","www.ksl.com/",linkNode.Attributes["href"].Value,Environment.NewLine));

        //                    searchResult.title=linkNode.InnerText.TrimStart();
        //                    searchResult.link=linkNode.Attributes["href"].Value;

        //                    doc.LoadHtml(detailBoxNode.OuterHtml.ToString());
        //                    HtmlAgilityPack.HtmlNode priceNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'price')]");
        //                    if (priceNode != null)
        //                    {
        //                        //sBuilder.Append(string.Format("{0}{1}{2}", "Price: ", priceNode.InnerText.TrimStart().TrimEnd().Remove(priceNode.InnerText.TrimStart().TrimEnd().Length - 2, 2)
        //                        //    , Environment.NewLine));                               
        //                        searchResult.price = priceNode.InnerText.TrimStart().TrimEnd().Remove(priceNode.InnerText.TrimStart().TrimEnd().Length - 2, 2);
        //                    }

        //                    HtmlAgilityPack.HtmlNode descNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'adDesc')]");
        //                    if (descNode != null)
        //                    {
        //                        //sBuilder.Append(string.Format("{0}{1}{2}{3}", "Description: ", descNode.InnerText.TrimStart(), Environment.NewLine, Environment.NewLine));                                
        //                        searchResult.description=descNode.InnerText.TrimStart();
        //                    }

        //                    SearchResults.Add(new SearchResult { title =searchResult.title , link =searchResult.link,price=searchResult.price,
        //                        description=searchResult.description});                            
        //                }
        //                //EmailContent = sBuilder.ToString();
        //            }
        //        }
        //    }                       
        //} 

        //public void SendMail()
        //{
        //    AssembleEmailContent();
        //    try
        //    {
        //        var fromAddress = new MailAddress( ConfigurationManager.AppSettings["serverEmail"], "FYK Solutions");
        //        var toAddress = new MailAddress(textBoxEmail.Text.ToString(), "Client");
        //        string fromPassword = ConfigurationManager.AppSettings["serverEmailPassword"];
        //        string subject = "Chen's list for "+textBoxKeyword.Text.ToString() 
        //               +" Price Range From $"+textBoxPriceMin.Text.ToString()+" To $"+textBoxPriceMax.Text.ToString();

           
        //        var smtp = new SmtpClient
        //        {
        //            Host = "smtp.gmail.com",
        //            Port = 587,
        //            EnableSsl = true,
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            UseDefaultCredentials = false,
        //            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        //        };
        //        using (var message = new MailMessage(fromAddress, toAddress)
        //        {
        //            Subject = subject,
        //            Body = EmailContent
        //        })
        //        {
        //            smtp.Send(message);
        //        }
        //    }
        //    catch (SystemException e)
        //    {
        //        MessageBox.Show("OOPS email can't be sent! \n" +e.ToString());
        //    }
        //}

        //private void AssembleEmailContent()
        //{
        //    sBuilder.Clear();
        //    EmailContent = "";
        //    foreach (SearchResult searchResult in SearchResults)
        //    {
        //        sBuilder.Append(string.Format("{0}{1}", searchResult.title, Environment.NewLine));
        //        sBuilder.Append(string.Format("{0}{1}{2}", "www.ksl.com/", searchResult.link, Environment.NewLine));
        //        sBuilder.Append(string.Format("{0}{1}{2}", "Price: ", searchResult.price, Environment.NewLine));
        //        sBuilder.Append(string.Format("{0}{1}{2}{3}", "Description: ", searchResult.description, Environment.NewLine, Environment.NewLine));
        //    }
        //    EmailContent = sBuilder.ToString();
        //}

        //public void CustomerTransaction()
        //{
        //    string customerEmail = textBoxEmail.Text.ToString();

        //    if (!MyDataContext.Customers.Any(o => o.Email == customerEmail))
        //    {
        //        MyDataContext.Customers.Add(new Customer { Email = textBoxEmail.Text.ToString() });
        //        MyDataContext.SaveChanges();
        //    }
        //    Customer customer = (from c in MyDataContext.Customers
        //                        where c.Email == customerEmail
        //                        select c).FirstOrDefault();
            
        //    //Now check if the search result is already in the database

        //    List<SearchResult> itemsToDelete= new List<SearchResult>(); //first define a set for items to delete
        //    foreach (var resultItem in SearchResults)
        //    {
        //       // you can use this syntax of linq method if (customer.SearchResults.Where( c=>c.Description==searchResult.description)!=null)
        //        if (!customer.SearchResults.Any(c => c.Description == resultItem.description))
        //        {
        //            customer.SearchResults.Add(new DataAccess.SearchResult
        //                                    {
        //                                        Description = resultItem.description,
        //                                        Title = resultItem.title,
        //                                        CreatedDate = DateTime.Now
        //                                    });
        //        }
        //        else
        //        {
        //            itemsToDelete.Add(resultItem);
        //        }
               
        //    }
        //    foreach (var item in itemsToDelete) SearchResults.Remove(item);
        //    MyDataContext.SaveChanges();

        //}

    }
}
