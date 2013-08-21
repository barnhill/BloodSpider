using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GlucaTrack.Website.Account
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetResources();
        }

        private void SetResources()
        {
            //labels
            this.lblUsername.Text = Resources.Account_Strings.Label_Username;
            this.lblEmail.Text = Resources.Account_Strings.Label_EmailAddress;
            this.lblPassword.Text = Resources.Account_Strings.Label_Password;
            this.lblConfirmPassword.Text = Resources.Account_Strings.Label_ConfirmPassword;
            this.lblMessageInfo.Text = Resources.Account_Strings.Label_RegistrationMessageInfo;

            //header
            this.Page.Title = Resources.Account_Strings.Header_RegistrationStep1;

            //errors
            this.RequiredFieldUsername.Text = Resources.Account_Strings.Label_Username_RequiredError;
            this.RequiredFieldEmail.Text = Resources.Account_Strings.Label_EmailAddress_RequiredError;
            this.RequiredFieldPassword.Text = Resources.Account_Strings.Label_Password_RequiredError;
            this.RequiredFieldConfirmPassword.Text = Resources.Account_Strings.Label_ConfirmPassword_RequiredError;
            this.RegularExpressionEmailFormat.Text = Resources.Account_Strings.Label_EmailFormat_Error;
            this.RegularExpressionPasswordLength.Text = Resources.Account_Strings.Label_PasswordLength_Error;
            this.ComparePassword.Text = Resources.Account_Strings.Label_PasswordCompare_Error;

            //buttons
            string spaces = "&nbsp;&nbsp;&nbsp;&nbsp;";
            spaces = Server.HtmlDecode(spaces);
            this.btnContinue.Text = spaces + Resources.Account_Strings.Button_Continue + spaces;
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                Session.Add("login", UserName.Text.Trim().ToLowerInvariant());
                Session.Add("password", Password.Text.Trim());
                Session.Add("email", Email.Text.Trim().ToLowerInvariant());

                Response.Redirect("Register2.aspx");
            }
        }

        protected bool IsUsernameValid()
        {
            if (string.IsNullOrEmpty(UserName.Text.Trim()))
            {
                //invalid username due to it being empty or null
                return false;
            }
            else
            {
                using (QueriesTableAdapters.sp_DoesLoginExistTableAdapter ta = new QueriesTableAdapters.sp_DoesLoginExistTableAdapter())
                {
                    using (Queries.sp_DoesLoginExistDataTable dt = new Queries.sp_DoesLoginExistDataTable())
                    {
                        ta.Fill(dt, UserName.Text.Trim().ToLowerInvariant());
                        return (((int)dt.Rows[0]["Count"]) == 0);
                    }
                }
            }
        }

        protected void UserName_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UserName.Text.Trim()))
            {
                UserAvailability.InnerHtml = "<image src='/Images/icons/ko-red.png' class='AvailabilityImage'/>" + Resources.Account_Strings.Username_Blank + "<span></span>";
            }
            else
            {
                if (!IsUsernameValid())
                {
                    UserAvailability.InnerHtml = "<image src='/Images/icons/ko-red.png' class='AvailabilityImage'/><span>" + Resources.Account_Strings.Username_NotAvailable + "</span>";
                }
                else
                {
                    UserAvailability.InnerHtml = "<image src='/Images/icons/ok-green.png' class='AvailabilityImage'/><span>" + Resources.Account_Strings.Username_Available + "</span>";
                }
            }
        }
    }
}