<%@ Page Title="Free GlucaTrack Download" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Download.aspx.cs" Inherits="GlucaTrack.Website.Download" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding-top:20px; padding-bottom:20px;">
        <hgroup id="Hgroup1" class="title" runat="server">
            <h3 id="H3" runat="server"><%: Title %></h3>
            </hgroup>
        <br />
        <div style="text-align: center; width: 250px;">
            <a href="/download/GlucaTrack_Windows_Setup.exe"><img src="Images/download/download.png" /></a><br />
            <asp:Label ID="lblWinVersion" runat="server" Text="[Windows Version]"/><br /><br />
        </div>
        <br /><br />        
        <a href="About.aspx">What is GlucaTrack?</a><br />
        <ul style="padding-left:20px; font-size:large;">
            <li>Keep your records electronically in one place</li>
            <li>Your data is always protected</li>
            <li>Empower yourself</li>
        </ul>
    </div>
</asp:Content>
