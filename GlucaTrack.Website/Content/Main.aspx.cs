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

namespace GlucaTrack.Website.Content
{
    public partial class Main : System.Web.UI.Page
    {
        //TODO: point system for uploading data.
        public string ImagePath { get; set; }

        Queries.sp_GetLoginRow LoginRow = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            //redirect if not logged in
            if (Session["LoggedInUser"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                LoginRow = (Queries.sp_GetLoginRow)Session["LoggedInUser"];

                if (Session["LoggedInUserId"] != null)
                    Session["LoggedInUserId"] = LoginRow.user_id;
                else
                    Session.Add("LoggedInUserId", LoginRow.user_id);
            }

            SetResources();

            PopulateDashboard();
        }

        private void SetResources()
        {
            chtMornings.Titles[0].Text = Resources.Content_Strings.GraphTitle_PieMornings;
            chtAfternoons.Titles[0].Text = Resources.Content_Strings.GraphTitle_PieAfternoons;
            chtNights.Titles[0].Text = Resources.Content_Strings.GraphTitle_PieNights;
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

                    GridOfValues();

                    MinMaxValues(dt);

                    PieCharts(dt);
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
            chtLastXDays.Series["LastXDays_Series"].IsValueShownAsLabel = true;
            chtLastXDays.Titles[chtLastXDays.Titles.Count - 1].Text = string.Empty; // string.Format(Resources.Content_Strings.GraphTitle_Main, ddDateRange.SelectedValue);
            chtLastXDays.ChartAreas["LastXDays_ChartArea"].AxisX.Title = "Date Range";
            chtLastXDays.ChartAreas["LastXDays_ChartArea"].AxisY.Title = "Glucose Reading";
            chtLastXDays.DataSource = DataSource;
            chtLastXDays.DataBind();
        }

        private void GridOfValues()
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
            var NumHigh = dt.Compute("Count(Glucose)", "Glucose > " + Statics.HighThreshold.ToString());
            var NumLow = dt.Compute("Count(Glucose)", "Glucose < " + Statics.LowThreshold.ToString());

            //populate values on the right side
            if (min.Count() > 0 && max.Count() > 0)
            {
                this.lblMin.Text = Resources.Content_Strings.Label_Minimum;
                this.MinValue.Text = min[0]["Glucose"].ToString();
                this.lblMax.Text = Resources.Content_Strings.Label_Maximum;
                this.MaxValue.Text = max[0]["Glucose"].ToString();
            }

            this.lblAvg.Text = Resources.Content_Strings.Label_Average;
            this.AvgValue.Text = avg.ToString();
            this.lblStdDev.Text = Resources.Content_Strings.Label_StandardDeviation;
            this.StdDevValue.Text = (stdev.ToString().Length > 0) ? Math.Round(Convert.ToDouble(stdev.ToString()), 2).ToString() : string.Empty;
            this.lblNumLows.Text = Resources.Content_Strings.Label_NumberOfLows;
            this.NumLowsValue.Text = NumLow.ToString();
            this.lblLowExplanation.Text = string.Format(Resources.Content_Strings.Label_LowExplanation, Statics.LowThreshold.ToString());
            this.lblNumHighs.Text = Resources.Content_Strings.Label_NumberOfHighs;
            this.NumHighsValue.Text = NumHigh.ToString();
            this.lblHighExplanation.Text = string.Format(Resources.Content_Strings.Label_HighExplanation, Statics.HighThreshold.ToString());
        }

