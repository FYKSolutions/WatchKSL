<%@ Page Title="Notification Manager" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Content_SearchNotification_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Use this page to manage your notifications.</h2>
            </hgroup>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ol class="round">
        <li>
            <h3>Create a new search notification</h3>
            <p style="margin-bottom: 96px;">
                <span style="width: 75px; height: 75px; float: left; display: block; margin: 12px;"><img src="Images/CreateNotifications.gif" alt="" style="width: 47px; height: 70px;" /></span>Use this page to create and test out any new notifications you want to create. Preview initial search results; as well as configure the notifications to meet your personal needs.
                <span style="display: block; font-size: 140%; margin-top: 12px;"><a href="Content/SearchNotification/CreateNotifications.aspx" title="">Create Notifications</a></span>
            </p>
            
            <h3>Modify an existing search notification</h3>
            <p style="margin-bottom: 96px;">
                <span style="width: 75px; height: 75px; float: left; display: block; margin: 12px;"><img src="Images/ModifyNotifications.gif" alt="" style="width: 59px; height: 54px;" /></span>Use this page to start, stop or modify any existing notifications you have previously saved. You will be able to renew your notifications as well as change the configuration of your notifications.
                <span style="display: block; font-size: 140%; margin-top: 12px;"><a href="Content/SearchNotification/ModifyNotifications.aspx" title="">Modify Notifications</a></span>
            </p>

            <h3>View online results of your search notifications</h3>
            <p style="margin-bottom: 96px;">
                <span style="width: 75px; height: 75px; float: left; display: block; margin: 12px;"><img src="Images/ViewNotifications.gif" alt="" style="width: 74px; height: 52px;" /></span>Use this page to view all the results from your search notifications in case you missed one.
                <span style="display: block; font-size: 140%; margin-top: 12px;"><a href="Content/SearchNotification/ViewNotifications.aspx" title="">View Notifications</a></span>
            </p>

            <h3>Delete an existing search notification</h3>
            <p style="margin-bottom: 96px;">
                <span style="width: 75px; height: 75px; float: left; display: block; margin: 12px;"><img src="Images/DeleteNotifications.gif" alt="" style="width: 59px; height: 54px;" /></span>Found what you were looking for with your notification? No longer needed notifications may be deleted from this page.
                <span style="display: block; font-size: 140%; margin-top: 12px;"><a href="Content/SearchNotification/DeleteNotifications.aspx" title="">Delete Notifications</a></span>
            </p>
        </li>
    </ol>
</asp:Content>

