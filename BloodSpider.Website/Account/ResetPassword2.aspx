<%@ Page Title="Reset Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ResetPassword2.aspx.cs" Inherits="BloodSpider.Website.Account.ResetPassword2" %>
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
            <p class="message-info">
                <asp:Label runat="server" ID="lblMessageInfo">[Info]</asp:Label>
            </p>
            <br />
            <p class="validation-summary-errors">
                <asp:Literal runat="server" ID="ErrorMessage"/>
            </p>
            <div>
            <asp:RegularExpressionValidator ID="RegularExpressionPasswordLength" runat="server"
                                        CssClass="field-validation-error" 
                                        Display="None"
                                        ControlToValidate="txtPassword" 
                                        ErrorMessage="[Password length invalid]"
                                        ValidationExpression="[^\s]{6,50}" />
            <div><asp:CompareValidator ID="ComparePassword" runat="server" ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword"
                                        CssClass="field-validation-error" 
                                        Display="None" 
                                        ErrorMessage="[Compare passwords do not match]" /></div>
            </div>
    
            <asp:ValidatorCalloutExtender ID="PasswordLength_ValidatorCalloutExtender" runat="server" TargetControlID="RegularExpressionPasswordLength" />
            <asp:ValidatorCalloutExtender ID="ComparePassword_ValidatorCalloutExtender" runat="server" TargetControlID="ComparePassword" />
            <div>
                <asp:Label runat="server" ID="lblPassword" AssociatedControlID="txtPassword">[Password]</asp:Label><br />
                <asp:TextBox runat="server" ID="txtPassword" Width="300px" TextMode="Password"/><br />
                <asp:Label ID="lblConfirmPassword" runat="server" AssociatedControlID="txtConfirmPassword">[Confirm password]</asp:Label><br />
                <asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password"/><br />
            </div>
            <asp:Button runat="server" ID="btnChangePassword" CommandName="Login" Text="[Change Password]" Font-Bold="True" class="LogButton" OnClick="btnChangePassword_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel runat="server" ID="upAfterSend" style="width:100%;">
        <ContentTemplate>
            <asp:Label runat="server" ID="lblPasswordChanged">[Password Changed]</asp:Label><br /><br />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
