﻿<%@ Page Title="GlucaTrack" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GlucaTrack.Website._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div>
        <a href="SupportedMeters.aspx" id="SupportedMeters">Supported Meters</a>
    </div>
    <img style=" border-bottom:2px #ECEDEF solid; padding-bottom:30px;" src="Images/testcontent.png" />
    <div class="Front_Page_Main_Container">
        <div class="Front_Page_Container">
            <div class="Front_Page_Text_Left">
                <b>Easy and Simple</b><br /><br />
                <p>Simply plug in your supported glucometer to your computer and sit back and relax.  No more dried up pens or fumbling with that old logbook.</p>
            </div>        
            <div class="Front_Page_Image_Right">
                <img src="Images/home/simpleandeasy.png" />
            </div>
        </div>
        <div class="Front_Page_Container">
            <div class="Front_Page_Image_Left">
                <img src="Images/home/consolidate.png" />
            </div>        
            <div  class="Front_Page_Text_Right">
                <b>Consolidate</b><br /><br />
                <p>Whether you use one brand of meter or have several at home, work and in the car, bring all of your records together in one place accessible from anywhere.</p>
            </div>
        </div>
        <div class="Front_Page_Container">
            <div class="Front_Page_Text_Left">
                <b>Safe and Secure</b><br /><br />
                <p>Safe, secure, encrypted.  Sleep easy knowing your information is protected with AES-256bit data encryption and transported via 128bit SSL.</p>
            </div>        
            <div class="Front_Page_Image_Right">
                <img src="Images/home/safeandsecure.png" />
            </div>
        </div>
        <div class="Front_Page_Container">
            <div class="Front_Page_Image_Left">
                <img src="Images/home/changeyourlife.png" />
            </div>        
            <div  class="Front_Page_Text_Right">
                <b>Change Your Life</b><br /><br />
                <p> Make an investment in yourself by making more informed decisions for a healthier life.  Take control of your diabetes by viewing, and acting upon trends spotted on your personalized graphical dashboard.</p>
            </div>
        </div>        
    </div>
</asp:Content>
