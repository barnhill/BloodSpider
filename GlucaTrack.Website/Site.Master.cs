using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GlucaTrack.Website
{
    public partial class SiteMaster : MasterPage
    {
        //TODO: recognize logged in user and hide login/register buttons and show welcome message and signout
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;
        private Queries.sp_GetLoginRow LoginRow = null;

        public static void setSecureProtocol(bool bSecure)
        {
            string redirectUrl = null;
            HttpContext context = HttpContext.Current;

            // if we want HTTPS and it is currently HTTP
            if (bSecure && !context.Request.IsSecureConnection)
            {
                redirectUrl = context.Request.Url.ToString().Replace("http:", "https:");
            }
            else
            {
                // if we want HTTP and it is currently HTTPS
                if (!bSecure && context.Request.IsSecureConnection)
                    redirectUrl = context.Request.Url.ToString().Replace("https:", "http:");
            }

            // in all other cases we don't need to redirect
            // check if we need to redirect, and if so use redirectUrl to do the job
            if (redirectUrl != null)
            {
                context.Response.Redirect(redirectUrl);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
#if (!DEBUG)
            setSecureProtocol(true);
#endif

            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        private void SetResources()
        {
            //login
            this.RegisterButton.Text = Resources.Master_Strings.Label_Register;
            this.LogoutButton.Text = Resources.Master_Strings.Label_Logout;

            //footer
            this.lblCopyright.Text = Resources.Master_Strings.Label_Copyright;

            //navigation
            this.lblHome.Text = Resources.Master_Strings.Label_Home;
            this.lblAbout.Text = Resources.Master_Strings.Label_About;
            this.lblContact.Text = Resources.Master_Strings.Label_Contact;
            this.lblDownload.Text = Resources.Master_Strings.Label_Download;

            //labels
            if (LoginRow != null)
            {
                this.WelcomeMessage.Text = string.Format(Resources.Master_Strings.Label_WelcomeMessage);
                this.linkPersonalSettings.Text = LoginRow.firstname;
                this.lblExclamation.Text = Resources.Master_Strings.Label_Exclamation;
            }

        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }

            if (Session["LoggedInUser"] != null)
            {
                LoginRow = (Queries.sp_GetLoginRow)Session["LoggedInUser"];
            }
            else
            {
                LoginRow = null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetResources();

            LoggedIn.Visible = LoginRow != null;
            NotLoggedIn.Visible = LoginRow == null;
        }

        protected void linkPersonalSettings_Click(object sender, EventArgs e)
        {
            Session.Remove("OnSave");

            Session.Add("OnSave", Request.ServerVariables["URL"]);
            Response.Redirect("../Account/PersonalSettings.aspx");
        }
    }
}