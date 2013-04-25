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
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Session.Clear();   
        }

        private void LoginContainer_Authenticate(object sender, AuthenticateEventArgs e)
        {
            using (QueriesTableAdapters.sp_GetLoginTableAdapter ta = new QueriesTableAdapters.sp_GetLoginTableAdapter())
            {
                using (Queries.sp_GetLoginDataTable dtLogin = new Queries.sp_GetLoginDataTable())
                {
                    string test = StringCipher.DES_Encrypt(this.LoginContainer.Password);
                    if (ta.Fill(dtLogin, this.LoginContainer.UserName, StringCipher.DES_Encrypt(this.LoginContainer.Password)) > 0)
                    {
                        //successful login
                        Queries.sp_GetLoginRow loginRow = dtLogin.First();
                        Session.Add("LoggedInUser", loginRow);

                        e.Authenticated = true;

                        //update last_weblogin datetime
                        using (QueriesTableAdapters.QueriesTableAdapter qta = new QueriesTableAdapters.QueriesTableAdapter())
                        {
                            qta.sp_UpdateLastWeblogin(loginRow.user_id);
                        }
                    }
                    else
                    {
                        //unsuccessful login
                        Session.Clear();
                        FormsAuthentication.SignOut();
                        e.Authenticated = false;
                    }
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