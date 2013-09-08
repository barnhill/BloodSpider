using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web.UI.DataVisualization.Charting;
using GlucaTrack.Website.Resources;

namespace GlucaTrack.Website.Content
{
    public partial class Main : System.Web.UI.Page
    {
        //TODO: point system for uploading data.
        public string ImagePath { get; set; }

        GTService.Common.sp_GetLoginRow LoginRow = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            //redirect if not logged in
            if (Session["LoggedInUser"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                LoginRow = (GTService.Common.sp_GetLoginRow)Session["LoggedInUser"];

                if (Session["LoggedInUserId"] != null)
                    Session["LoggedInUserId"] = LoginRow.user_id;
                else
                    Session.Add("LoggedInUserId", LoginRow.user_id);

                if (!IsPostBack && Session["SelectedDays"] != null)
                {
                    //selected days exists so set the dropdown accordingly
                    ddDateRange.SelectedItem.Selected = false;
                    foreach (ListItem item in ddDateRange.Items)
                    {
                        if (Convert.ToInt16(Session["SelectedDays"].ToString()) == Convert.ToInt16(item.Value))
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }

            SetResources();

            PopulateDashboard();

            upMainChart.Update();
        }

        private void StoreSelectedDateRangeSetting()
        {
            try
            {
                Session.Remove("SelectedDays");
            }
            catch { }
            Session.Add("SelectedDays", ddDateRange.SelectedValue);
        }

        private void SetResources()
        {
            chtMornings.Titles[0].Text = Content_Strings.GraphTitle_PieMornings;
            chtAfternoons.Titles[0].Text = Content_Strings.GraphTitle_PieAfternoons;
            chtNights.Titles[0].Text = Content_Strings.GraphTitle_PieNights;
            lblNoData.Text = Content_Strings.Label_Nodata;
            linkPersonalSettings.Text = Content_Strings.Button_PersonalSettings;
            lblMin.Text = Content_Strings.Label_Minimum;
            lblMax.Text = Content_Strings.Label_Maximum;
            lblAvg.Text = Content_Strings.Label_Average;
            lblStdDev.Text = Content_Strings.Label_StandardDeviation;
            lblNumLows.Text = Content_Strings.Label_NumberOfLows;
            lblNumHighs.Text = Content_Strings.Label_NumberOfHighs;

            lblMainGraph_SectionHeader.Text = string.Format(Content_Strings.Label_MainGraphSectionHeader, ddDateRange.SelectedValue);
            lblRawValues_SectionHeader.Text = Content_Strings.Label_RawValuesSectionHeader;
            lblStatsAndGauges_SectionHeader.Text = Content_Strings.Label_GraphsAndGaugesSectionHeader;
        }

        private void PopulateDashboard()
        {
            var series = chtLastXDays.Series["LastXDays_Series"];
            series.XValueType = ChartValueType.Date;

            using (QueriesTableAdapters.sp_GetDataForLastXDaysTableAdapter ta = new QueriesTableAdapters.sp_GetDataForLastXDaysTableAdapter())
            {
                using (Queries.sp_GetDataForLastXDaysDataTable dt = new Queries.sp_GetDataForLastXDaysDataTable())
                {
                    ta.Fill(dt, LoginRow.user_id, Convert.ToInt32(ddDateRange.SelectedValue));

                    MainGraph(dt);

                    GridOfValues(dt);

                    MinMaxValues(dt);

                    PieCharts(dt);

                    GetHbA1c();

                    RefreshUpdatePanels();
                }
            }
        }

        protected void gridValues_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Image image = (Image)e.Row.FindControl("imgIndicator");
            string glucose = (image != null) ? image.AlternateText : string.Empty;
            
            int num;
            if (Int32.TryParse(glucose, out num))
            {
                //glucose number was converted successfully so set image indicator
                if (num > Statics.HighThreshold)
                {
                    //high
                    image.ImageUrl = Resources.Images.arrow_high_path;
                }
                else if (num < Statics.LowThreshold)
                {
                    //low
                    image.ImageUrl = Resources.Images.arrow_low_path;
                }
                else
                {
                    //normal
                    image.ImageUrl = Resources.Images.normal_path;
                }
            }
        }

        private void MainGraph(object DataSource)
        {
            //last X days graph
            chtLastXDays.Series["LastXDays_Series"].XValueMember = "TimeStamp";
            chtLastXDays.Series["LastXDays_Series"].YValueMembers = "Glucose";
            chtLastXDays.Series["LastXDays_Series"].ToolTip = "Glucose: #VALY";
            chtLastXDays.Series["LastXDays_Series"].IsValueShownAsLabel = true;
            chtLastXDays.ChartAreas["LastXDays_ChartArea"].AxisX.Title = Resources.Content_Strings.AxisLabel_X_MainGraph;
            chtLastXDays.ChartAreas["LastXDays_ChartArea"].AxisY.Title = Resources.Content_Strings.AxisLabel_Y_MainGraph;
            chtLastXDays.DataSource = DataSource;
            chtLastXDays.DataBind();
        }

        private void GridOfValues(object DataSource)
        {
            //grid of values
            gridValues.DataBind();
        }

        private void MinMaxValues(DataTable dt)
        {
            //min max values
            var min = dt.Select("Glucose = MIN(Glucose)");
            var max = dt.Select("Glucose = MAX(Glucose)");
            var avg = dt.Compute("AVG(Glucose)", null);
            var stdev = dt.Compute("StDev(Glucose)", null);
            var variance = dt.Compute("Var(Glucose)", null).ToString();
            var NumHigh = dt.Compute("Count(Glucose)", "Glucose > " + GetPersonalNormalRange().Y);
            var NumLow = dt.Compute("Count(Glucose)", "Glucose < " + GetPersonalNormalRange().X);

            //populate statistical values
            if (min.Count() > 0 && max.Count() > 0)
            {
                this.MinValue.Text = min[0]["Glucose"].ToString();
                this.MaxValue.Text = max[0]["Glucose"].ToString();
            }

            this.AvgValue.Text = avg.ToString();
            this.StdDevValue.Text = (stdev.ToString().Length > 0) ? Math.Round(Convert.ToDouble(stdev.ToString()), 2).ToString() : string.Empty;
            this.NumLowsValue.Text = NumLow.ToString();
            this.lblLowExplanation.Text = string.Format(Resources.Content_Strings.Label_LowExplanation, GetPersonalNormalRange().X);
            this.NumHighsValue.Text = NumHigh.ToString();
            this.lblHighExplanation.Text = string.Format(Resources.Content_Strings.Label_HighExplanation, GetPersonalNormalRange().Y);
        }

        private void PieCharts(DataTable dt)
        {
            ReadingPiePercents pieMorn = new ReadingPiePercents(dt, new DateTime(1, 1, 1, GetUserTimeSettings().Morning, 0, 0), new DateTime(1, 1, 1, GetUserTimeSettings().Afternoon, 0, 0).AddMilliseconds(-1));
            chtMornings.Series["Mornings_Series"].XValueMember = "Label";
            chtMornings.Series["Mornings_Series"].YValueMembers = "Value";
            chtMornings.Series["Mornings_Series"].IsValueShownAsLabel = false;
            chtMornings.ChartAreas["Mornings_ChartArea"].Area3DStyle.Enable3D = true;
            chtMornings.ChartAreas["Mornings_ChartArea"].Area3DStyle.Inclination = 50;
            chtMornings.DataSource = pieMorn.BindingSource;
            chtMornings.DataBind();

            ReadingPiePercents pieAfter = new ReadingPiePercents(dt, new DateTime(1, 1, 1, GetUserTimeSettings().Afternoon, 0, 0), new DateTime(1, 1, 1, GetUserTimeSettings().Night, 0, 0).AddMilliseconds(-1));
            chtAfternoons.Series["Afternoons_Series"].XValueMember = "Label";
            chtAfternoons.Series["Afternoons_Series"].YValueMembers = "Value";
            chtAfternoons.Series["Afternoons_Series"].IsValueShownAsLabel = false;
            chtAfternoons.ChartAreas["Afternoons_ChartArea"].Area3DStyle.Enable3D = true;
            chtAfternoons.ChartAreas["Afternoons_ChartArea"].Area3DStyle.Inclination = 50;
            chtAfternoons.DataSource = pieAfter.BindingSource;
            chtAfternoons.DataBind();

            ReadingPiePercents pieNight = new ReadingPiePercents(dt, new DateTime(1, 1, 1, GetUserTimeSettings().Night, 0, 0), new DateTime(1, 1, 1, GetUserTimeSettings().Morning, 0, 0).AddMilliseconds(-1));
            chtNights.Series["Nights_Series"].XValueMember = "Label";
            chtNights.Series["Nights_Series"].YValueMembers = "Value";
            chtNights.Series["Nights_Series"].IsValueShownAsLabel = false;
            chtNights.ChartAreas["Nights_ChartArea"].Area3DStyle.Enable3D = true;
            chtNights.ChartAreas["Nights_ChartArea"].Area3DStyle.Inclination = 50;
            chtNights.DataSource = pieNight.BindingSource;
            chtNights.DataBind();
        }

        private void RefreshUpdatePanels()
        {
            upMainChart.Update();
            upNoData.Update();
            upPieGraphs.Update();
            upGridOfValues.Update();
            upStatistics.Update();
        }

        protected void chtLastXDays_DataBound(object sender, EventArgs e)
        {
            if (chtLastXDays.Series[0].Points.Count == 0)
            {
                //no data to show
                chtLastXDays.ChartAreas["LastXDays_ChartArea"].ShadowOffset = 0;
                chtMornings.Visible = false;
                chtAfternoons.Visible = false;
                chtNights.Visible = false;
                divStatistics.Visible = false;
                divMainChart.Visible = false;
                divNoData.Visible = true;
            }
            else
            {
                //data is present
                chtMornings.Visible = true;
                chtAfternoons.Visible = true;
                chtNights.Visible = true;
                divStatistics.Visible = true;
                divMainChart.Visible = true;
                divNoData.Visible = false;
                
                chtLastXDays.Series["LastXDays_Series"].IsValueShownAsLabel = false;
                chtLastXDays.ChartAreas["LastXDays_ChartArea"].ShadowOffset = 5;

                //add personalized 'normal' range
                this.chtLastXDays.ChartAreas["LastXDays_ChartArea"].AxisY.StripLines.Add(HighlightNormalRange(GetPersonalNormalRange()));
            }
        }

        /// <summary>
        /// Gets the users normal range set in personal settings.
        /// </summary>
        /// <returns>Point that contains the low (X) and high (Y) of the users personal settings normal range.</returns>
        private System.Drawing.Point GetPersonalNormalRange()
        {
            using (QueriesTableAdapters.sp_GetUserSettingsTableAdapter ta = new QueriesTableAdapters.sp_GetUserSettingsTableAdapter())
            {
                using (Queries.sp_GetUserSettingsDataTable dt = new Queries.sp_GetUserSettingsDataTable())
                {
                    ta.Fill(dt, LoginRow.user_id);

                    return new System.Drawing.Point(dt.FirstOrDefault().lownormal, dt.FirstOrDefault().highnormal);
                }
            }
        }

        private MorningAfternoonNight GetUserTimeSettings()
        {
            using (QueriesTableAdapters.sp_GetUserSettingsTableAdapter ta = new QueriesTableAdapters.sp_GetUserSettingsTableAdapter())
            {
                using (Queries.sp_GetUserSettingsDataTable dt = new Queries.sp_GetUserSettingsDataTable())
                {
                    ta.Fill(dt, LoginRow.user_id);

                    if (dt.Rows.Count > 0)
                    {
                        Queries.sp_GetUserSettingsRow row = (Queries.sp_GetUserSettingsRow)dt[0];
                        return new MorningAfternoonNight(row.start_morning, row.start_afternoon, row.start_night);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the strip line representing the normal range on the graph
        /// </summary>
        /// <param name="normalRange">Point that contains the low (X) and high (Y) of the users personal settings normal range.</param>
        /// <returns>Strip line representing the normal range on the graph</returns>
        private StripLine HighlightNormalRange(System.Drawing.Point normalRange)
        {
            StripLine sl = new StripLine();
            double low = normalRange.X;
            double high = normalRange.Y;

            sl.Interval = 10000; //set the interval high enough that only one strip line will show
            sl.BackColor = System.Drawing.Color.FromArgb(60, 16, 150, 24);
            sl.StripWidth = high - low; //set width of strip to width of normal range
            sl.IntervalOffset = low; //start the first strip line at the bottom of range

            return sl;
        }

        protected void linkPersonalSettings_Click(object sender, EventArgs e)
        {
            Session.Remove("OnSave");

            Session.Add("OnSave", "/Content/Main.aspx");
            Response.Redirect("../Account/PersonalSettings.aspx");
        }

        private void GetHbA1c()
        {
            using (QueriesTableAdapters.sp_GetHbA1cTableAdapter ta = new QueriesTableAdapters.sp_GetHbA1cTableAdapter())
            {
                using (Queries.sp_GetHbA1cDataTable dt = new Queries.sp_GetHbA1cDataTable())
                {
                    ta.Fill(dt, LoginRow.user_id, DateTime.Now);

                    Session.Remove("HbA1c_Value");
                    Session.Remove("eAG_Value");

                    if (dt.Rows.Count > 0)
                    {
                        //HbA1c value returned
                        double HbA1c = dt.FirstOrDefault().HbA1c;
                        double eAG = (HbA1c == 0) ? 0 : Math.Round(28.7 * HbA1c - 46.7, 1, MidpointRounding.AwayFromZero);
                        Session.Add("HbA1c_Value", HbA1c);
                        Session.Add("eAG_Value", eAG);
                    }
                    else
                    {
                        //no HbA1c value returned
                    }
                }
            }
        }

        protected void ddDateRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            StoreSelectedDateRangeSetting();

            SetResources();

            PopulateDashboard();

            upMainChart.Update();
        }
    }

    public class ReadingPiePercents
    {
        public double PercentLow { get; set; }
        public double PercentNormal { get; set; }
        public double PercentHigh { get; set; }

        public DataTable BindingSource { get; set; }

        public ReadingPiePercents()
        {
            PercentLow = 0;
            PercentNormal = 0;
            PercentHigh = 0;
        }

        public ReadingPiePercents(DataTable dt, DateTime start, DateTime stop)
            : this()
        {
            CalculatePieChart(dt, start, stop);
        }

        private void CalculatePieChart(DataTable dt, DateTime start, DateTime stop)
        {
            Queries.sp_GetDataForLastXDaysDataTable castDT = (Queries.sp_GetDataForLastXDaysDataTable)dt;
            if (start.TimeOfDay < stop.TimeOfDay)
            {
                PercentLow = 100 * Convert.ToDouble(castDT.Where(i => i.Glucose <= Statics.LowThreshold &&
                                            i.TimeStamp.TimeOfDay >= start.TimeOfDay &&
                                            i.TimeStamp.TimeOfDay <= stop.TimeOfDay).Count()) / Convert.ToDouble(dt.Rows.Count);

                PercentHigh = 100 * Convert.ToDouble(castDT.Where(i => i.Glucose >= Statics.HighThreshold &&
                                             i.TimeStamp.TimeOfDay >= start.TimeOfDay &&
                                             i.TimeStamp.TimeOfDay <= stop.TimeOfDay).Count()) / Convert.ToDouble(dt.Rows.Count);

                PercentNormal = 100 * Convert.ToDouble(castDT.Where(i => i.Glucose > Statics.LowThreshold &&
                                               i.Glucose < Statics.HighThreshold &&
                                               i.TimeStamp.TimeOfDay >= start.TimeOfDay &&
                                               i.TimeStamp.TimeOfDay <= stop.TimeOfDay).Count()) / Convert.ToDouble(dt.Rows.Count);
            }
            else
            {
                PercentLow = 100 * Convert.ToDouble(castDT.Where(i => i.Glucose <= Statics.LowThreshold &&
                                            i.TimeStamp.TimeOfDay >= start.TimeOfDay ||
                                            i.TimeStamp.TimeOfDay <= stop.TimeOfDay).Count()) / Convert.ToDouble(dt.Rows.Count);

                PercentHigh = 100 * Convert.ToDouble(castDT.Where(i => i.Glucose >= Statics.HighThreshold &&
                                             i.TimeStamp.TimeOfDay >= start.TimeOfDay ||
                                             i.TimeStamp.TimeOfDay <= stop.TimeOfDay).Count()) / Convert.ToDouble(dt.Rows.Count);

                PercentNormal = 100 * Convert.ToDouble(castDT.Where(i => (i.Glucose > Statics.LowThreshold &&
                                               i.Glucose < Statics.HighThreshold) &&
                                               i.TimeStamp.TimeOfDay >= start.TimeOfDay ||
                                               i.TimeStamp.TimeOfDay <= stop.TimeOfDay).Count()) / Convert.ToDouble(dt.Rows.Count);
            }

            BindingSource = new DataTable("Pie");
            BindingSource.Columns.Add("Label");
            BindingSource.Columns.Add("Value");
            BindingSource.Rows.Add(new object[] { "", AllAreBlank() ? 1 : PercentLow });
            BindingSource.Rows.Add(new object[] { "", AllAreBlank() ? 98 : PercentNormal });
            BindingSource.Rows.Add(new object[] { "", AllAreBlank() ? 1 : PercentHigh });
        }

        private bool AllAreBlank()
        {
            return PercentLow == 0 && PercentNormal == 0 && PercentHigh == 0;
        }
    }

    public class MorningAfternoonNight
    {
        public int Morning;
        public int Afternoon;
        public int Night;

        public MorningAfternoonNight(int morning, int afternoon, int night)
        {
            Morning = morning;
            Afternoon = afternoon;
            Night = night;
        }
    }
}