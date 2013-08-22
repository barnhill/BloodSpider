<%@ Page Title="Free GlucaTrack Download" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Download.aspx.cs" Inherits="GlucaTrack.Website.Download" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup id="Hgroup1" class="title" runat="server">
        <h1 id="H1" runat="server"><%: Title %>.</h1>
        </hgroup>
    <br /><br />
    <div style="text-align: center; width: 250px;">
        <a href="/download/GlucaTrack_Windows_Setup.exe"><img src="Images/download/download.png" /></a><br />
        <asp:Label ID="lblWinVersion" runat="server" Text="[Windows Version]"/><br /><br />
    </div>
    <br /><br />
    <a href="About.aspx">What is GlucaTrack?</a><br />
    
    - Keep your records electronically in one place<br />
    - Your data is always protected<br />
    - Empower yourself<br />
</asp:Content>
