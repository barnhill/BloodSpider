using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GlucaTrack.Services.Common;

namespace GlucaTrack.Website.Account
{
    public partial class Register2 : System.Web.UI.Page
    {
        //TODO: Type of diabetes (Type 1 or Type 2)
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //redirect if workflow wasnt followed
                if (Session["login"] == null)
                {
                    Response.Redirect("/");
                }
                SetResources();
                ShowStateSelection();
                PopulateBirthdate_YearDropdown();
            }
        }

        private void SetResources()
        {
            //labels
            this.lblMessageInfo.Text = Resources.Account_Strings.Label_Registration2MessageInfo;
            this.lblFirstname.Text = Resources.Account_Strings.Label_Firstname;
            this.lblMiddlename.Text = Resources.Account_Strings.Label_Middlename;
            this.lblLastname.Text = Resources.Account_Strings.Label_Lastname;
            this.lblAddress1.Text = Resources.Account_Strings.Label_Address1;
            this.lblAddress2.Text = Resources.Account_Strings.Label_Address2;
            this.lblCity.Text = Resources.Account_Strings.Label_City;
            this.lblState.Text = Resources.Account_Strings.Label_State;
            this.lblZipcode.Text = Resources.Account_Strings.Label_Zipcode;
            this.lblCountry.Text = Resources.Account_Strings.Label_Country;
            this.lblSex.Text = Resources.Account_Strings.Label_Sex;
            this.lblIncome.Text = Resources.Account_Strings.Label_IncomeRange;
            this.lblRace.Text = Resources.Account_Strings.Label_Race;
            this.lblBirthDate_Month.Text = Resources.Account_Strings.Label_Birthdate;
            this.lblTimezone.Text = Resources.Account_Strings.Label_Timezone;
            this.lblDiabetesType.Text = Resources.Account_Strings.Label_DiabetesType;

            //error messages
            this.RequiredFieldFirstName.ErrorMessage = Resources.Account_Strings.Label_Firstname_RequiredError;
            this.RequiredFieldMiddleName.ErrorMessage = Resources.Account_Strings.Label_Middlename_RequiredError;
            this.RequiredFieldLastName.ErrorMessage = Resources.Account_Strings.Label_Lastname_RequiredError;
            this.RequiredFieldAddress1.ErrorMessage = Resources.Account_Strings.Label_Address1_RequiredError;
            this.RequiredFieldCity.ErrorMessage = Resources.Account_Strings.Label_City_RequiredError;
            this.RequiredFieldState.ErrorMessage = Resources.Account_Strings.Label_State_RequiredError;
            this.RequiredFieldZipcode.ErrorMessage = Resources.Account_Strings.Label_Zipcode_RequiredError;
            this.RequiredFieldCountry.ErrorMessage = Resources.Account_Strings.Label_Country_RequiredError;
            this.RequiredFieldSex.ErrorMessage = Resources.Account_Strings.Label_Sex_RequiredError;
            this.RequiredFieldIncome.ErrorMessage = Resources.Account_Strings.Label_IncomeRange_RequiredError;
            this.RequiredFieldRace.ErrorMessage = Resources.Account_Strings.Label_Race_RequiredError;
            this.RequiredFieldBirthdate_Day.ErrorMessage = Resources.Account_Strings.Label_Birthdate_RequiredError;
            this.RequiredFieldTimezone.ErrorMessage = Resources.Account_Strings.Label_Birthdate_RequiredError;
            this.RequiredFieldDiabetesType.ErrorMessage = Resources.Account_Strings.Label_DiabetesType_RequiredError;
            
            //buttons
            this.btnFinishRegistration.Text = Resources.Account_Strings.Button_FinishRegistration;

            this.Country.DataBind();
            ListItem US = Country.Items.FindByText("United States");
            if (US != null)
            {
                US.Selected = true;
            }
        }

        private void ShowStateSelection()
        {
            ListItem US = Country.Items.FindByText("United States");

            State.Visible = US.Selected;
            OtherState.Visible = !US.Selected;

            RequiredFieldState.Enabled = State.Visible;
            RequiredFieldOtherState.Enabled = OtherState.Visible;
        }

        protected void Country_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowStateSelection();
        }

        protected void btnFinishRegistration_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                using (QueriesTableAdapters.sp_DoesLoginExistTableAdapter ta = new QueriesTableAdapters.sp_DoesLoginExistTableAdapter())
                {
                    using (Queries.sp_DoesLoginExistDataTable dt = new Queries.sp_DoesLoginExistDataTable())
                    {
                        ta.Fill(dt, Session["login"].ToString().Trim().ToLowerInvariant());
                        if (((int)dt.Rows[0]["Count"]) == 0)
                        {
                            //no login of this name so clear to create a new one
                            using (QueriesTableAdapters.QueriesTableAdapter qta = new QueriesTableAdapters.QueriesTableAdapter())
                            {
                                short stateid = 0;
                                try
                                {
                                    stateid = Convert.ToInt16(State.SelectedValue);
                                }
                                catch { }

                                DateTime dtBirthdate = DateTime.Today.Date;
                                try
                                {
                                    dtBirthdate = new DateTime(Convert.ToInt32(ddBirthdate_Year.SelectedValue), Convert.ToInt32(ddBirthdate_Month.SelectedValue), Convert.ToInt32(txtBirthdate_Day.Text.Trim())); 
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    return;
                                }

                                //call stored procedure to add new user
                                qta.sp_CreateLogin(FirstName.Text.Trim(),
                                                   MiddleName.Text.Trim(),
                                                   LastName.Text.Trim(),
                                                   Address1.Text.Trim(),
                                                   Address2.Text.Trim(),
                                                   City.Text.Trim(),
                                                   stateid,
                                                   Zipcode.Text.Trim(),
                                                   Session["login"].ToString().Trim(),
                                                   StringCipher.DES_Encrypt(Session["password"].ToString().Trim()),
                                                   null,
                                                   null,
                                                   1,//usertype
                                                   Convert.ToInt32(Country.SelectedValue),
                                                   Convert.ToInt32(Income.SelectedValue),
                                                   Convert.ToInt16(Sex.SelectedValue),
                                                   Convert.ToInt16(Race.SelectedValue),
                                                   dtBirthdate,
                                                   null,
                                                   Session["email"].ToString().Trim(),
                                                   OtherState.Text.Trim(),
                                                   true,
                                                   Convert.ToInt16(Timezone.SelectedValue),
                                                   Convert.ToInt16(DiabetesType.SelectedValue));
                            }
                            
                            Response.Redirect("/Account/RegistrationComplete.aspx");
                        }
                    }
                }
            }
        }

        private void PopulateBirthdate_YearDropdown()
        {
            for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 120; i--)
            {
                this.ddBirthdate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
    }
}