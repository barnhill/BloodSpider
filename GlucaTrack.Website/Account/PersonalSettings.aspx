<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PersonalSettings.aspx.cs" Inherits="GlucaTrack.Website.Account.PersonalSettings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="PersonalSettings_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetPersonalSettings" SelectCommandType="StoredProcedure" >
        <SelectParameters>
            <asp:SessionParameter Name="userid" SessionField="LoggedInUserId" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:Label ID="lblLowNormal" runat="server" AssociatedControlID="LowNormal">Low normal</asp:Label><br />
    <asp:Textbox runat="server" ID="LowNormal" /><br />
    <asp:Label ID="lblHighNormal" runat="server" AssociatedControlID="HighNormal">High normal</asp:Label><br />
    <asp:Textbox runat="server" ID="HighNormal" /><br />
</asp:Content>
