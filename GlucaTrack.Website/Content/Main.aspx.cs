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
        static int LowThreshold = 70;
        static int HighThreshold = 110;

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

            this.drPicker.ToDate = DateTime.Today;
            this.drPicker.FromDate = drPicker.ToDate.AddDays(-7);

            PopulateDashboard();
        }

        private void SetResources()
        {
            this.Label_Enable3dGraphs.Text = Resources.Content_Strings.Option_3DGraphs;
        }

        private void PopulateDashboard()
        {
            var series = chtLastXDays.Series["LastXDays_Series"];
            series.XValueType = ChartValueType.Date;

            using (QueriesTableAdapters.sp_GetDataForLastXDaysTableAdapter ta = new QueriesTableAdapters.sp_GetDataForLastXDaysTableAdapter())
            {
                using (Queries.sp_GetDataForLastXDaysDataTable dt = new Queries.sp_GetDataForLastXDaysDataTable())
                {
                    ta.Fill(dt, LoginRow.user_id, drPicker.ToDate, drPicker.DaysInRange);

                    //TODO: move these sections to separate functions to clean this function up

                    //last X days graph
                    chtLastXDays.Series["LastXDays_Series"].XValueMember = "TimeStamp";
                    chtLastXDays.Series["LastXDays_Series"].YValueMembers = "Glucose";
                    chtLastXDays.Series["LastXDays_Series"].IsValueShownAsLabel = true;
                    chtLastXDays.Width = (int)(Math.Round(Request.Browser.ScreenPixelsWidth * 0.85, 0, MidpointRounding.AwayFromZero));
                    chtLastXDays.DataSource = dt;
                    chtLastXDays.DataBind();

                    //grid of values
                    gridValues.DataBind();
                   
                    //min max values
                    var min = dt.Select("Glucose = MIN(Glucose)");
                    var max = dt.Select("Glucose = MAX(Glucose)");
                    var avg = dt.Compute("AVG(Glucose)", null);
                    var stdev = dt.Compute("StDev(Glucose)", null);
                    var variance = dt.Compute("Var(Glucose)", null);
                    var NumHigh = dt.Compute("Count(Glucose)", "Glucose > " + HighThreshold.ToString());
                    var NumLow = dt.Compute("Count(Glucose)", "Glucose < " + LowThreshold.ToString());

                    //populate values on the right side
                    this.lblMin.Text = Resources.Content_Strings.Label_Minimum;
                    this.MinValue.Text = min[0]["Glucose"].ToString();
                    this.lblMax.Text = Resources.Content_Strings.Label_Maximum;
                    this.MaxValue.Text = max[0]["Glucose"].ToString();
                    this.lblAvg.Text = Resources.Content_Strings.Label_Average;
                    this.AvgValue.Text = avg.ToString();
                    this.lblNumLows.Text = Resources.Content_Strings.Label_NumberOfLows;
                    this.NumLowsValue.Text = NumLow.ToString();
                    this.lblLowExplanation.Text = string.Format(Resources.Content_Strings.Label_LowExplanation, LowThreshold.ToString());
                    this.lblNumHighs.Text = Resources.Content_Strings.Label_NumberOfHighs;
                    this.NumHighsValue.Text = NumHigh.ToString();
                    this.lblHighExplanation.Text = string.Format(Resources.Content_Strings.Label_HighExplanation, HighThreshold.ToString());

                    //TODO: Send data to ReadingPiePercents class to figure percentages per time range
                    //TODO: Let user set time ranges for morning afternoon and night
                }
            }
        }

        protected void Enable3dGraphs_CheckedChanged(object sender, EventArgs e)
        {
            chtLastXDays.ChartAreas["LastXDays_ChartArea"].Area3DStyle.Enable3D = Enable3dGraphs.Checked;
        }

        protected void gridValues_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Image image = (Image)e.Row.FindControl("imgIndicator");
            string glucose = (image != null) ? image.AlternateText : string.Empty;
            
            int num;
            if (Int32.TryParse(glucose, out num))
            {
                //glucose number was converted successfully so set image indicator
                if (num > HighThreshold)
                {
                    //high
                    image.ImageUrl = Resources.Images.arrow_high_path;
                }
                else if (num < LowThreshold)
                {
                    //low
                    image.ImageUrl = Resources.Images.arrow_low_path;
                }
                else
                {
                    //normal
                    image.ImageUrl = string.Empty;
                }
            }
        }
    }

    public class ReadingPiePercents
    {
        public double PercentLow { get; set; }
        public double PercentNormal { get; set; }
        public double PercentHigh { get; set; }

        public ReadingPiePercents()
        {
            PercentLow = 0;
            PercentNormal = 0;
            PercentHigh = 0;
        }

        private void CalculatePieChart(Queries.sp_GetDataForLastXDaysDataTable dt, DateTime Start, DateTime Stop)
        {
            //TODO: write calculation for figuring percentage of numbers that were low, normal, high
        }
    }
}