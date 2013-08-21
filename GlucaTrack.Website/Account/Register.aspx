<%@ Page Title="Registration Step 1" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="GlucaTrack.Website.Account.Register" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title" runat="server">
        <h1 runat="server"><%: Title %>.</h1>
        </hgroup>
                    <p class="message-info">
                        <asp:Label runat="server" ID="lblMessageInfo">[Info]</asp:Label>
                    </p>
                    
                        <div><asp:RequiredFieldValidator ID="RequiredFieldUsername" runat="server" ControlToValidate="UserName"
                                    CssClass="field-validation-error" ErrorMessage="UserName is required." Display="None"/>
                        </div>
                        <div><asp:RequiredFieldValidator ID="RequiredFieldEmail" runat="server" ControlToValidate="Email"
                                    CssClass="field-validation-error" ErrorMessage="Email address field is required." Display="None"/></div>
                        <div><asp:RequiredFieldValidator ID="RequiredFieldPassword" runat="server" 
                                    ControlToValidate="Password"
                                    CssClass="field-validation-error" Display="None"
                                    ErrorMessage="The password field is required." /></div>
                        <div><asp:RequiredFieldValidator ID="RequiredFieldConfirmPassword" runat="server" ControlToValidate="ConfirmPassword"
                                    CssClass="field-validation-error" 
                                    Display="None" 
                                    ErrorMessage="The confirm password field is required."/></div>
                        <div><asp:RegularExpressionValidator ID="RegularExpressionEmailFormat" runat="server"
                                    CssClass="field-validation-error" 
                                    Display="None"
                                    ControlToValidate="Email" 
                                    ErrorMessage="Invalid email address format."
                                    ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$" /></div>
                        <div><asp:RegularExpressionValidator ID="RegularExpressionPasswordLength" runat="server"
                                    CssClass="field-validation-error" 
                                    Display="None"
                                    ControlToValidate="Password" 
                                    ErrorMessage="[Password length invalid]"
                                    ValidationExpression="[^\s]{6,50}" /> </div>
                        <div><asp:CompareValidator ID="ComparePassword" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                                    CssClass="field-validation-error" 
                                    Display="None" 
                                    ErrorMessage="[Compare passwords do not match]" /></div>                
                    
                    <asp:ValidatorCalloutExtender ID="RequiredFieldUsername_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldUsername" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldEmail_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldEmail" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldPassword_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldPassword" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldConfirmPassword_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldConfirmPassword" />
                    <asp:ValidatorCalloutExtender ID="EmailFormat_ValidatorCalloutExtender" runat="server" TargetControlID="RegularExpressionEmailFormat" />
                    <asp:ValidatorCalloutExtender ID="PasswordLength_ValidatorCalloutExtender" runat="server" TargetControlID="RegularExpressionPasswordLength" />
                    <asp:ValidatorCalloutExtender ID="ComparePassword_ValidatorCalloutExtender" runat="server" TargetControlID="ComparePassword" />

                    <p class="validation-summary-errors">
                        <asp:Literal runat="server" ID="ErrorMessage" />
                    </p>

                        <asp:UpdatePanel runat="server" ID="up1" style="width:100%;">
                            <ContentTemplate>
                                <asp:Label ID="lblUsername" runat="server" AssociatedControlID="UserName">User name</asp:Label><br />
                                <asp:TextBox runat="server" ID="UserName" AutoPostBack="true" OnTextChanged="UserName_TextChanged"/>
                                <div runat="server" id="UserAvailability" class="Availibility-Username"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <asp:Label ID="lblEmail" runat="server" AssociatedControlID="Email">Email address</asp:Label><br />
                        <asp:TextBox runat="server" ID="Email" /><br />

                        <asp:Label ID="lblPassword" runat="server" AssociatedControlID="Password">Password</asp:Label><br />
                        <asp:TextBox runat="server" ID="Password" TextMode="Password"/>
                        <br />
                        <asp:PasswordStrength ID="Password_PasswordStrength" runat="server" TargetControlID="Password" PreferredPasswordLength="6" 
                            TextStrengthDescriptions="Very Poor;Medium;Strong;" 
                            StrengthStyles="barIndicator_poor; barIndicator_good; barIndicator_strong;"
                            StrengthIndicatorType="Text"/>

                        <asp:Label ID="lblConfirmPassword" runat="server" AssociatedControlID="ConfirmPassword">Confirm password</asp:Label><br />
                        <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password"/>                     
                        <br />
                        <asp:Button ID="btnContinue" runat="server" OnClick="btnContinue_Click" Text="Continue" class="LogButton"/>
</asp:Content>