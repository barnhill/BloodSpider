using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BloodSpider.Website
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetResources();
        }

        private void SetResources()
        {
            this.lblFirstTitle.Text = Resources.General_Strings.Label_FirstTitle;
            this.lblSecondTitle.Text = Resources.General_Strings.Label_SecondTitle;
            this.lblThirdTitle.Text = Resources.General_Strings.Label_ThirdTitle;
            this.lblFourthTitle.Text = Resources.General_Strings.Label_FourthTitle;

            this.lblFirstContent.Text = Resources.General_Strings.Label_FirstContent;
            this.lblSecondContent.Text = Resources.General_Strings.Label_SecondContent;
            this.lblThirdContent.Text = Resources.General_Strings.Label_ThirdContent;
            this.lblFourthContent.Text = Resources.General_Strings.Label_FourthContent;
        }
    }
}