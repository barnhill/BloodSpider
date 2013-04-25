using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GlucaTrack.Website.Account
{
    public partial class RegistrationComplete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //redirect if workflow wasnt followed
            if (Session["login"] == null)
            {
                Response.Redirect("/");
            }

            using (QueriesTableAdapters.sp_GetFirstNameFromLoginTableAdapter ta = new QueriesTableAdapters.sp_GetFirstNameFromLoginTableAdapter())
            {
                using (Queries.sp_GetFirstNameFromLoginDataTable dt = new Queries.sp_GetFirstNameFromLoginDataTable())
                {
                    ta.Fill(dt, Session["login"].ToString().Trim().ToLowerInvariant());
                    lblWelcome.Text = string.Format(Resources.Account_Strings.Label_WelcomeRegistrationComplete, dt.Rows[0]["firstname"].ToString());
                }
            }
        }

        protected void TimerRedirect_Tick(object sender, EventArgs e)
        {
            string login = Session["login"].ToString();
            Session.Clear();
            Session.Add("login", login);
            Response.Redirect("/");
        }
    }
}