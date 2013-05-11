<%@ Page Title="My Data" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="GlucaTrack.Website.Content.Main" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Styles/dashboard.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="LastXDays_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetDataForLastXDays" SelectCommandType="StoredProcedure" >
        <SelectParameters>
            <asp:SessionParameter Name="userid" SessionField="LoggedInUserId" Type="Int32" />
            <asp:ControlParameter ControlID="ctl00$MainContent$ddDateRange" DefaultValue="7" Name="days" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    <div>&nbsp;</div>
    <div id="divTopOptions" runat="server" style="display: inline-block;">
        <asp:DropDownList ID="ddDateRange" runat="server" AutoPostBack="True">
            <asp:ListItem Text="Last 7 days" Value="7" Selected="True" />
            <asp:ListItem Text="Last 14 days" Value="14" />
            <asp:ListItem Text="Last 30 days" Value="30" />
            <asp:ListItem Text="Last 90 days" Value="90" />
            <asp:ListItem Text="Last 6 months" Value="180" />
            <asp:ListItem Text="Last 1 year" Value="365" />
        </asp:DropDownList>    
    </div>
    <div id="divMainChart" runat="server" style="margin-left:-105px;">
        <asp:Chart ID="chtLastXDays" runat="server" EnableTheming="True" BorderlineColor="255, 153, 0" OnDataBound="chtLastXDays_DataBound" Width="975">
            <Titles> 
                <asp:Title Text="Trending" Name="LastXDays_Title" />
            </Titles>
            <Series>
                <asp:Series Name="LastXDays_Series" ChartArea="LastXDays_ChartArea" ChartType="Line" BorderWidth="3" Color="0, 133, 198" ShadowOffset="1" />
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="LastXDays_ChartArea" BackHatchStyle="None" />
            </ChartAreas>
        </asp:Chart>
    </div>        
    <div>
        <table>
            <tr>
                <td>
                    <asp:Chart ID="chtMornings" runat="server" Height="125px" Width="125px" Palette="None" PaletteCustomColors="Yellow; 0, 192, 0; 192, 0, 0">
                        <Titles> 
                            <asp:Title Text="Mornings" Name="Title1" />
                        </Titles>
                        <Series>
                            <asp:Series Name="Mornings_Series" ChartType="Pie" ChartArea="Mornings_ChartArea" />
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="Mornings_ChartArea" />
                        </ChartAreas>
                    </asp:Chart>
                    <asp:Chart ID="chtAfternoons" runat="server" Height="125px" Width="125px" Palette="None" PaletteCustomColors="Yellow; 0, 192, 0; 192, 0, 0">
                        <Titles> 
                            <asp:Title Text="Afternoons" Name="Afternoons" />
                        </Titles>
                        <Series>
                            <asp:Series Name="Afternoons_Series" ChartType="Pie" ChartArea="Afternoons_ChartArea" />
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="Afternoons_ChartArea" />
                        </ChartAreas>
                    </asp:Chart>
                    <asp:Chart ID="chtNights" runat="server" Height="125px" Width="125px" Palette="None" PaletteCustomColors="Yellow; 0, 192, 0; 192, 0, 0">
                        <Titles> 
                            <asp:Title Text="Nights" Name="Nights" />
                        </Titles>
                        <Series>
                            <asp:Series Name="Nights_Series" ChartType="Pie" ChartArea="Nights_ChartArea" />
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="Nights_ChartArea" />
                        </ChartAreas>
                    </asp:Chart>
                </td>
                <td>
                    <asp:Panel runat="server" ID="RightTopSideBar" BackColor="White" Height="150px">
                        <p><asp:Label ID="lblMin" runat="server" Font-Bold="True" /><asp:Label ID="MinValue" runat="server" /></p>
                        <p><asp:Label ID="lblMax" runat="server" Font-Bold="True" /><asp:Label ID="MaxValue" runat="server" /></p>
                        <p><asp:Label ID="lblAvg" runat="server" Font-Bold="True" /><asp:Label ID="AvgValue" runat="server" /></p>
                        <p><asp:Label ID="lblStdDev" runat="server" Font-Bold="True" /><asp:Label ID="StdDevValue" runat="server" /></p>
                        <p><asp:Label ID="lblNumLows" runat="server" Font-Bold="True" /><asp:Label ID="NumLowsValue" runat="server" /><asp:Label ID="lblLowExplanation" runat="server" Font-Italic="True" /></p>
                        <p><asp:Label ID="lblNumHighs" runat="server" Font-Bold="True" /><asp:Label ID="NumHighsValue" runat="server" /><asp:Label ID="lblHighExplanation" runat="server" Font-Italic="True" /></p>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <asp:GridView 
        CssClass="mGrid"
        AllowPaging="True" 
        ID="gridValues" 
        runat="server" 
        AutoGenerateColumns="False" 
        DataSourceID="LastXDays_DataSource" OnRowDataBound="gridValues_RowDataBound" Width="800">
            <AlternatingRowStyle CssClass="alt" />
                <Columns>
                    <asp:BoundField DataField="date" HeaderText="Date" SortExpression="Timestamp" />
                    <asp:BoundField DataField="time" HeaderText="Time" SortExpression="Timestamp" />
                    <asp:TemplateField HeaderText="Glucose">
                    <ItemTemplate>
                        <div style="text-align: right;">
                            <asp:Label runat="server" ID="lblGlucoseValue" Text='<%# Bind("Glucose") %>' />
                            <asp:Image runat="server" ID="imgIndicator" AlternateText='<%# Bind("Glucose") %>' />
                        </div>
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="UnitType" HeaderText="UnitType" SortExpression="UnitType" />
                    <asp:BoundField DataField="Manufacturer" HeaderText="Manufacturer" SortExpression="Manufacturer" />
                    <asp:BoundField DataField="Meter" HeaderText="Meter" SortExpression="Meter" />
                </Columns>
            <PagerStyle CssClass="pgr" />
        </asp:GridView>        

    </div>
</asp:Content>
