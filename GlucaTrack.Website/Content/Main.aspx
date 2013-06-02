<%@ Page Title="My Data" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="GlucaTrack.Website.Content.Main" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Styles/dashboard.css" rel="stylesheet" />
    <script type="text/javascript" src="http://www.google.com/jsapi"></script>
    <script type="text/javascript">
        google.load('visualization', '1.1', { packages: ['gauge'] });
    </script>
    <script type="text/javascript">
        var gauge_HbA1c;
        var gaugeData_HbA1c;
        var gaugeOptions_HbA1c;

        var gauge_eAG;
        var gaugeData_eAG;
        var gaugeOptions_eAG;

        google.setOnLoadCallback(drawGauge_HbA1c);
        google.setOnLoadCallback(drawGauge_eAG);

        function drawGauge_HbA1c() {
            gaugeData_HbA1c = google.visualization.arrayToDataTable([
              ['HbA1c'],
              [5]
            ]);

            gauge_HbA1c = new google.visualization.Gauge(document.getElementById('gauge_HbA1c'));
            gaugeOptions_HbA1c = {
                min: 0,
                max: 15,
                greenFrom: 4.5,
                greenTo: 6,
                yellowFrom: 0,
                yellowTo: 4.5,
                redFrom: 6,
                redTo: 15,
                minorTicks: 1
            };
            gauge_HbA1c.draw(gaugeData_HbA1c, gaugeOptions_HbA1c);
        }

        function drawGauge_eAG() {
            gaugeData_eAG = google.visualization.arrayToDataTable([
              ['eAG'],
              [90]
            ]);

            gauge_eAG = new google.visualization.Gauge(document.getElementById('gauge_eAG'));
            gaugeOptions_eAG = {
                min: 0,
                max: 200,
                greenFrom: 82,
                greenTo: 126,
                yellowFrom: 0,
                yellowTo: 82,
                redFrom: 126,
                redTo: 200,
                minorTicks: 1
            };
            gauge_eAG.draw(gaugeData_eAG, gaugeOptions_eAG);
        }

        function change_HbA1c(dir) {
            gaugeData_HbA1c.setValue(0, 0, dir);
            gauge_HbA1c.draw(gaugeData_HbA1c, gaugeOptions_HbA1c);
        }

        function change_eAG(dir) {
            gaugeData_eAG.setValue(0, 0, dir);
            gauge_eAG.draw(gaugeData_eAG, gaugeOptions_eAG);
        }

    </script>
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
    <asp:SqlDataSource ID="PersonalSettings_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetPersonalSettings" SelectCommandType="StoredProcedure" >
        <SelectParameters>
            <asp:SessionParameter Name="login" SessionField="LoggedInUserId" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

    <div>&nbsp;</div>
    
    <asp:UpdatePanel ID="upMiddleContent" runat="server">
        <ContentTemplate>
            <div id="divTopOptions" runat="server" style="display: inline-block;">
                <asp:DropDownList ID="ddDateRange" runat="server" AutoPostBack="True" CssClass="dropdownlist">
                    <asp:ListItem Text="Last 7 days" Value="7" Selected="True" />
                    <asp:ListItem Text="Last 14 days" Value="14" />
                    <asp:ListItem Text="Last 30 days" Value="30" />
                    <asp:ListItem Text="Last 3 months" Value="90" />
                    <asp:ListItem Text="Last 6 months" Value="180" />
                    <asp:ListItem Text="Last 1 year" Value="365" />
                </asp:DropDownList>
                <asp:Button runat="server" ID="linkPersonalSettings" CssClass="LogButton" OnClick="linkPersonalSettings_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upNoData" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divNoData" runat="server">
                <asp:Label runat="server" ID="lblNoData" Text="" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel ID="upMainChart" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divMainChart" runat="server" style="margin-left:-105px;">
                <asp:Chart ID="chtLastXDays" runat="server" EnableTheming="True" BorderlineColor="255, 153, 0" OnDataBound="chtLastXDays_DataBound" Width="975">
                    <Titles> 
                        <asp:Title Text="Trending" Name="LastXDays_Title" />
                    </Titles>
                    <Series>
                        <asp:Series Name="LastXDays_Series" ChartArea="LastXDays_ChartArea" ChartType="Line" BorderWidth="3" Color="196, 0, 0" ShadowOffset="1" IsValueShownAsLabel="False"/>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="LastXDays_ChartArea" BackHatchStyle="None" />
                    </ChartAreas>
                </asp:Chart>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>  

    <div>
        <table>
            <tr>
                <td>
                    <asp:UpdatePanel ID="upPieGraphs" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Chart ID="chtMornings" runat="server" Height="125px" Width="125px" Palette="None" PaletteCustomColors="255, 153, 0; 16, 150, 24; 220, 57, 18">
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
                            <asp:Chart ID="chtAfternoons" runat="server" Height="125px" Width="125px" Palette="None" PaletteCustomColors="255, 153, 0; 16, 150, 24; 220, 57, 18">
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
                            <asp:Chart ID="chtNights" runat="server" Height="125px" Width="125px" Palette="None" PaletteCustomColors="255, 153, 0; 16, 150, 24; 220, 57, 18">
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
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <asp:UpdatePanel ID="upStatistics" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="divStatistics" runat="server">
                                <p><asp:Label ID="lblMin" runat="server" Font-Bold="True" /><asp:Label ID="MinValue" runat="server" /></p>
                                <p><asp:Label ID="lblMax" runat="server" Font-Bold="True" /><asp:Label ID="MaxValue" runat="server" /></p>
                                <p><asp:Label ID="lblAvg" runat="server" Font-Bold="True" /><asp:Label ID="AvgValue" runat="server" /></p>
                                <p><asp:Label ID="lblStdDev" runat="server" Font-Bold="True" /><asp:Label ID="StdDevValue" runat="server" /></p>
                                <p><asp:Label ID="lblNumLows" runat="server" Font-Bold="True" /><asp:Label ID="NumLowsValue" runat="server" /><asp:Label ID="lblLowExplanation" runat="server" Font-Italic="True" /></p>
                                <p><asp:Label ID="lblNumHighs" runat="server" Font-Bold="True" /><asp:Label ID="NumHighsValue" runat="server" /><asp:Label ID="lblHighExplanation" runat="server" Font-Italic="True" /></p>
                                <p><asp:Label ID="lblHbA1c" runat="server" Font-Bold="True" /><asp:Label ID="NumHbA1c" runat="server" ForeColor="#3366FF" /></p>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <div id="gauge_HbA1c" style="width: 150px; height: 150px;" />
                </td>
            </tr>
        </table>
    </div>

    
    <table>
        <tr>
            <td>
                <asp:UpdatePanel ID="upGridOfValues" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="GridHolder">
                        <!-- Raw Values Table -->
                            <asp:GridView 
                                CssClass="mGrid"
                                AllowPaging="False" 
                                ID="gridValues" 
                                runat="server" 
                                AutoGenerateColumns="False" 
                                DataSourceID="LastXDays_DataSource" 
                                OnRowDataBound="gridValues_RowDataBound">
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
                                    </Columns>
                                <PagerStyle CssClass="pgr" />
                            </asp:GridView>
                        </div> 
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td>
                <div id="gauge_eAG" style="width: 150px; height: 150px;" />
            </td>
        </tr>
    </table>

    <br />
</asp:Content>
