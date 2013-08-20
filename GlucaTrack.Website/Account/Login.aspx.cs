using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using GlucaTrack.Services.Common;

namespace GlucaTrack.Website.Account
{
    public partial class Login : Page
    {
        //TODO: implement 'Forgot Username/Password'
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register.aspx";
            linkResetPassword.NavigateUrl = "~/Account/ResetPassword.aspx";

            SetResources();

            //ReadRememberMeCookie();

            LoginContainer.Authenticate += LoginContainer_Authenticate;
            LoginContainer.DestinationPageUrl = "~/Content/Main.aspx";
        }

        private void SetResources()
        {
            //login control
            ((Label)LoginContainer.FindControl("lblUsername")).Text = Resources.Account_Strings.Label_Username;
            ((Label)LoginContainer.FindControl("lblPassword")).Text = Resources.Account_Strings.Label_Password;
            ((Label)LoginContainer.FindControl("lblRememberMe")).Text = Resources.Account_Strings.Label_RememberMe;
            ((Button)LoginContainer.FindControl("btnLogin")).Text = Resources.Account_Strings.Button_Login;

            //failure text
            this.LoginContainer.FailureText = Resources.Account_Strings.Login_Failure;

            //link below login control
            this.lblDontHave.Text = Resources.Account_Strings.Label_DontHaveAccount;
            this.RegisterHyperLink.Text = Resources.Account_Strings.Label_RegisterLink;
            this.linkResetPassword.Text = Resources.Account_Strings.Link_ResetPassword;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Session.Clear();   
        }

        private void LoginContainer_Authenticate(object sender, AuthenticateEventArgs e)
        {
            using (GTService.GTServiceClient client = new GTService.GTServiceClient())
            {
                try
                {
                    GTService.Common common = client.ValidateLogin(StringCipher.Encrypt("GlucaTrack.Website"), StringCipher.Encrypt("e87c87ba-a48c-4e37-b2c1-9186531afcfb"), StringCipher.Encrypt(StringCipher.Encrypt(this.LoginContainer.UserName, true)), StringCipher.Encrypt(StringCipher.Encrypt(this.LoginContainer.Password, true)));

                    //successful login
                    GTService.Common.sp_GetLoginRow loginRow = common.sp_GetLogin.First();
                    Session.Add("LoggedInUser", loginRow);

                    e.Authenticated = true;

                    //update last_weblogin datetime
                    client.UpdateLastWebLogin(common);
                }
                catch
                {
                    //unsuccessful login
                    Session.Clear();
                    FormsAuthentication.SignOut();
                    e.Authenticated = false;
                }
            }
        }

        //TODO: finish remember me logic
        private void ReadRememberMeCookie()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value) && authCookie.Expires > DateTime.Now)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                LoginContainer.UserName = authTicket.Name;
                LoginContainer.RememberMeSet = true;
            }
        }
    }
}