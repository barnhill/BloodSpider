<%@ Page Title="GlucaTrack" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GlucaTrack.Website._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div>
        <a href="SupportedMeters.aspx" id="SupportedMeters">Supported Meters</a>
    </div>
    <div class="Front_Page_Main_Container">
        <div class="Front_Page_Container">
            <div class="Front_Page_Text_Left">
                <b>Easy and Simple</b><br /><br />
                <p>Simply plug in your supported glucometer to your computer and sit back and relax.  No more dried up pens or fumbling with that old logbook.</p>
            </div>        
            <div class="Front_Page_Image_Right">
                <img src="Images/PlaceHolder.png" />
            </div>
        </div>
        <div class="Front_Page_Container">
            <div class="Front_Page_Image_Left">
                <img src="Images/PlaceHolder.png" />
            </div>        
            <div  class="Front_Page_Text_Right">
                <b>Consolidate</b><br /><br />
                <p>One brand of meter or several for home, car, and office, compile it all in one place and access it from anywhere.</p>
            </div>
        </div>
        <div class="Front_Page_Container">
            <div class="Front_Page_Text_Left">
                <b>Safe and secure</b><br /><br />
                <p>Safe, secure, encrypted.  Sleep easy knowing your information is protected with AES-256bit encryption.</p>
            </div>        
            <div class="Front_Page_Image_Right">
                <img src="Images/PlaceHolder.png" />
            </div>
        </div>
        <div class="Front_Page_Container">
            <div class="Front_Page_Image_Left">
                <img src="Images/PlaceHolder.png" />
            </div>        
            <div  class="Front_Page_Text_Right">
                <b>Change your life</b><br /><br />
                <p>Take control of your diabetes by viewing, and acting upon trends spotted on your personalized graphical dashboard.</p>
            </div>
        </div>        
    </div>
</asp:Content>
