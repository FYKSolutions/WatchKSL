﻿<%@ Page Title="View Notifications" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ViewNotifications.aspx.cs" Inherits="Content_SearchNotification_ViewNotifications" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Use this page to view your notifications.</h2>
            </hgroup>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
</asp:Content>

