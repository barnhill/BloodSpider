<%@ Page Title="Personal Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PersonalSettings.aspx.cs" Inherits="GlucaTrack.Website.Account.PersonalSettings" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
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
    <asp:SqlDataSource ID="States_US_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetAllStates_US" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="Countries_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetAllCountries" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="Sex_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetAllSex" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="Income_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetAllIncomeRanges" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="Race_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetAllRaces" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="UserType_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetAllUserTypes" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="TimeZones_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetAllTimezones" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="DiabetesTypes_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetAllDiabetesTypes" SelectCommandType="StoredProcedure" />
    <br />
    
     <div class="Settings_Content" id="divProfileImage" runat="server" style="width:900px;">
        <div class="Settings_Header">
            <h4><asp:Label runat="server" ID="lblProfileImage" Text="[Profile Image]"/></h4>                   
        </div>
        
        <asp:UpdatePanel ID="upUpload" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="height:55px; padding-top:10px; padding-left:10px;">
                    <div style="width:55px; float:left;">
                        <img src="/Content/UserImage.ashx" id="imgAvatar" alt="User Image" width="48" height="48"/>
                    </div>
                    <div style="width:200px; float:left;">
                        <ajaxToolkit:AsyncFileUpload ID="AsyncFileUpload2" 
                            runat="server" 
                            OnUploadedComplete="AsyncFileUpload2_UploadedComplete" 
                            OnClientUploadComplete="reloadImage"
                            visible="true"
                            AllowedFileTypes="jpg,jpeg,gif,png"
                            MaximumNumberOfFiles=1
                            CssClass="uploadControl" 
                            ClientIDMode="Static" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>    
    </div>
    <div class="Settings_Content" id="divDiabetes" runat="server" style="width:900px; margin-top:10px;">
        <div class="Settings_Header">
            <h4><asp:Label runat="server" ID="lblDiabetes" Text="[Diabetes Information]"/></h4>                   
        </div>
         <div style="height:220px; padding-top:10px; padding-left:10px;">
            <div style="width:45%; float:left;">
                
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
            </div>
            <div style="width:45%; float:left;">
                <asp:Label ID="lblMorningStart" runat="server">[Morning Start]</asp:Label><br />
                <asp:DropDownList ID="ddMorningStart" runat="server" CssClass="dropdownlist"/><br /><br />
    
                <asp:Label ID="lblAfternoonStart" runat="server">[Afternoon Start]</asp:Label><br />
                <asp:DropDownList ID="ddAfternoonStart" runat="server" CssClass="dropdownlist"/><br /><br />
    
                <asp:Label ID="lblNightStart" runat="server">[Night Start]</asp:Label><br />
                <asp:DropDownList ID="ddNightStart" runat="server" CssClass="dropdownlist"/>
                
            </div>
        </div>
    </div>

     <div class="Graph_Content" id="divPersonal" runat="server" style="width:900px; margin-top:10px;">
        <div class=" Graph_Header">
            <h4><asp:Label runat="server" ID="lblPersonal" Text="[Personal Information]"/></h4>                   
        </div>
        <div style="height:70px; padding-left:10px; padding-top:10px;">
            <div style="width:33%; float:left;">
                <asp:Label ID="lblFirstName" runat="server">[First name]</asp:Label><br />
                <asp:Textbox runat="server" ID="txtFirstName" Text='[First name]' style="font-size:smaller;"/><br /><br />
            </div>
            <div style="width:33%; float:left;">
                <asp:Label ID="lblMiddleName" runat="server">[Middle name]</asp:Label><br />
                <asp:Textbox runat="server" ID="txtMiddleName" Text='[Middle name]' style="font-size:smaller;"/><br /><br />
            </div>
            <div style="width:33%; float:left;">
                <asp:Label ID="lblLastName" runat="server">[Last name]</asp:Label><br />
                <asp:Textbox runat="server" ID="txtLastName" Text='[Last name]' style="font-size:smaller;"/><br /><br />
            </div>
        </div>
        
         <div style="height:70px; padding-left:10px; padding-top:10px;">
            <div style="width:33%; float:left;">
                <asp:UpdatePanel runat="server" ID="upSex" style="width:100%;">
                    <ContentTemplate>
                        <asp:Label ID="lblSex" runat="server" AssociatedControlID="ddSex">[Sex]</asp:Label><br />
                        <asp:DropDownList ID="ddSex" runat="server" DataSourceID="Sex_DataSource" DataTextField = "description" DataValueField = "sex_id" 
                            class="dropdownlist" AutoPostBack="true"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style="width:33%; float:left;">
                <asp:UpdatePanel runat="server" ID="upRace" style="width:100%;">
                    <ContentTemplate>
                        <asp:Label ID="lblRace" runat="server" AssociatedControlID="ddRace">[Race]</asp:Label><br />
                        <asp:DropDownList ID="ddRace" runat="server" DataSourceID="Race_DataSource" DataTextField = "description" DataValueField = "race_id" 
                            class="dropdownlist" AutoPostBack="true"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style="width:33%; float:left;">
                <asp:Label ID="lblBirthDate_Month" runat="server" AssociatedControlID="ddBirthdate_Month">[Birthdate]</asp:Label><br />
                <asp:DropDownList ID="ddBirthdate_Month" runat="server" class="dropdownlist" />
                            &nbsp;

                <asp:TextBox runat="server" ID="txtBirthdate_Day" Width="30" style="font-size:smaller;"/>
                <asp:FilteredTextBoxExtender ID="txtBirthdate_Day_FilteredTextBoxExtender" runat="server" FilterType="Numbers" TargetControlID="txtBirthdate_Day"/>
                            &nbsp;
                <asp:DropDownList ID="ddBirthdate_Year" runat="server" class="dropdownlist"/> 
            </div>
        </div>
        <div style="height:90px; padding-left:10px;">
            <asp:UpdatePanel runat="server" ID="upDiabetesType" style="width:100%;">
                <ContentTemplate>
                    <asp:Label ID="lblDiabetesType" runat="server" AssociatedControlID="ddDiabetesType">[DiabetesType]</asp:Label><br />
                    <asp:DropDownList ID="ddDiabetesType" runat="server" DataSourceID="DiabetesTypes_DataSource" DataTextField = "name" DataValueField = "diabetestypes_id" 
                        class="dropdownlist" AutoPostBack="true"/>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div style="height:90px; padding-left:10px;">
            <asp:Label ID="lblEmail" runat="server">[Email]</asp:Label><br />
            <asp:Textbox runat="server" ID="txtEmail" Text='[Email]' style="font-size:smaller;" Width="350" />
        </div>
        <div style="height:240px; padding-left:10px;">
            <div style="width:40%; float:left;">    
                <asp:Label ID="lblAddress1" runat="server">[Address1]</asp:Label><br />
                <asp:Textbox runat="server" ID="txtAddress1" Text='[Address1]' style="font-size:smaller;" Width="250" /><br /><br />
    
                <asp:Label ID="lblAddress2" runat="server">[Address2]</asp:Label><br />
                <asp:Textbox runat="server" ID="txtAddress2" Text='[Address2]' style="font-size:smaller;" Width="250" /><br /><br />
                
                <asp:Label ID="lblZipcode" runat="server">[Zipcode]</asp:Label><br />
                <asp:Textbox runat="server" ID="txtZipcode" Text='[Zipcode]' style="font-size:smaller;"/>                              
            </div>
            <div style="width:50%; float:left;">
                <asp:Label ID="lblCity" runat="server">[City]</asp:Label><br />
                <asp:Textbox runat="server" ID="txtCity" Text='[City]' style="font-size:smaller;" Width="250" /><br /><br />
                <asp:Label ID="lblState" runat="server" AssociatedControlID="ddState">[State]</asp:Label><br />
                <asp:UpdatePanel runat="server" ID="upState" style="width:100%;">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddState" runat="server" DataSourceID="States_US_DataSource" DataTextField = "name" DataValueField = "state_id" 
                            class="dropdownlist" AutoPostBack="true" Width="250" />
                        <asp:Textbox runat="server" ID="txtOtherState" Text='[Other state]' style="font-size:smaller;" Width="175"/>
                    </ContentTemplate>
                </asp:UpdatePanel><br /><br />

                <asp:UpdatePanel runat="server" ID="upCountry" style="width:100%;">
                    <ContentTemplate>
                        <asp:Label ID="lblCountry" runat="server" AssociatedControlID="ddCountry">[Country]</asp:Label><br />
                        <asp:DropDownList ID="ddCountry" runat="server" DataSourceID="Countries_DataSource" DataTextField = "name" DataValueField = "country_id" 
                            class="dropdownlist" AutoPostBack="true" OnSelectedIndexChanged="Country_SelectedIndexChanged"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>            
        </div>
        <div style="height:70px; padding-left:10px;">
            <asp:UpdatePanel runat="server" ID="upTimezone" style="width:100%;">
                <ContentTemplate>
                    <asp:Label ID="lblTimezone" runat="server" AssociatedControlID="ddTimezone">[Timezone]</asp:Label><br />
                    <asp:DropDownList ID="ddTimezone" runat="server" DataSourceID="TimeZones_DataSource" DataTextField = "location" DataValueField = "timezone_id" 
                        class="dropdownlist" AutoPostBack="true"/>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
         <div style="height:70px; padding-left:10px;">
            <asp:UpdatePanel runat="server" ID="upIncome" style="width:100%;">
                <ContentTemplate>
                    <asp:Label ID="lblIncome" runat="server" AssociatedControlID="ddIncome">[Income]</asp:Label><br />
                    <asp:DropDownList ID="ddIncome" runat="server" DataSourceID="Income_DataSource" DataTextField = "income" DataValueField = "income_id" 
                        class="dropdownlist" AutoPostBack="true"/>
                </ContentTemplate>
            </asp:UpdatePanel>
         </div>
    </div>

    <div class="Settings_Content" id="divAccount" runat="server" style="width:900px; margin-top:10px;">
        <div class="Settings_Header">
            <h4><asp:Label runat="server" ID="lblAccount" Text="[Account Information]"/></h4>                   
        </div>
        <div style="height:60px; padding-left:10px; padding-top:10px;">
            <div style="width:33%; float:left;">
                <asp:Label ID="lblLastSyncLabel" runat="server">[Last Sync]</asp:Label><br />
                <asp:Label ID="lblLastSyncValue" runat="server">[Last Sync]</asp:Label><br /><br />
            </div>
            <div style="width:33%; float:left;">
                <asp:Label ID="lblLastWebLoginLabel" runat="server">[Last Weblogin]</asp:Label><br />
                <asp:Label ID="lblLastWebLoginValue" runat="server">[Last Weblogin]</asp:Label><br /><br />
            </div>
            <div style="width:33%; float:left;">
                <asp:UpdatePanel runat="server" ID="upUserType" style="width:100%;">
                    <ContentTemplate>
                        <asp:Label ID="lblUserType" runat="server" AssociatedControlID="ddUserType">[UserType]</asp:Label><br />
                        <asp:DropDownList ID="ddUserType" runat="server" DataSourceID="UserType_DataSource" DataTextField = "description" DataValueField = "usertype_id" 
                            class="dropdownlist" AutoPostBack="true" Enabled="false"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <div style="padding-top:10px; padding-bottom:10px; text-align:right;">
        <asp:Button ID="btnSavePersonalSettings" runat="server" CommandName="Save" Text="Save Settings" OnClick="btnSavePersonalSettings_Click" class="LogButton" />         
    </div>
</asp:Content>
