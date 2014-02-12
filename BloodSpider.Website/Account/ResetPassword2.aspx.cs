using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BloodSpider.Services.Common;
using BloodSpider.Website.Properties;
using BloodSpider.Website.Resources;

namespace BloodSpider.Website.Account
{
    public partial class ResetPassword2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string encoded = Request.QueryString["u"];
            if (encoded != null && CheckInput(encoded))
            {
                SetResources();
                SetVisibleControls(false);
            }
            else
            {
                System.Threading.Thread.Sleep(2000);
                Response.Redirect("/");
            }
        }

        private void SetResources()
        {
            string spaces = "&nbsp;&nbsp;&nbsp;&nbsp;";
            spaces = Server.HtmlDecode(spaces);

            this.lblMessageInfo.Text = Resources.Account_Strings.Label_RegistrationMessageInfo;
            this.lblPassword.Text = Account_Strings.Label_NewPassword;
            this.lblConfirmPassword.Text = Account_Strings.Label_ConfirmPassword;
            this.btnChangePassword.Text = spaces + Account_Strings.Button_ChangePassword + spaces;
            this.lblPasswordChanged.Text = Account_Strings.Label_PasswordChangedSuccessfully;

            this.RegularExpressionPasswordLength.ErrorMessage = Resources.Account_Strings.Label_PasswordLength_Error;
            this.ComparePassword.ErrorMessage = Resources.Account_Strings.Label_PasswordCompare_Error;
        }

        private bool CheckInput(string encoded)
        {
            try
            {
                using (QueriesTableAdapters.sp_DoesLoginExistTableAdapter ta = new QueriesTableAdapters.sp_DoesLoginExistTableAdapter())
                {
                    using (Queries.sp_DoesLoginExistDataTable dt = new Queries.sp_DoesLoginExistDataTable())
                    {
                        ta.Fill(dt, StringCipher.Decrypt(encoded));
                        return dt.Rows.Count > 0;
                    }
                }
            }
            catch { return false; }
        }

        private void SetVisibleControls(bool value)
        {
            foreach (Control c in upAfterSend.Controls)
                c.Visible = value;

            foreach (Control c in upBeforeSend.Controls)
                c.Visible = !value;
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                //passwords match and page is valid so save the new password
                using (QueriesTableAdapters.QueriesTableAdapter qta = new QueriesTableAdapters.QueriesTableAdapter())
                {
                    qta.sp_ChangePasswordByLogin(StringCipher.Decrypt(Request.QueryString["u"]), StringCipher.Encrypt(this.txtConfirmPassword.Text.Trim(), true));
                    SetVisibleControls(true);
                }
            }
        }
    }
}