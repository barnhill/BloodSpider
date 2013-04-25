<%@ Page Title="GlucaTrack Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GlucaTrack.Website.Account.Login" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <br /><br />
        <h1><%: Title %></h1>
    </hgroup>
    <link href="/Styles/login.css" rel="stylesheet" /> 
    <br />
    <section id="loginForm">
        <asp:Login runat="server" ID="LoginContainer" ViewStateMode="Disabled" RenderOuterTable="false" >
            <LayoutTemplate>
                <p class="validation-summary-errors">
                    <asp:Literal runat="server" ID="FailureText" />
                </p>
                    <div>
                        <asp:Label runat="server" ID="lblUsername" AssociatedControlID="UserName">User name</asp:Label><br />
                        <asp:TextBox runat="server" ID="UserName" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredField_Username" ControlToValidate="UserName" CssClass="field-validation-error" ErrorMessage="The user name field is required." Display="None" />
                    </div>
                    <div>
                        <asp:Label runat="server" ID="lblPassword" AssociatedControlID="Password">Password</asp:Label><br />
                        <asp:TextBox runat="server" ID="Password" TextMode="Password" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredField_Password" ControlToValidate="Password" CssClass="field-validation-error" ErrorMessage="The password field is required." Display="None" />
                    </div>
                    <div>
                        <asp:CheckBox runat="server" ID="RememberMe" Visible="False" />
                        <asp:Label runat="server" ID="lblRememberMe" AssociatedControlID="RememberMe" CssClass="checkbox" Visible="False">Remember me?</asp:Label>
                    </div>

                    <asp:ValidatorCalloutExtender ID="RequiredFieldUsername_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredField_Username" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldPassword_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredField_Password" />

                    <asp:Button runat="server" ID="btnLogin" CommandName="Login" Text="Log in" Font-Bold="True" class="LogButton" OnClick="btnLogin_Click" />
            </LayoutTemplate>
        </asp:Login>
        <p>
            <asp:Label runat="server" ID="lblDontHave" Font-Size="Small" />
            <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled" Font-Size="Small">Register</asp:HyperLink>
        </p>
        </section>
    <br /><br /><br /><br />
    </asp:Content>
