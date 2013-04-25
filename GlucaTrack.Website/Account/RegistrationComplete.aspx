<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegistrationComplete.aspx.cs" Inherits="GlucaTrack.Website.Account.RegistrationComplete" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Timer ID="TimerRedirect" runat="server" Interval="3000" OnTick="TimerRedirect_Tick">
    </asp:Timer>
    <br />
    <br />
    <br />
    <br />
    <asp:Label ID="lblWelcome" runat="server" CssClass="subheader">[Welcome message]</asp:Label>
</asp:Content>
