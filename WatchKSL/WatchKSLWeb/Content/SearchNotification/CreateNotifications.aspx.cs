using SearchEngineSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Content_SearchNotification_CreateNotifications : System.Web.UI.Page
{
    SearchUser mySearchUser = new SearchUser()
    {
        UserName = HttpContext.Current.User.ToString(),
        DisplayName = HttpContext.Current.User.Identity.Name,
        EmailAddress = Membership.GetUser(HttpContext.Current.User.Identity.Name).Email
    };

    SearchEngine searchEngine = new SearchEngine();
    
    List<string> CurrentKeywords {
        get {
                if (Session["CurrentKeywords"] == null) {
                    Session["CurrentKeywords"] = new List<string>();
                }

            return (List<string>)Session["CurrentKeywords"];
        }

        set
        {
            Session["CurrentKeywords"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        BindKeywords();
    }

    private void BindKeywords()
    {
        lbKeywords.DataSource = CurrentKeywords;
        lbKeywords.DataBind();
    }

    protected void btnAddKeyword_Click(object sender, EventArgs e)
    {
        string keyword = tbKeyword.Text.Trim();
        if (!string.IsNullOrEmpty(keyword))
        {
            if (!CurrentKeywords.Contains(keyword))
            {
                //Add and display new keyword
                CurrentKeywords.Add(keyword);
                BindKeywords();

                //clear the keyword once added
                tbKeyword.Text = string.Empty;
            }
            else
            {
                //Error Message: Can't enter same keyword twice
            }
        }
        else
        {
            //Error Message: No keyword entered in keyword box
        }
    }

    protected void btnPerformSearch_Click(object sender, EventArgs e)
    {
        //Construct a search request
        SearchRequest mySearchRequest = new SearchRequest();
            mySearchRequest.OriginatingSearchUser = mySearchUser;   //who is performing the search
            mySearchRequest.Keywords.AddRange(CurrentKeywords);     //what are they searching for?

        //Process the search request, results are attached to the search request
        searchEngine.ProcessSearchRequest(mySearchRequest);

        BindNotificationResults(mySearchRequest);
    }

    private void BindNotificationResults(SearchRequest mySearchRequest)
    {
        rptNotificationResults.DataSource = mySearchRequest.SearchResultsFound;
        rptNotificationResults.DataBind();
    }

    protected void btnRemoveKeyword_Click(object sender, EventArgs e)
    {
        List<string> cleanedKeywords = CurrentKeywords;
        foreach (ListItem item in lbKeywords.Items)
        {
            if (item.Selected)
            {
                cleanedKeywords.Remove(item.Value);
            }
        }

        CurrentKeywords = cleanedKeywords;
        BindKeywords();
    }
}