using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace GlucaTrack.Website
{
    public partial class DateRangePicker : System.Web.UI.UserControl
    {
        private int rangeType;
        private string validationGroup;
        private DateTime fromDate, toDate;

        [Browsable(true)]
        [Bindable(true, BindingDirection.TwoWay)]
        public DateTime FromDate
        {
            get
            {
                int.TryParse(DateRangeList.SelectedValue, out rangeType);
                switch (rangeType)
                {
                    //Today
                    case 0:
                        fromDate = DateTime.Now.Date;
                        break;
                    //Yesterday
                    case 1:
                        fromDate = DateTime.Now.AddDays(-1).Date;
                        break;
                    //Last7Days
                    case 2:
                        fromDate = DateTime.Now.AddDays(-7).Date;
                        break;
                    //Last14Days
                    case 3:
                        fromDate = DateTime.Now.AddDays(-14).Date;
                        break;
                    //Last30Days
                    case 4:
                        fromDate = DateTime.Now.AddDays(-30).Date;
                        break;
                    //Last60Days
                    case 5:
                        fromDate = DateTime.Now.AddDays(-60).Date;
                        break;
                    //Period
                    case 6:
                        DateTime.TryParse(FromDateTextBox.Text, out fromDate);
                        break;
                    default:
                        fromDate = DateTime.Now.Date;
                        break;
                }
                return fromDate;
            }
            set
            {
                fromDate = value;
                FromDateTextBox.Text = fromDate.ToShortDateString();
                SetSessionVariables();
            }
        }

        [Browsable(true)]
        [Bindable(true, BindingDirection.TwoWay)]
        public DateTime ToDate
        {
            get
            {
                int.TryParse(DateRangeList.SelectedValue, out rangeType);
                switch (rangeType)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        toDate = DateTime.Now.Date;
                        break;
                    case 6:
                        DateTime.TryParse(ToDateTextBox.Text, out toDate);
                        break;
                    default:
                        toDate = DateTime.Now.Date;
                        break;
                }
                return toDate;
            }
            set
            {
                toDate = value;
                ToDateTextBox.Text = toDate.ToShortDateString();
                SetSessionVariables();
            }
        }

        public int DaysInRange
        {
            get 
            { 
                return Math.Abs((ToDate - FromDate).Days); 
            }
        }

        public string ValidationGroup
        {
            get
            {
                validationGroup = FromDateMaskedEditValidator.ValidationGroup;
                return validationGroup;
            }
            set
            {
                validationGroup = value;
                ToDateMaskedEditValidator.ValidationGroup = validationGroup;
                FromDateMaskedEditValidator.ValidationGroup = validationGroup;
            }
        }

        [DefaultValue(false)]
        public bool IsValidEmpty
        {
            get
            {
                return FromDateMaskedEditValidator.IsValidEmpty;
            }
            set
            {
                FromDateMaskedEditValidator.IsValidEmpty = value;
                ToDateMaskedEditValidator.IsValidEmpty = value;
            }
        }

        public void SetSessionVariables()
        {
            if (Session["RangeToDate"] != null)
                Session["RangeToDate"] = ToDate;
            else
                Session.Add("RangeToDate", ToDate);

            if (Session["RangeFromDate"] != null)
                Session["RangeFromDate"] = FromDate;
            else
                Session.Add("RangeFromDate", FromDate);

            if (Session["RangeDays"] != null)
                Session["RangeDays"] = DaysInRange;
            else
                Session.Add("RangeDays", DaysInRange);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FromDateMaskedEditValidator.ValidationGroup = validationGroup;
            ToDateMaskedEditValidator.ValidationGroup = validationGroup;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
        }
        protected override object SaveControlState()
        {
            return fromDate;
        }

        protected override void LoadControlState(object savedState)
        {
            FromDate = Convert.ToDateTime(savedState);
        }

        protected void DateRangeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetSessionVariables();
        }

        protected void FromDateTextBox_TextChanged(object sender, EventArgs e)
        {
            SetSessionVariables();
        }

        protected void ToDateTextBox_TextChanged(object sender, EventArgs e)
        {
            SetSessionVariables();
        }
    }
}