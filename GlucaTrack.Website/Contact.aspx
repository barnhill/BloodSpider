<%@ Page Title="Contact Us" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="GlucaTrack.Website.Contact" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title" style="padding-top: 10px;">
        <h1><%: Title %>.</h1><br />
        <h4>We look forward to your comments.  Please feel free to send us your thoughts, concerns, or challenges.</h4>
    </hgroup>
    <br />
    <section class="contact">
        <asp:Label ID="lblEmail" runat="server" Text="[Email:]"></asp:Label><br />
        <asp:TextBox ID="txtEmailAddress" runat="server" Width="350px"/><br />
        <br />
        <asp:Label ID="lblMessage" runat="server" Text="[Message:]"></asp:Label><br />
        <asp:TextBox ID="txtMessage" runat="server" Height="200px" Width="500px" TextMode="MultiLine" Rows="10"/><br /><br />
        <asp:Button ID="btnSendEmail" runat="server" onclick="btnSendEmail_Click" Text="[Send Email]" class="LogButton"/>
        <asp:Label ID="lblMessageStatus" runat="server" Text="[Message Status]" Visible="False"></asp:Label><br /><br />
    </section>
</asp:Content>