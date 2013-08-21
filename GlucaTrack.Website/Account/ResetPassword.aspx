<%@ Page Title="Reset Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="GlucaTrack.Website.Account.ResetPassword" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
        <br /><br />
        <h1><%: Title %></h1>
    </hgroup>
    <link href="/Styles/login.css" rel="stylesheet" />
    <br />
    <asp:UpdatePanel runat="server" ID="upBeforeSend" style="width:100%;">
        <ContentTemplate>
            <p class="validation-summary-errors">
                <asp:Literal runat="server" ID="ErrorMessage"/>
            </p>
            <div><asp:RegularExpressionValidator 
                ID="RegularExpressionEmailFormat" 
                runat="server"
                CssClass="field-validation-error" 
                Display="None"
                ControlToValidate="txtEmail" 
                ErrorMessage="Invalid email address format."
                ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$" />
                <asp:ValidatorCalloutExtender ID="EmailFormat_ValidatorCalloutExtender" runat="server" TargetControlID="RegularExpressionEmailFormat" />
            </div>
            <div>
                <asp:Label runat="server" ID="lblEmail" AssociatedControlID="txtEmail">[Email]</asp:Label><br />
                <asp:TextBox runat="server" ID="txtEmail" Width="375px"/><br />
            </div>
            <asp:Button runat="server" ID="btnSendEmail" CommandName="Login" Text="[Send Email]" Font-Bold="True" class="LogButton" OnClick="btnLogin_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel runat="server" ID="upAfterSend" style="width:100%;">
        <ContentTemplate>
            <asp:Label runat="server" ID="lblEmailSent">[Email Sent]</asp:Label>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
