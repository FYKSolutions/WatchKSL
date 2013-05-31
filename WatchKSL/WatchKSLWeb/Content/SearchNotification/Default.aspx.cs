﻿using SearchEngineSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Content_SearchNotification_Default : System.Web.UI.Page
{
    SearchUser mySearchUser = new SearchUser()
    {
        UserName = HttpContext.Current.User.ToString(),
        DisplayName = HttpContext.Current.User.Identity.Name,
        EmailAddress = Membership.GetUser(HttpContext.Current.User.Identity.Name).Email
    };

    SearchEngine searchEngine = new SearchEngine();
    
    protected void Page_Load(object sender, EventArgs e)
    {
        SearchRequest mySearchRequest = new SearchRequest();
            mySearchRequest.OriginatingSearchUser = mySearchUser;
            mySearchRequest.Keywords.Add("motorcycle");

        searchEngine.ProcessSearchRequest(mySearchRequest);

        NotificationEmail searchResultNotification = new NotificationEmail(mySearchRequest);
        searchResultNotification.SendMailViaHotmail();
    }
}