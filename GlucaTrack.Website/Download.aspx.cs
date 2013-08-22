using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GlucaTrack.Website
{
    public partial class Download : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (QueriesTableAdapters.sp_GetApplicationVersionsTableAdapter ta = new QueriesTableAdapters.sp_GetApplicationVersionsTableAdapter())
            {
                using (Queries.sp_GetApplicationVersionsDataTable dt = new Queries.sp_GetApplicationVersionsDataTable())
                {
                    ta.Fill(dt);
                    PopulateVersionButtons(dt);
                }
            }
        }

        private void PopulateVersionButtons(Queries.sp_GetApplicationVersionsDataTable dt)
        {
            if (dt.Rows.Count <= 0)
                return;

            Queries.sp_GetApplicationVersionsRow drow = dt.Where(i => i.Name.Contains("Windows Service")).FirstOrDefault();

            lblWinVersion.Text = "Version: " + drow.version;
        }
    }
}