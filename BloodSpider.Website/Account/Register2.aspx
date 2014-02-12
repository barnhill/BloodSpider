<%@ Page Title="Registration Step 2" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register2.aspx.cs" Inherits="BloodSpider.Website.Account.Register2" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup id="Hgroup1" class="title" runat="server">
        <h1 id="H1" runat="server"><%: Title %>.</h1>
    </hgroup>
    <!-- DataSources -->
    <asp:SqlDataSource ID="Countries_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:BloodSpiderConnectionString %>" SelectCommand="sp_GetAllCountries" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="States_US_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:BloodSpiderConnectionString %>" SelectCommand="sp_GetAllStates_US" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="Sex_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:BloodSpiderConnectionString %>" SelectCommand="sp_GetAllSex" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="Income_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:BloodSpiderConnectionString %>" SelectCommand="sp_GetAllIncomeRanges" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="Race_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:BloodSpiderConnectionString %>" SelectCommand="sp_GetAllRaces" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="Timezone_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:BloodSpiderConnectionString %>" SelectCommand="sp_GetAllTimezones" SelectCommandType="StoredProcedure" />
    <asp:SqlDataSource ID="DiabetesType_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:BloodSpiderConnectionString %>" SelectCommand="sp_GetAllDiabetesTypes" SelectCommandType="StoredProcedure" />
    
    <p class="message-info">
        <asp:Label ID="lblMessageInfo" runat="server">This information will help us trend data across the world geographically.</asp:Label>
    </p>

                    <!-- Validators -->
                    <div><asp:RequiredFieldValidator ID="RequiredFieldFirstName" runat="server" ControlToValidate="FirstName"
                        CssClass="field-validation-error" ErrorMessage="The first name field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldMiddleName" runat="server" ControlToValidate="MiddleName"
                        CssClass="field-validation-error" ErrorMessage="The middle name field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldLastName" runat="server" ControlToValidate="LastName"
                        CssClass="field-validation-error" ErrorMessage="The last name field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldAddress1" runat="server" ControlToValidate="Address1"
                        CssClass="field-validation-error" ErrorMessage="The address field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldCity" runat="server" ControlToValidate="City"
                        CssClass="field-validation-error" ErrorMessage="The City field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldState" runat="server" ControlToValidate="State"
                        CssClass="field-validation-error" ErrorMessage="The State field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldOtherState" runat="server" ControlToValidate="OtherState"
                        CssClass="field-validation-error" ErrorMessage="The Other State field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldZipcode" runat="server" ControlToValidate="Zipcode"
                        CssClass="field-validation-error" ErrorMessage="The Zipcode field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldCountry" runat="server" ControlToValidate="Country"
                        CssClass="field-validation-error" ErrorMessage="The Country field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldBirthdate_Day" runat="server" ControlToValidate="txtBirthdate_Day"
                        CssClass="field-validation-error" ErrorMessage="The Birthdate field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldIncome" runat="server" ControlToValidate="Income"
                        CssClass="field-validation-error" ErrorMessage="The Income Range field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldSex" runat="server" ControlToValidate="Sex"
                        CssClass="field-validation-error" ErrorMessage="The Sex field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldRace" runat="server" ControlToValidate="Race"
                        CssClass="field-validation-error" ErrorMessage="The Race field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldTimezone" runat="server" ControlToValidate="Timezone"
                        CssClass="field-validation-error" ErrorMessage="The Timezone field is required." Display="None"/></div>
                    <div><asp:RequiredFieldValidator ID="RequiredFieldDiabetesType" runat="server" ControlToValidate="DiabetesType"
                        CssClass="field-validation-error" ErrorMessage="The DiabetesType field is required." Display="None"/></div>

                    <asp:ValidatorCalloutExtender ID="RequiredFieldFirstName_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldFirstName" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldMiddleName_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldMiddleName" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldLastName_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldLastName" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldAddress1_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldAddress1" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldCity_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldCity" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldState_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldState" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldOtherState_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldOtherState" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldZipcode_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldZipcode" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldCountry_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldCountry" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldBirthdate_Day_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldBirthdate_Day" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldIncome_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldIncome" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldSex_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldSex" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldRace_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldRace" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldTimezone_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldTimezone" />
                    <asp:ValidatorCalloutExtender ID="RequiredFieldDiabetesType_ValidatorCalloutExtender" runat="server" TargetControlID="RequiredFieldDiabetesType" />

                    <p class="validation-summary-errors">
                        <asp:Literal runat="server" ID="ErrorMessage" />
                    </p>
    
                    <p>
                        <asp:Label ID="lblFirstname" runat="server" AssociatedControlID="FirstName">First name</asp:Label>
                        <asp:TextBox runat="server" ID="FirstName"/>
                    </p>

                    <p>
                        <asp:Label ID="lblMiddlename" runat="server" AssociatedControlID="MiddleName">Middle name</asp:Label>
                        <asp:TextBox runat="server" ID="MiddleName"/>
                    </p>
                    
                    <p>
                        <asp:Label ID="lblLastname" runat="server" AssociatedControlID="LastName">Last name</asp:Label>
                        <asp:TextBox runat="server" ID="LastName"/>
                    </p>

                    <p>
                        <asp:Label ID="lblAddress1" runat="server" AssociatedControlID="Address1">Address line 1</asp:Label>
                        <asp:TextBox runat="server" ID="Address1"/>
                    </p>

                    <p>
                        <asp:Label ID="lblAddress2" runat="server" AssociatedControlID="Address2">Address line 2</asp:Label>
                        <asp:TextBox runat="server" ID="Address2"/>
                    </p>

                    <p>
                        <asp:Label ID="lblCity" runat="server" AssociatedControlID="City">City</asp:Label>
                        <asp:TextBox runat="server" ID="City"/>
                    </p>

                    <p>        
                    <asp:Label ID="lblState" runat="server" AssociatedControlID="State">State</asp:Label>
                    <asp:UpdatePanel runat="server" ID="upState" style="width:100%;">
                        <ContentTemplate>
                            <asp:DropDownList ID="State" runat="server" CssClass="dropdownlist" 
                                DataSourceID="States_US_DataSource" DataTextField = "name" DataValueField = "state_id"/>
                            <asp:TextBox runat="server" ID="OtherState" Width="175"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>   
                    </p>

                    <p>
                    <asp:Label ID="lblZipcode" runat="server" AssociatedControlID="Zipcode">Zipcode</asp:Label>
                    <asp:TextBox runat="server" ID="Zipcode" Width="75"/>
                    </p>

                    <p>        
                    <asp:UpdatePanel runat="server" ID="upCountry" style="width:100%;">
                        <ContentTemplate>
                            <asp:Label ID="lblCountry" runat="server" AssociatedControlID="Country">Country</asp:Label>
                            <asp:DropDownList ID="Country" runat="server" DataSourceID="Countries_DataSource" DataTextField = "name" DataValueField = "country_id" 
                                class="dropdownlist" OnSelectedIndexChanged="Country_SelectedIndexChanged" AutoPostBack="true"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </p>

                    <p>
                    <asp:UpdatePanel runat="server" ID="upTimezone" style="width:100%;">
                        <ContentTemplate>
                            <asp:Label ID="lblTimezone" runat="server" AssociatedControlID="Timezone">Timezone</asp:Label>
                            <asp:DropDownList ID="Timezone" runat="server" DataSourceID="Timezone_DataSource" DataTextField = "location" DataValueField = "timezone_id" 
                                class="dropdownlist" AutoPostBack="true"/>
                        </ContentTemplate>
                   </asp:UpdatePanel>
                   </p>

                   <p>
                   <asp:Label ID="lblBirthDate_Month" runat="server" AssociatedControlID="ddBirthdate_Month">Birthdate</asp:Label>
                   <asp:DropDownList ID="ddBirthdate_Month" runat="server" class="dropdownlist" />
                                &nbsp;

                   <asp:TextBox runat="server" ID="txtBirthdate_Day" Width="30" style="font-size:smaller;"/>
                   <asp:FilteredTextBoxExtender ID="txtBirthdate_Day_FilteredTextBoxExtender" runat="server" FilterType="Numbers" TargetControlID="txtBirthdate_Day"/>
                                &nbsp;
                   <asp:DropDownList ID="ddBirthdate_Year" runat="server" class="dropdownlist"/>

                    <asp:UpdatePanel runat="server" ID="upSex" style="width:100%;">
                        <ContentTemplate>
                            <asp:Label ID="lblSex" runat="server" AssociatedControlID="Sex">Sex/Gender</asp:Label>
                            <asp:DropDownList ID="Sex" runat="server" DataSourceID="Sex_DataSource" DataTextField = "Description" DataValueField = "sex_id" 
                                    class="dropdownlist" AutoPostBack="true"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </p>

                    <p>
                    <asp:UpdatePanel runat="server" ID="upIncome" style="width:100%;">
                        <ContentTemplate>
                            <asp:Label ID="lblIncome" runat="server" AssociatedControlID="Income">Income Range</asp:Label>
                            <asp:DropDownList ID="Income" runat="server" DataSourceID="Income_DataSource" DataTextField = "income" DataValueField = "income_id" 
                                    class="dropdownlist" AutoPostBack="true"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </p>

                    <p>
                    <asp:UpdatePanel runat="server" ID="upRace" style="width:100%;">
                        <ContentTemplate>
                            <asp:Label ID="lblRace" runat="server" AssociatedControlID="Race">Race</asp:Label>
                            <asp:DropDownList ID="Race" runat="server" DataSourceID="Race_DataSource" DataTextField = "description" DataValueField = "race_id" 
                                    class="dropdownlist" AutoPostBack="true"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </p>

                    <p>
                    <asp:UpdatePanel runat="server" ID="upDiabetesType" style="width:100%;">
                        <ContentTemplate>
                            <asp:Label ID="lblDiabetesType" runat="server" AssociatedControlID="DiabetesType">Type of Diabetes</asp:Label>
                            <asp:DropDownList ID="DiabetesType" runat="server" DataSourceID="DiabetesType_DataSource" DataTextField = "name" DataValueField = "diabetestypes_id" 
                                    CssClass="dropdownlist" AutoPostBack="true"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    
                    <asp:Button ID="btnFinishRegistration" runat="server" CommandName="MoveNext" Text="Finish Registration" OnClick="btnFinishRegistration_Click" class="LogButton" />
</asp:Content>
