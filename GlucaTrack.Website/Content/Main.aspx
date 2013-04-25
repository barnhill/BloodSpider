<%@ Page Title="My Data" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="GlucaTrack.Website.Content.Main" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Styles/dashboard.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="LastXDays_DataSource" runat="server" ConnectionString="<%$ ConnectionStrings:glucatrackConnectionString %>" SelectCommand="sp_GetDataForTimeframe" SelectCommandType="StoredProcedure" >
        <SelectParameters>
            <asp:SessionParameter Name="userid" SessionField="LoggedInUserId" Type="Int32" />
            <asp:SessionParameter DefaultValue="7" Name="days" SessionField="NumDaysView" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

  <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="fancy-green" UseVerticalStripPlacement="True" VerticalStripWidth="200px" BorderColor="Black" BorderStyle="Dotted" BorderWidth="1px">
        <ajaxToolkit:TabPanel runat="server" HeaderText="Dashboard" ID="Dashboard">
            <ContentTemplate>
                <div runat="server" id="divTopOptions">
                        <div runat="server" style="display: inline-block;">
                            <asp:Button ID="link7_Days" CssClass="button blue" runat="server" OnClick="link7_Days_Click" />
                        </div>
                        <div runat="server" style="display: inline-block;">
                            <asp:Button ID="link30_Days" CssClass="button blue" runat="server" OnClick="link30_Days_Click" />
                        </div>
                        <div runat="server" style="display: inline-block;">
                            <asp:Button ID="link1_Year" CssClass="button blue" runat="server" OnClick="link1_Year_Click" />
                        </div>
                        <div runat="server" id="div3dGraphs" style="display:inline-block;">
                            <asp:CheckBox ID="Enable3dGraphs" runat="server" OnCheckedChanged="Enable3dGraphs_CheckedChanged" AutoPostBack="True" />
                            <asp:Label ID="Label_Enable3dGraphs" runat="server" AssociatedControlID="Enable3dGraphs" />
                        </div>
                </div>
                <table>
                    <tr>
                        <td>
                            <asp:Chart ID="chtLastXDays" runat="server" EnableTheming="False" BorderlineColor="255, 153, 0">
                                <Titles> 
                                    <asp:Title Text="Last X Days" Name="LastXDays_Title" />
                                </Titles>
                                    <Series>
                                        <asp:Series Name="LastXDays_Series" ChartArea="LastXDays_ChartArea" ChartType="Line" BorderWidth="3" Color="0, 133, 198" />
                                    </Series>
                                    <ChartAreas>
                                        <asp:ChartArea Name="LastXDays_ChartArea" />
                                    </ChartAreas>
                            </asp:Chart>
                            <ajaxToolkit:RoundedCornersExtender ID="chtLastXDays_RoundedCornersExtender" runat="server" Enabled="True" Radius="8" TargetControlID="chtLastXDays">
                            </ajaxToolkit:RoundedCornersExtender>
                        </td>
                        <td>
                            <asp:Panel runat="server" ID="RightTopSideBar" BackColor="White" Height="150px">
                                <p><asp:Label ID="lblMin" runat="server" Font-Bold="True" /><asp:Label ID="MinValue" runat="server" /></p>
                                <p><asp:Label ID="lblMax" runat="server" Font-Bold="True" /><asp:Label ID="MaxValue" runat="server" /></p>
                                <p><asp:Label ID="lblAvg" runat="server" Font-Bold="True" /><asp:Label ID="AvgValue" runat="server" /></p>
                                <p><asp:Label ID="lblNumLows" runat="server" Font-Bold="True" /><asp:Label ID="NumLowsValue" runat="server" /><asp:Label ID="lblLowExplanation" runat="server" Font-Italic="True" /></p>
                                <p><asp:Label ID="lblNumHighs" runat="server" Font-Bold="True" /><asp:Label ID="NumHighsValue" runat="server" /><asp:Label ID="lblHighExplanation" runat="server" Font-Italic="True" /></p>
                            </asp:Panel>
                            <ajaxToolkit:RoundedCornersExtender ID="RoundedCornersExtender1" runat="server" Enabled="True" Radius="8" TargetControlID="RightTopSideBar">
                            </ajaxToolkit:RoundedCornersExtender>
                        </td>
                    </tr>
                    <tr>
                        <td rowspan="3">
                            <asp:GridView 
                                CssClass="mGrid"
                                AllowPaging="True" 
                                ID="gridValues" 
                                runat="server" 
                                AutoGenerateColumns="False" 
                                DataSourceID="LastXDays_DataSource" OnRowDataBound="gridValues_RowDataBound">
                                <AlternatingRowStyle CssClass="alt" />
                                <Columns>
                                    <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp" SortExpression="TimeStamp" />
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
                        </td>
                        <td>
                            <asp:Chart ID="chtMornings" runat="server" Height="125px" Width="125px">
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
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Chart ID="chtAfternoons" runat="server" Height="125px" Width="125px">
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
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Chart ID="chtNights" runat="server" Height="125px" Width="125px">
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
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" HeaderText="HbA1C Estimator" ID="HbA1C_Estimator">
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>  
    
</asp:Content>
