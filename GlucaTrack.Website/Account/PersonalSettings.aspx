<%@ Page Title="GlucaTrack" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PersonalSettings.aspx.cs" Inherits="GlucaTrack.Website.Account.PersonalSettings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="PersonalSettings_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetPersonalSettings" SelectCommandType="StoredProcedure" >
        <SelectParameters>
            <asp:SessionParameter Name="login" SessionField="LoggedInUserId" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:Label ID="lblLowNormal" runat="server" AssociatedControlID="fvLowNormal">Low normal</asp:Label><br />
    <asp:FormView ID="fvLowNormal" runat="server" DataKeyNames="user_id" DataSourceID="PersonalSettings_DataSource">
        <ItemTemplate>
            <asp:Textbox runat="server" ID="LowNormal" Text='<%# Eval("lownormal") %>' style="font-size:smaller;" />
            <asp:RegularExpressionValidator ValidationGroup="reg" ID="RegularExpressionValidator_LowNormal" ControlToValidate="LowNormal" Display="None"
                runat="server" ErrorMessage="Invalid" ValidationExpression="^[0-9]*$">*</asp:RegularExpressionValidator>
        </ItemTemplate>
    </asp:FormView> 
    <br />
    <asp:Label ID="lblHighNormal" runat="server" AssociatedControlID="fvHighNormal">High normal</asp:Label><br />
    <asp:FormView ID="fvHighNormal" runat="server" DataKeyNames="user_id" DataSourceID="PersonalSettings_DataSource">
        <ItemTemplate>
            <asp:Textbox runat="server" ID="HighNormal" Text='<%# Eval("highnormal") %>' style="font-size:smaller;"/>
            <asp:RegularExpressionValidator ValidationGroup="reg" ID="RegularExpressionValidator_HighNormal" ControlToValidate="HighNormal" Display="None"
                runat="server" ErrorMessage="Invalid" ValidationExpression="^[0-9]*$">*</asp:RegularExpressionValidator>
        </ItemTemplate>
    </asp:FormView> 
    <br /> <br />
    <asp:Button ID="btnSavePersonalSettings" runat="server" CommandName="Save" Text="Save Settings" OnClick="btnSavePersonalSettings_Click" class="LogButton" />         
</asp:Content>
