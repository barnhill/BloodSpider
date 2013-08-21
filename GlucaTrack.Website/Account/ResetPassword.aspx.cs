using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GlucaTrack.Website.Properties;
using GlucaTrack.Website.Resources;

namespace GlucaTrack.Website.Account
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetResources();
            SetVisibleControls(false);
        }
        
        private void SetResources()
        {
            string spaces = "&nbsp;&nbsp;&nbsp;&nbsp;";
            spaces = Server.HtmlDecode(spaces);

            this.lblEmail.Text = Account_Strings.Label_EmailAddress;
            this.btnSendEmail.Text = spaces + Account_Strings.Button_SendEmail + spaces;
            this.lblEmailSent.Text = Account_Strings.Label_EmailSentSuccessfully;

            this.RegularExpressionEmailFormat.ErrorMessage = Account_Strings.Label_EmailFormat_Error;
        }

        private void SetVisibleControls(bool value)
        {
            foreach (Control c in upAfterSend.Controls)
                c.Visible = value;

            foreach (Control c in upBeforeSend.Controls)
                c.Visible = !value;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            using (QueriesTableAdapters.sp_DoesUserEmailExistTableAdapter ta = new QueriesTableAdapters.sp_DoesUserEmailExistTableAdapter())
            {
                using (Queries.sp_DoesUserEmailExistDataTable dt = new Queries.sp_DoesUserEmailExistDataTable())
                {
                    ta.Fill(dt, this.txtEmail.Text.Trim());

                    if (dt.Rows.Count <= 0)
                    {
                        //invalid email
                        ErrorMessage.Text = Account_Strings.Label_EmailInvalid_Error;
                    }
                    else
                    {
                        //valid email entered
                        ErrorMessage.Text = string.Empty;
                        string encrypted_userid = GlucaTrack.Services.Common.StringCipher.Encrypt(dt.Rows[0]["login"].ToString().Trim());
                        
                        if (SendEmail(encrypted_userid))
                            SetVisibleControls(true);
                    }
                }
            }
        }

        private bool SendEmail(string encrypted_userid)
        {
            try
            {
                SmtpClient smtp = new SmtpClient(Settings.Default.Email_SMTPServer, int.Parse(Settings.Default.Email_SMTPPort.Trim()));

                NetworkCredential netCred = new NetworkCredential();
                netCred.UserName = Settings.Default.Email_SMTPLogin;
                netCred.Password = Settings.Default.Email_SMTPPassword;

                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.To.Add(this.txtEmail.Text.Trim());
                message.Subject = Account_Strings.Email_ResetPasswordSubject;
                message.From = new MailAddress(Settings.Default.Email_SMTPSenderAddress);
                message.Body = Account_Strings.Email_ResetPasswordBody;
                message.Body += GenerateResetLink(encrypted_userid);

                smtp.Credentials = netCred;
                smtp.Send(message);
            }
            catch { return false; }

            return true;
        }

        private string GenerateResetLink(string encrypted_userid)
        {
            string ResetLink = "<a href='#'>#</a>";

            string webpath = "https://www.glucatrack.com/Account/ResetPassword2.aspx?u=" + encrypted_userid;

            return ResetLink.Replace("#", webpath);
        }
    }
}