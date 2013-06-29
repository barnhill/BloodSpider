<%@ Page Title="GlucaTrack" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PersonalSettings.aspx.cs" Inherits="GlucaTrack.Website.Account.PersonalSettings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="PersonalSettings_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetUserSettings" SelectCommandType="StoredProcedure" >
        <SelectParameters>
            <asp:SessionParameter Name="user_id" SessionField="LoggedInUserId" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    <asp:UpdatePanel ID="upUpload" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <img src="/Content/UserImage.ashx" id="imgAvatar" alt="User Image" width="48" height="48"/>
            <ajaxToolkit:AsyncFileUpload ID="AsyncFileUpload2" 
                runat="server" 
                OnUploadedComplete="AsyncFileUpload2_UploadedComplete" 
                OnClientUploadComplete="reloadImage"
                visible="true"
                AllowedFileTypes="jpg,jpeg,gif,png"
                MaximumNumberOfFiles=1
                CssClass="uploadControl" 
                ClientIDMode="Static" />
            </ContentTemplate>
        </asp:UpdatePanel>
    <br />
    <asp:Label ID="lblLowNormal" runat="server" AssociatedControlID="fvLowNormal">[Low normal]</asp:Label><br />
    <asp:FormView ID="fvLowNormal" runat="server" DataKeyNames="user_id" DataSourceID="PersonalSettings_DataSource">
        <ItemTemplate>
            <asp:Textbox runat="server" ID="LowNormal" Text='<%# Eval("lownormal") %>' style="font-size:smaller;" />
            <asp:RegularExpressionValidator ValidationGroup="reg" ID="RegularExpressionValidator_LowNormal" ControlToValidate="LowNormal" Display="None"
                runat="server" ErrorMessage="Invalid" ValidationExpression="^[0-9]*$">*</asp:RegularExpressionValidator>
        </ItemTemplate>
    </asp:FormView> 
    <br />
    <asp:Label ID="lblHighNormal" runat="server" AssociatedControlID="fvHighNormal">[High normal]</asp:Label><br />
    <asp:FormView ID="fvHighNormal" runat="server" DataKeyNames="user_id" DataSourceID="PersonalSettings_DataSource">
        <ItemTemplate>
            <asp:Textbox runat="server" ID="HighNormal" Text='<%# Eval("highnormal") %>' style="font-size:smaller;"/>
            <asp:RegularExpressionValidator ValidationGroup="reg" ID="RegularExpressionValidator_HighNormal" ControlToValidate="HighNormal" Display="None"
                runat="server" ErrorMessage="Invalid" ValidationExpression="^[0-9]*$">*</asp:RegularExpressionValidator>
        </ItemTemplate>
    </asp:FormView> 
    <br />
    
    <asp:Label ID="lblMorningStart" runat="server">[Morning Start]</asp:Label><br />
    <asp:DropDownList ID="ddMorningStart" runat="server" CssClass="dropdownlist"/><br /><br />
    
    <asp:Label ID="lblAfternoonStart" runat="server">[Afternoon Start]</asp:Label><br />
    <asp:DropDownList ID="ddAfternoonStart" runat="server" CssClass="dropdownlist"/><br /><br />
    
    <asp:Label ID="lblNightStart" runat="server">[Night Start]</asp:Label><br />
    <asp:DropDownList ID="ddNightStart" runat="server" CssClass="dropdownlist"/><br /><br />
    
    <asp:Label ID="lblFirstName" runat="server">[First name]</asp:Label><br />
    <asp:Textbox runat="server" ID="txtFirstName" Text='[First name]' style="font-size:smaller;"/><br /><br />
    <asp:Label ID="lblMiddleName" runat="server">[Middle name]</asp:Label><br />
    <asp:Textbox runat="server" ID="txtMiddleName" Text='[Middle name]' style="font-size:smaller;"/><br /><br />
    <asp:Label ID="lblLastName" runat="server">[Last name]</asp:Label><br />
    <asp:Textbox runat="server" ID="txtLastName" Text='[Last name]' style="font-size:smaller;"/><br /><br />
    <asp:Label ID="lblAddress1" runat="server">[Address1]</asp:Label><br />
    <asp:Textbox runat="server" ID="txtAddress1" Text='[Address1]' style="font-size:smaller;"/><br /><br />
    <asp:Label ID="lblAddress2" runat="server">[Address2]</asp:Label><br />
    <asp:Textbox runat="server" ID="txtAddress2" Text='[Address2]' style="font-size:smaller;"/><br /><br />

    <asp:Button ID="btnSavePersonalSettings" runat="server" CommandName="Save" Text="Save Settings" OnClick="btnSavePersonalSettings_Click" class="LogButton" />         
</asp:Content>
