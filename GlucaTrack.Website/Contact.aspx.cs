using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GlucaTrack.Website
{
    public partial class Contact : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lblMessageStatus.Visible = false;
            SetResources();
        }

        private void SetResources()
        {
            this.lblEmail.Text = Resources.Master_Strings.Label_Email;
            this.lblMessage.Text = Resources.Master_Strings.Label_Message;
            this.btnSendEmail.Text = Resources.Master_Strings.Button_SendEmail;
        }

        protected void btnSendEmail_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnSendEmail.Visible = false;
                bool Submit = true;

                if (this.txtEmailAddress.Text.Trim() == "" || !this.txtEmailAddress.Text.Trim().Contains('@') || !this.txtEmailAddress.Text.Trim().Contains('.'))
                {
                    this.txtEmailAddress.BackColor = System.Drawing.Color.Salmon;
                    Submit = false;
                }//else if
                if (this.txtMessage.Text.Trim() == "")
                {
                    this.txtMessage.BackColor = System.Drawing.Color.Salmon;
                    Submit = false;
                }//else if

                if (Submit)
                {
                    MailMessage msg = new MailMessage(this.txtEmailAddress.Text.Trim(), ConfigurationManager.AppSettings["WebmasterEmail"].Trim(), "[GLUCATRACK WEBSITE]", this.txtMessage.Text.Trim());
                    msg.IsBodyHtml = false;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Send(msg);

                    //message was successfully sent
                    this.lblMessageStatus.Visible = true;
                    this.lblMessageStatus.Text = Resources.Master_Strings.Label_MessageStatus_Success;
                }//if
            }
            catch (Exception)
            {
                //message failed to send
                this.lblMessageStatus.Visible = true;
                this.lblMessageStatus.Text = Resources.Master_Strings.Label_MessageStatus_Fail;
            }//catch
        }//try
    }
}