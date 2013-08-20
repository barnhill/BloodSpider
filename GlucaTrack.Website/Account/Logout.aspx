<%@ Page Title="Logout" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="GlucaTrack.Website.Logout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Timer ID="TimerRedirect" runat="server" Interval="2500" OnTick="TimerRedirect_Tick">
    </asp:Timer>
    <br />
    <br />
    <br />
    <br />
    <asp:Label ID="lblGoodbye" runat="server" CssClass="subheader">[Goodbye message]</asp:Label>
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
</asp:Content>
