using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GlucaTrack.Website.Account
{
    public partial class PersonalSettings : System.Web.UI.Page
    {
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

            if (!IsPostBack)
            {
                SetResources();

                //get current settings
                using (QueriesTableAdapters.sp_GetUserSettingsTableAdapter ta = new QueriesTableAdapters.sp_GetUserSettingsTableAdapter())
                {
                    using (Queries.sp_GetUserSettingsDataTable dt = new Queries.sp_GetUserSettingsDataTable())
                    {
                        ta.Fill(dt, LoginRow.user_id);

                        PopulateTimeDropdown(ddMorningStart, Convert.ToInt32(dt[0]["start_morning"]));
                        PopulateTimeDropdown(ddAfternoonStart, Convert.ToInt32(dt[0]["start_afternoon"]));
                        PopulateTimeDropdown(ddNightStart, Convert.ToInt32(dt[0]["start_night"]));
                    }
                }
            }
        }

        private void SetResources()
        {
            this.lblLowNormal.Text = Resources.Account_Strings.Label_LowNormal;
            this.lblHighNormal.Text = Resources.Account_Strings.Label_HighNormal;
            this.lblMorningStart.Text = Resources.Account_Strings.Label_StartMorning;
            this.lblAfternoonStart.Text = Resources.Account_Strings.Label_StartAfternoon;
            this.lblNightStart.Text = Resources.Account_Strings.Label_StartNight;

            this.btnSavePersonalSettings.Text = Resources.Account_Strings.Button_SavePersonalSettings;
        }

        protected void btnSavePersonalSettings_Click(object sender, EventArgs e)
        {
            using (QueriesTableAdapters.QueriesTableAdapter qta = new QueriesTableAdapters.QueriesTableAdapter())
            {
                qta.sp_UpdateUserSettings(LoginRow.user_id,
                                              Convert.ToByte(((TextBox)this.fvLowNormal.Row.FindControl("LowNormal")).Text), 
                                              Convert.ToByte(((TextBox)this.fvHighNormal.Row.FindControl("HighNormal")).Text),
                                              Convert.ToByte(ddMorningStart.SelectedValue),
                                              Convert.ToByte(ddAfternoonStart.SelectedValue),
                                              Convert.ToByte(ddNightStart.SelectedValue));
            }

            if (Session["OnSave"].ToString().Trim() != null && Session["OnSave"].ToString().Trim() != string.Empty)
            {
                string target = Session["OnSave"].ToString();
                Session.Remove("OnSave");
                Response.Redirect(target);
            }
        }

        private void PopulateTimeDropdown(DropDownList ddTarget, int currentHour)
        {
            ddTarget.Items.Clear();

            for (int i = 0; i <= 24; i++)
            {
                ListItem li = new ListItem((i < 10 ? "0" : string.Empty) + i.ToString() + ":00", i.ToString());
                li.Selected = (currentHour == i) ? true : false;
                ddTarget.Items.Add(li);
            }
        }
    }
}