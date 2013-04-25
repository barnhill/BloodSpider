<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="GlucaTrack.Website.Contact" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>Tell us your thoughts, concerns or problems.</h2>
    </hgroup>

    <section class="contact">
        <header>
            <h3>Email:</h3>
        </header>
        <p>
            <span class="label">Support:</span>
            <span><a href="mailto:bradbarnhill@hotmail.com">bradbarnhill@hotmail.com</a></span>
        </p>
        <p>
            <span class="label">Marketing:</span>
            <span><a href="mailto:bradbarnhill@hotmail.com">bradbarnhill@hotmail.com</a></span>
        </p>
        <p>
            <span class="label">General:</span>
            <span><a href="mailto:bradbarnhill@hotmail.com">bradbarnhill@hotmail.com</a></span>
        </p>
    </section>

    <section class="contact">
        <header>
            <h3>Address:</h3>
        </header>
        <p>
            10716 College Ave.<br />
            Kansas City, MO 64137
        </p>
    </section>
</asp:Content>