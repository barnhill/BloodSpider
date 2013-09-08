using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace GlucaTrack.Website
{
    public partial class Logout : System.Web.UI.Page
    {
        GTService.Common.sp_GetLoginRow LoginRow = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                LoginRow = (GTService.Common.sp_GetLoginRow)Session["LoggedInUser"];
            }

            SetResources();
        }

        private void SetResources()
        {
            if (LoginRow != null)
                this.lblGoodbye.Text = string.Format(Resources.Account_Strings.Label_GoodbyeMessage, LoginRow.firstname);
        }

        protected void TimerRedirect_Tick(object sender, EventArgs e)
        {
            //HttpCookie authCookie = Request.Cookies[GlucaTrack.Services.Common.Statics.AuthenticationCookie];

            //if (authCookie != null)
            //{
            //    authCookie.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(authCookie);
            //}

            FormsAuthentication.SignOut();
            Session.Clear();
            Response.Redirect("/");
        }
    }
}