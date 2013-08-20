using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GlucaTrack.Website
{
    public partial class SupportedMeters : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadSupportedMetersTree();
        }

        private void LoadSupportedMetersTree()
        {
            treeSupportedMeters.Nodes.Clear();

            using (QueriesTableAdapters.sp_GetAllSupportedMetersTableAdapter ta = new QueriesTableAdapters.sp_GetAllSupportedMetersTableAdapter())
            {
                using (Queries.sp_GetAllSupportedMetersDataTable dt = new Queries.sp_GetAllSupportedMetersDataTable())
                {
                    ta.Fill(dt);
                    foreach (string manufacturer in (from DataRow dRow in dt.Rows select dRow["Manufacturer"]).Distinct())
                    {
                        //add each manufacturer
                        TreeNode manufacturerNode = new TreeNode(manufacturer);
                        foreach (DataRow meterRow in dt.Select("Manufacturer = '" + manufacturer + "'"))
                        {
                            manufacturerNode.ChildNodes.Add(new TreeNode(meterRow["Meter"].ToString()));
                        }

                        treeSupportedMeters.Nodes.Add(manufacturerNode);
                    }
                }
            }
        }
    }
}