        private void PieCharts(DataTable dt)
        {
            ReadingPiePercents pieMorn = new ReadingPiePercents(dt, new DateTime(1, 1, 1, 6, 0, 0), new DateTime(1, 1, 1, 11, 59, 59));
            chtMornings.Series["Mornings_Series"].XValueMember = "Label";
            chtMornings.Series["Mornings_Series"].YValueMembers = "Value";
            chtMornings.Series["Mornings_Series"].IsValueShownAsLabel = false;
            chtMornings.ChartAreas["Mornings_ChartArea"].Area3DStyle.Enable3D = true;
            chtMornings.ChartAreas["Mornings_ChartArea"].Area3DStyle.Inclination = 50;
            chtMornings.DataSource = pieMorn.BindingSource;
            chtMornings.DataBind();

            ReadingPiePercents pieAfter = new ReadingPiePercents(dt, new DateTime(1, 1, 1, 12, 0, 0), new DateTime(1, 1, 1, 20, 59, 59));
            chtAfternoons.Series["Afternoons_Series"].XValueMember = "Label";
            chtAfternoons.Series["Afternoons_Series"].YValueMembers = "Value";
            chtAfternoons.Series["Afternoons_Series"].IsValueShownAsLabel = false;
            chtAfternoons.ChartAreas["Afternoons_ChartArea"].Area3DStyle.Enable3D = true;
            chtAfternoons.ChartAreas["Afternoons_ChartArea"].Area3DStyle.Inclination = 50;
            chtAfternoons.DataSource = pieAfter.BindingSource;
            chtAfternoons.DataBind();

            ReadingPiePercents pieNight = new ReadingPiePercents(dt, new DateTime(1, 1, 1, 21, 0, 0), new DateTime(1, 1, 1, 5, 59, 59));
            chtNights.Series["Nights_Series"].XValueMember = "Label";
            chtNights.Series["Nights_Series"].YValueMembers = "Value";
            chtNights.Series["Nights_Series"].IsValueShownAsLabel = false;
            chtNights.ChartAreas["Nights_ChartArea"].Area3DStyle.Enable3D = true;
            chtNights.ChartAreas["Nights_ChartArea"].Area3DStyle.Inclination = 50;
            chtNights.DataSource = pieNight.BindingSource;
            chtNights.DataBind();
        }

        protected void chtLastXDays_DataBound(object sender, EventArgs e)
        {
            if (chtLastXDays.Series[0].Points.Count == 0)
            {
                System.Web.UI.DataVisualization.Charting.TextAnnotation annotation =
                    new System.Web.UI.DataVisualization.Charting.TextAnnotation();
                annotation.Text = "No data for this period";
                annotation.X = 5;
                annotation.Y = 5;
                annotation.Font = new System.Drawing.Font("Arial", 12);
                annotation.ForeColor = System.Drawing.Color.Red;
                chtLastXDays.Annotations.Add(annotation);
                chtLastXDays.ChartAreas["LastXDays_ChartArea"].ShadowOffset = 0;
                chtMornings.Visible = false;
                chtAfternoons.Visible = false;
                chtNights.Visible = false;
                RightTopSideBar.Visible = false;
            }
            else
            {
                chtMornings.Visible = true;
                chtAfternoons.Visible = true;
                chtNights.Visible = true;
                RightTopSideBar.Visible = true;

                chtLastXDays.Series["LastXDays_Series"].IsValueShownAsLabel = false;
                chtLastXDays.ChartAreas["LastXDays_ChartArea"].ShadowOffset = 5;

                //add normal range
                //TODO: finish normal range
                chtLastXDays.Series["LastXDays_ChartArea"].Points.Add(chtLastXDays.Series["LastXDays_Series"][chtLastXDays.Series["LastXDays_Series"].XValueMember].Max(), GetPersonalNormalRange().XValue);
            }
        }

        private DataPoint GetPersonalNormalRange()
        {
            using (QueriesTableAdapters.sp_GetPersonalSettingsTableAdapter ta = new QueriesTableAdapters.sp_GetPersonalSettingsTableAdapter())
            {
                using (Queries.sp_GetPersonalSettingsDataTable dt = new Queries.sp_GetPersonalSettingsDataTable())
                {
                    ta.Fill(dt, LoginRow.user_id);

                    return new DataPoint(dt.FirstOrDefault().lownormal, dt.FirstOrDefault().highnormal);
                }
            }

            return null;
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
}