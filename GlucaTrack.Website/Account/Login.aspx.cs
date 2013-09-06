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
        protected void Page_Load(object sender, EventArgs e)
        {
            this.RegisterHyperLink.NavigateUrl = "Register.aspx";
            this.linkResetPassword.NavigateUrl = "~/Account/ResetPassword.aspx";

            SetResources();

            LoginContainer.Authenticate += LoginContainer_Authenticate;
            LoginContainer.DestinationPageUrl = "~/Content/Main.aspx";

            if (!Page.IsPostBack)
            {
                try
                {
                    ReadRememberMeCookie();
                }
                catch { }
            }
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
            this.lblResetPassword.Text = Resources.Account_Strings.Label_ResetPassword;
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

                    //set auth cookie
                    if (LoginContainer.RememberMeSet)
                    {
                        HttpCookie cookie = new HttpCookie(GlucaTrack.Services.Common.Statics.AuthenticationCookie);
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, StringCipher.Encrypt(this.LoginContainer.UserName, true), DateTime.Now, DateTime.Now.AddDays(30), true, string.Empty);
                        cookie.Value = FormsAuthentication.Encrypt(authTicket);
                        cookie.Expires = authTicket.Expiration;
                        Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie(GlucaTrack.Services.Common.Statics.AuthenticationCookie);
                        cookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(cookie);
                    }

                    //update last_weblogin datetime
                    client.UpdateLastWebLogin(common);
                }
                catch (Exception ex)
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
            HttpCookie authCookie = Request.Cookies[GlucaTrack.Services.Common.Statics.AuthenticationCookie];

            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (!authTicket.Expired)
                {
                    this.LoginContainer.UserName = GlucaTrack.Services.Common.StringCipher.Decrypt(authTicket.Name, true);
                    this.LoginContainer.RememberMeSet = true;
                }
                else
                {
                    //expired
                    LoginContainer.UserName = string.Empty;
                    LoginContainer.RememberMeSet = false;
                    Session.Clear();
                    FormsAuthentication.SignOut();
                }
            }
        }
    }
}