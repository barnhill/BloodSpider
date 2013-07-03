using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace GlucaTrack.Website.Account
{
    public partial class PersonalSettings : System.Web.UI.Page
    {
        Queries.sp_GetLoginRow LoginRow = null;

        private byte [] PendingAvatar
        {
            set
            {
                Session.Remove("PendingAvatar");

                Session.Add("PendingAvatar", value);
            }
            get 
            {
                return (Session["PendingAvatar"] != null ? (byte[])Session["PendingAvatar"] : null);
            }
        }

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

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            
            if (!IsPostBack)
            {
                SetResources();

                Populate_YearDropdown(ddBirthdate_Year);
                Populate_MonthDropdown(ddBirthdate_Month);

                //get current settings
                using (QueriesTableAdapters.sp_GetUserSettingsTableAdapter ta = new QueriesTableAdapters.sp_GetUserSettingsTableAdapter())
                {
                    using (Queries.sp_GetUserSettingsDataTable dt = new Queries.sp_GetUserSettingsDataTable())
                    {
                        ta.Fill(dt, LoginRow.user_id);

                        PopulateTimeDropdown(ddMorningStart, Convert.ToInt32(dt[0]["start_morning"]));
                        PopulateTimeDropdown(ddAfternoonStart, Convert.ToInt32(dt[0]["start_afternoon"]));
                        PopulateTimeDropdown(ddNightStart, Convert.ToInt32(dt[0]["start_night"]));

                        this.txtFirstName.Text = dt[0]["firstname"].ToString();
                        this.txtMiddleName.Text = dt[0]["middlename"].ToString();
                        this.txtLastName.Text = dt[0]["lastname"].ToString();
                        this.txtAddress1.Text = dt[0]["address1"].ToString();
                        this.txtAddress2.Text = dt[0]["address2"].ToString();
                        this.txtCity.Text = dt[0]["city"].ToString();

                        this.txtZipcode.Text = dt[0]["zipcode"].ToString();
                        this.lblLastSyncValue.Text = dt[0]["last_sync"].ToString();
                        this.lblLastWebLoginValue.Text = dt[0]["last_weblogin"].ToString();
                        this.txtEmail.Text = dt[0]["email"].ToString();
                        this.txtOtherState.Text = dt[0]["other_state"].ToString();

                        ddState.DataBind();
                        ddUserType.DataBind();
                        ddCountry.DataBind();
                        ddIncome.DataBind();
                        ddSex.DataBind();
                        ddRace.DataBind();
                        ddTimezone.DataBind();
                        ddDiabetesType.DataBind();

                        SelectInDropDown(ddState, dt[0]["state_id"].ToString().Trim());
                        SelectInDropDown(ddUserType, dt[0]["usertype_id"].ToString().Trim());
                        SelectInDropDown(ddCountry, dt[0]["country_id"].ToString().Trim());
                        SelectInDropDown(ddIncome, dt[0]["income_id"].ToString().Trim());
                        SelectInDropDown(ddSex, dt[0]["sex_id"].ToString().Trim());
                        SelectInDropDown(ddRace, dt[0]["race_id"].ToString().Trim());
                        SelectInDropDown(ddTimezone, dt[0]["timezone_id"].ToString().Trim());
                        SelectInDropDown(ddDiabetesType, dt[0]["diabetestypes_id"].ToString().Trim());

                        DateTime dtBirthDate = Convert.ToDateTime(dt[0]["birthdate"].ToString().Trim());
                        SelectInDropDown(ddBirthdate_Month, dtBirthDate.Month.ToString().Trim());
                        this.txtBirthdate_Day.Text = dtBirthDate.Day.ToString();
                        SelectInDropDown(ddBirthdate_Year, dtBirthDate.Year.ToString().Trim());
                    }
                }

                //shows the correct state/province field based on country settings (must be after population of fields)
                ShowStateSelection();

                //get user image
                using (QueriesTableAdapters.sp_GetUserImageTableAdapter ta = new QueriesTableAdapters.sp_GetUserImageTableAdapter())
                {
                    using (Queries.sp_GetUserImageDataTable dt = new Queries.sp_GetUserImageDataTable())
                    {
                        ta.Fill(dt, LoginRow.user_id);

                        if (dt.Rows.Count > 0)
                        {
                            PendingAvatar = dt[0].image;
                        }
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

            this.lblFirstName.Text = Resources.Account_Strings.Label_Firstname;
            this.lblMiddleName.Text = Resources.Account_Strings.Label_Middlename;
            this.lblLastName.Text = Resources.Account_Strings.Label_Lastname;
            this.lblAddress1.Text = Resources.Account_Strings.Label_Address1;
            this.lblAddress2.Text = Resources.Account_Strings.Label_Address2;
            this.lblCity.Text = Resources.Account_Strings.Label_City;
            this.lblState.Text = Resources.Account_Strings.Label_State;
            this.lblZipcode.Text = Resources.Account_Strings.Label_Zipcode;
            this.lblLastSyncLabel.Text = Resources.Account_Strings.Label_LastSync;
            this.lblLastWebLoginLabel.Text = Resources.Account_Strings.Label_LastWebLogin;
            this.lblUserType.Text = Resources.Account_Strings.Label_UserType;
            this.lblCountry.Text = Resources.Account_Strings.Label_Country;
            this.lblIncome.Text = Resources.Account_Strings.Label_IncomeRange;
            this.lblSex.Text = Resources.Account_Strings.Label_Sex;
            this.lblRace.Text = Resources.Account_Strings.Label_Race;
            this.lblBirthDate_Month.Text = Resources.Account_Strings.Label_Birthdate;
            this.lblEmail.Text = Resources.Account_Strings.Label_EmailAddress;
            this.lblTimezone.Text = Resources.Account_Strings.Label_Timezone;
            this.lblDiabetesType.Text = Resources.Account_Strings.Label_DiabetesType;

            this.btnSavePersonalSettings.Text = Resources.Account_Strings.Button_SavePersonalSettings;
        }

        protected void btnSavePersonalSettings_Click(object sender, EventArgs e)
        {
            using (QueriesTableAdapters.QueriesTableAdapter qta = new QueriesTableAdapters.QueriesTableAdapter())
            {
                DateTime dtBirthdate = DateTime.Today.Date;
                try
                {
                    dtBirthdate = new DateTime(Convert.ToInt32(ddBirthdate_Year.SelectedValue), Convert.ToInt32(ddBirthdate_Month.SelectedValue), Convert.ToInt32(txtBirthdate_Day.Text.Trim()));
                }
                catch (ArgumentOutOfRangeException)
                {
                    return;
                }

                qta.sp_UpdateUserSettings(LoginRow.user_id,
                                          Convert.ToByte(((TextBox)this.fvLowNormal.Row.FindControl("LowNormal")).Text), 
                                          Convert.ToByte(((TextBox)this.fvHighNormal.Row.FindControl("HighNormal")).Text),
                                          Convert.ToByte(ddMorningStart.SelectedValue),
                                          Convert.ToByte(ddAfternoonStart.SelectedValue),
                                          Convert.ToByte(ddNightStart.SelectedValue),
                                          PendingAvatar,
                                          txtFirstName.Text.Trim(),
                                          txtMiddleName.Text.Trim(),
                                          txtLastName.Text.Trim(),
                                          txtAddress1.Text.Trim(),
                                          txtAddress2.Text.Trim(),
                                          txtCity.Text.Trim(),
                                          Convert.ToInt16(ddState.SelectedValue),
                                          txtZipcode.Text.Trim(),
                                          Convert.ToByte(ddUserType.SelectedValue),
                                          Convert.ToInt32(ddCountry.SelectedValue),
                                          Convert.ToInt32(ddIncome.SelectedValue),
                                          Convert.ToInt16(ddSex.SelectedValue),
                                          Convert.ToInt16(ddRace.SelectedValue),
                                          dtBirthdate,
                                          txtEmail.Text.Trim(),
                                          txtOtherState.Text.Trim(),
                                          Convert.ToInt16(ddTimezone.SelectedValue),
                                          Convert.ToInt16(ddDiabetesType.SelectedValue));
            }
            
            Session.Remove("PendingAvatar");

            if (Session["OnSave"] != null && Session["OnSave"].ToString().Trim() != null && Session["OnSave"].ToString().Trim() != string.Empty)
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

        protected void AsyncFileUpload2_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            AjaxControlToolkit.AsyncFileUpload fileUpload = sender as AjaxControlToolkit.AsyncFileUpload;

            if (fileUpload.HasFile)
            {
                using (System.Drawing.Image img = ScaleImage(System.Drawing.Image.FromStream(fileUpload.FileContent), 64, 64))
                {
                    PendingAvatar = Statics.imageToByteArray(img);
                }
            }
        }

        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new System.Drawing.Bitmap(newWidth, newHeight);
            System.Drawing.Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        private void SelectInDropDown(DropDownList ddTarget, string value)
        {
            foreach (ListItem li in ddTarget.Items)
            {
                if (li.Value == value)
                {
                    li.Selected = true;
                    break;
                }
            }
        }

        private void Populate_YearDropdown(DropDownList ddTarget)
        {
            for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 120; i--)
            {
                ddTarget.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

        private void Populate_MonthDropdown(DropDownList ddTarget)
        {
            ddTarget.Items.Clear();
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_1, "1"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_2, "2"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_3, "3"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_4, "4"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_5, "5"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_6, "6"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_7, "7"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_8, "8"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_9, "9"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_10, "10"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_11, "11"));
            ddTarget.Items.Add(new ListItem(Resources.Account_Strings.Month_12, "12"));
        }

        private void ShowStateSelection()
        {
            ListItem US = ddCountry.Items.FindByText("United States");

            ddState.Visible = US.Selected;
            txtOtherState.Visible = !US.Selected;
        }

        protected void Country_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowStateSelection();
        }
    }
}