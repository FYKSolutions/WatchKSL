<%@ Page Title="Create Notifications" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CreateNotifications.aspx.cs" Inherits="Content_SearchNotification_CreateNotifications" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Use this page to create your notifications.</h2>
            </hgroup>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">

            <div style="width: 100%; margin: auto;">
                <h2>Create Search Notification</h2>
                <p>Give your search notification a name and begin refining your search by adding keywords to your notification. Enter one word at a time and you must have a least one keyword to perform a search. Detailed search use can be found here.</p>
                
                <hr />
                <table>
                    <tr>
                        <td>

                            <asp:Label ID="Label2" runat="server" Text="Notification Name:"></asp:Label>

                        </td>
                        <td>

                            <asp:TextBox ID="tbNotificationName" runat="server"></asp:TextBox>

                        </td>
                        <td>

                            <asp:Button ID="btnSaveNotification" runat="server" Text="Save Notification" />

                        </td>
                    </tr>
                    <tr>
                        <td>

                            <asp:Label ID="Label3" runat="server" Text="Notification Description:"></asp:Label>

                        </td>
                        <td>

                            <asp:TextBox ID="tbNotificationDescription" runat="server"></asp:TextBox>

                        </td>
                        <td>

                        </td>
                    </tr>
                </table>

                <hr />
                <table>
                    <tr valign="top">
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Keyword:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="tbKeyword" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnAddKeyword" runat="server" Text="Add Keyword" OnClick="btnAddKeyword_Click" />
                        </td>
                        <td rowspan="2">
                            <div style="text-align: right;">
                                <asp:Button ID="btnPerformSearch" runat="server" Text="Perform Search" OnClick="btnPerformSearch_Click" />
                            </div>
                            <h3>Current Keywords:</h3>
                            <asp:UpdatePanel ID="upContent" runat="server">
                                <ContentTemplate>
                                    <div style="width: 285px; padding: 12px; border: 1px ridge #808080;">
                                        <asp:ListBox ID="lbKeywords" runat="server" EnableViewState="true" style="width: 100%;">
                                        </asp:ListBox>
                                        <div style="text-align: right;">
                                            <asp:Button ID="btnRemoveKeyword" runat="server" Text="Remove Keyword" OnClick="btnRemoveKeyword_Click" />
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdateProgress id="updateProgress1" AssociatedUpdatePanelID="upContent" runat="server">
                                <ProgressTemplate>
                                    <div style="position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
                                            <span style="border-width: 0px; position: fixed; padding: 50px; background-color: #FFFFFF; font-size: 36px; left: 40%; top: 40%;">Processing Search Request ...</span>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td colspan="3">

                            <asp:UpdatePanel ID="upNotificationResults" runat="server">
                                <ContentTemplate>
                                    <asp:Repeater ID="rptNotificationResults" runat="server">
                                        <HeaderTemplate>
                                            <h3>Current Results: </h3>
                                            <hr />

                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <fieldset>                                                
                                                <h4 style="margin-bottom: 0px;"><%# DataBinder.Eval(Container.DataItem, "Title") %></h4>
                                                <p style="margin-top: 4px;"><%# DataBinder.Eval(Container.DataItem, "Description") %></p>
                                                <h5 style="margin-top: 4px; color: green; text-align: right;"><a href="http://www.ksl.com/index.php<%# DataBinder.Eval(Container.DataItem, "Link") %>" title="Click to view listing">View Listing</a></h5>
                                                <hr />
                                            </fieldset>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
                
</asp:Content>

