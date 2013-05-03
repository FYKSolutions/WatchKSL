using System;
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

namespace WatchKSL
{
    public partial class Form1 : Form
    {
        private string searchWord, url, htmlResult, emailContent;
        private List<SearchResult> searchResults;
        private StringBuilder sBuilder;

        public Form1()
        {
            #region Initialize the form

            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
            url = "http://www.ksl.com/index.php?nid=231&search=";
            searchResults = new List<SearchResult>();
            sBuilder = new StringBuilder();
            #endregion
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            assembleSearchWord();
            webBrowser1.Navigate(url + searchWord);
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
            searchEngine();     
        }

        private void searchEngine()
        {
            WebClient client = new WebClient();
            assembleSearchWord();
            htmlResult = client.DownloadString(url + searchWord);
            parseHTML(htmlResult);

            listBox1.DataSource = null;
            listBox1.DataSource = searchResults;
            listBox1.DisplayMember = "title";
            sendMail();
        }

        private void assembleSearchWord()
        {
            string[] list = textBoxKeyword.Text.Split(' ');
            searchWord = string.Join("+", list);

            if (textBoxPriceMin != null)
                searchWord += "&min_price=" + textBoxPriceMin.Text.ToString();
            if (textBoxPriceMax != null)
                searchWord += "&max_price=" + textBoxPriceMax.Text.ToString();
            searchWord += "&sort=1";

            #region unusedCode
            //searchWord += "&addisplay=%5BNOW-1HOURS+TO+NOW%5D&sort=1&userid=&markettype=sale&adsstate=&nocache=1&o_facetSelected=true&o_facetKey=ad+posted&o_facetVal=Last+Hour&viewSelect=list&viewNumResults=12&sort=1";
            //NameValueCollection postData = new NameValueCollection()
            //{
            //    {"search",searchWord}
            //}; 
            #endregion
        }

        private void parseHTML( string strToParse)
        {
            emailContent="";
            sBuilder.Clear();
            searchResults.Clear();

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
                    SearchResult searchResult=new SearchResult();
                         
                    if (titleNode != null)
                    {
                        doc.LoadHtml(titleNode.OuterHtml.ToString());
                        HtmlAgilityPack.HtmlNode linkNode = doc.DocumentNode.SelectSingleNode("//a[contains(@class,'listlink')]");
                        if (linkNode != null)
                        {
                            sBuilder.Append(string.Format("{0}{1}", linkNode.InnerText.TrimStart(), Environment.NewLine));
                            sBuilder.Append(string.Format("{0}{1}{2}","www.ksl.com/",linkNode.Attributes["href"].Value,Environment.NewLine));

                            searchResult.title=linkNode.InnerText.TrimStart();
                            searchResult.link=linkNode.Attributes["href"].Value;

                            doc.LoadHtml(detailBoxNode.OuterHtml.ToString());
                            HtmlAgilityPack.HtmlNode priceNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'price')]");
                            if (priceNode != null)
                            {
                                sBuilder.Append(string.Format("{0}{1}{2}", "Price: ", priceNode.InnerText.TrimStart().TrimEnd().Remove(priceNode.InnerText.TrimStart().TrimEnd().Length - 2, 2)
                                    , Environment.NewLine));                               
                                searchResult.price = priceNode.InnerText.TrimStart().TrimEnd().Remove(priceNode.InnerText.TrimStart().TrimEnd().Length - 2, 2);
                            }

                            HtmlAgilityPack.HtmlNode descNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'adDesc')]");
                            if (descNode != null)
                            {
                                sBuilder.Append(string.Format("{0}{1}{2}{3}", "Description: ", descNode.InnerText.TrimStart(), Environment.NewLine, Environment.NewLine));                                
                                searchResult.description=descNode.InnerText.TrimStart();
                            }

                            searchResults.Add(new SearchResult { title =searchResult.title , link =searchResult.link,price=searchResult.price,
                                description=searchResult.description});                            
                        }
                        emailContent = sBuilder.ToString();
                    }
                }
            }                       
        } 

        private void sendMail()
        {
            try
            {
                var fromAddress = new MailAddress("FYKsolutions@gmail.com", "FYK Solutions");
                var toAddress = new MailAddress(textBoxEmail.Text.ToString(), "Client");
                const string fromPassword = "RichardChen777"; //put your password here
                string subject = "Chen's list for "+textBoxKeyword.Text.ToString() 
                       +" Price Range From $"+textBoxPriceMin.Text.ToString()+" To $"+textBoxPriceMax.Text.ToString();

           
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
                MessageBox.Show("OOPS email can't be sent! \n" +e.ToString());
            }
        }
       
    }
}
