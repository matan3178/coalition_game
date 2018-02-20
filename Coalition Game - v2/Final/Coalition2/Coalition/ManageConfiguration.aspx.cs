using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Coalition.App_Data;

namespace Coalition
{
    public partial class ManageConfiguration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }

        protected void Unnamed2_Click(object sender, EventArgs e)
        {
         //   DAL db = DAL.GetDAL();
            DAL.AppendConfigurations(txt_configurations.Text);

        }

        protected void Unnamed3_Click(object sender, EventArgs e)
        {
          //  DAL db = DAL.GetDAL();
            DAL.WriteNewConfigurations(txt_configurations.Text);

        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
           // DAL db = DAL.GetDAL();
            txt_configurations.Text = DAL.ReadFileToString(DAL._pathRoom);
        }
    }
}