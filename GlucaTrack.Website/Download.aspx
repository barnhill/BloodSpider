<%@ Page Title="Free GlucaTrack Download" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Download.aspx.cs" Inherits="GlucaTrack.Website.Download" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding-top:20px; height:320px;">   
        <div style="float:left; width:500px;">
            <hgroup id="Hgroup1" class="title" runat="server">
                <h3 id="H1" runat="server"><%: Title %>.</h3>
            </hgroup>
            <br /><br />
            <div style="text-align: center; width: 250px;">
                <a href="/download/GlucaTrack_Windows_Setup.exe"><img src="Images/download/download.png" /></a><br />
                <asp:Label ID="lblWinVersion" runat="server" Text="[Windows Version]"/><br /><br />
            </div><br />       
            <a href="About.aspx">What is GlucaTrack?</a><br />
            <ul style="padding-left:20px; font-size:large;">
                <li>Keep your records electronically in one place</li>
                <li>Your data is always protected</li>
                <li>Empower yourself</li>
            </ul>
        </div>
        <div style="float:right; width:300px;">
            <img src="Images/download/downloads.png" />
        </div>
    </div>
</asp:Content>
