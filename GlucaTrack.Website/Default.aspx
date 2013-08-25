<%@ Page Title="GlucaTrack" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GlucaTrack.Website._Default" %>

<asp:Content runat="server" ID="HeadContent" ContentPlaceHolderID="HeadContent">
    <script language="JavaScript1.1">
        <!--

        /*
        JavaScript Image slideshow:
        By JavaScript Kit (www.javascriptkit.com)
        Over 200+ free JavaScript here!
        */

        var slideimages=new Array()
        var slidelinks=new Array()
        function slideshowimages(){
        for (i=0;i<slideshowimages.arguments.length;i++){
        slideimages[i]=new Image()
        slideimages[i].src=slideshowimages.arguments[i]
        }
        }

        function slideshowlinks(){
        for (i=0;i<slideshowlinks.arguments.length;i++)
        slidelinks[i]=slideshowlinks.arguments[i]
        }

        function gotoshow(){
        if (!window.winslide||winslide.closed)
        winslide=window.open(slidelinks[whichlink])
        else
        winslide.location=slidelinks[whichlink]
        winslide.focus()
        }
        
        //-->
    </script>
</asp:Content>
<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <%--<div>
        <a href="SupportedMeters.aspx" id="SupportedMeters">Supported Meters</a>
    </div>--%>
    <div style="padding-top:10px;">
        <a><img src="/Images/home/rotator/test.png" name="slide" border=0 width=900 height=400></a>
        
        <script>
            //configure the paths of the images, plus corresponding target links
            var imageBasePath = "/Images/home/rotator/"
            slideshowimages(imageBasePath + "test.jpg", imageBasePath + "upload.jpg", imageBasePath + "track.jpg")

            //configure the speed of the slideshow, in milliseconds
            var slideshowspeed = 8000

            var whichlink = 0
            var whichimage = 0
            function slideit() {
                if (!document.images)
                    return
                document.images.slide.src = slideimages[whichimage].src
                whichlink = whichimage
                if (whichimage < slideimages.length - 1)
                    whichimage++
                else
                    whichimage = 0
                setTimeout("slideit()", slideshowspeed)
            }
            slideit()
        </script>
    </div>
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
