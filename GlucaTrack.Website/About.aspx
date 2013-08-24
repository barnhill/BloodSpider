<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="GlucaTrack.Website.About" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div style="background-image:url('Images/about/about.jpg'); background-repeat:no-repeat; height:320px;">
        <div style="width:600px; padding:20px 0px 0px 0px;">
            <hgroup class="title">
                <h3><%: Title %> Our beginning and dream ...</h3>
            </hgroup><br /> 
            <article>        
                <p>        
                    GlucaTrack was dreamed up while watching a few diabetics suffer though the monotony of recording their glucose readings in a logbook or a cumbersome piece of software that required manual entry of the data.  
                    The thought of "there has to be a better way" crossed our minds just like it has so many diabetics in the past.  So in our lab (a home office, coffee shops, and online chats) GlucaTrack was born.  
                </p><br />        
                <p>        
                    Our dream is to offer a reliable, easy and secure method of online glucose reading collection for the masses.  
                    This thought has come along with others of how to utilize the data to help the community of diabetics as a whole, while continuing to offer valuable solutions for diabetics quickly and reliably.  
                    This is the story of our humble beginning, but we are happy that it is a story without end.  Every day we will continue to bring new ideas and new solutions to our friends. </p>
            </article> 
        </div>
    </div>
</asp:Content>