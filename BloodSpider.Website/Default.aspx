<%@ Page Title="BloodSpider" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BloodSpider.Website._Default" %>

<asp:Content runat="server" ID="HeadContent" ContentPlaceHolderID="HeadContent">
    <script src="js/ImageRotator.js"></script>
</asp:Content>
<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <%--<div>
        <a href="SupportedMeters.aspx" id="SupportedMeters">Supported Meters</a>
    </div>--%>
    <div style="padding-top:10px;">
        <a><img src="/Images/home/rotator/test.jpg" name="slide" border=0 width=900 height=400></a>
        
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
                <b><asp:Label id=lblFirstTitle runat="server">[Title 1]</asp:Label></b><br /><br />
                <p><asp:Label id=lblFirstContent runat="server">[Content 1]</asp:Label></p>
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
                <b><asp:Label id=lblSecondTitle runat="server">[Title 2]</asp:Label></b><br /><br />
                <p><asp:Label id=lblSecondContent runat="server">[Content 2]</asp:Label></p>
            </div>
        </div>
        <div class="Front_Page_Container">
            <div class="Front_Page_Text_Left">
                <b><asp:Label id=lblThirdTitle runat="server">[Title 3]</asp:Label></b><br /><br />
                <p><asp:Label id=lblThirdContent runat="server">[Content 3]</asp:Label></p>
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
                <b><asp:Label id=lblFourthTitle runat="server">[Title 4]</asp:Label></b><br /><br />
                <p><asp:Label id=lblFourthContent runat="server">[Content 4]</asp:Label></p>
            </div>
        </div>        
    </div>
</asp:Content>
