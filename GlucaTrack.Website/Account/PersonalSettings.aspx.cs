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
                                              PendingAvatar);
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
    }
}