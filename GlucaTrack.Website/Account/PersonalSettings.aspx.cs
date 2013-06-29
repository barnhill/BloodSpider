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

                        try
                        {
                            SelectStateInDropdown(ddState, dt[0]["state_id"].ToString().Trim());
                        }
                        catch { }
                    }
                }

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
                                              Convert.ToByte(ddNightStart.SelectedValue),
                                              PendingAvatar,
                                              txtFirstName.Text.Trim(),
                                              txtMiddleName.Text.Trim(),
                                              txtLastName.Text.Trim(),
                                              txtAddress1.Text.Trim(),
                                              txtAddress2.Text.Trim(),
                                              txtCity.Text.Trim(),
                                              Convert.ToInt16(ddState.SelectedValue));
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

        private void SelectStateInDropdown(DropDownList ddTarget, string stateid)
        {
            using (QueriesTableAdapters.sp_GetAllStates_USTableAdapter ta = new QueriesTableAdapters.sp_GetAllStates_USTableAdapter())
            {
                using (Queries.sp_GetAllStates_USDataTable dt = new Queries.sp_GetAllStates_USDataTable())
                {
                    ta.Fill(dt);

                    ddTarget.Items.Clear();

                    foreach (Queries.sp_GetAllStates_USRow state_row in dt.ToList())
                    {
                        ListItem li = new ListItem(state_row.name, state_row.state_id.ToString());
                        li.Selected = (li.Value.Trim().ToLowerInvariant() == stateid.Trim().ToLowerInvariant());
                        ddTarget.Items.Add(li);
                    }
                }
            }
        }
    }
}