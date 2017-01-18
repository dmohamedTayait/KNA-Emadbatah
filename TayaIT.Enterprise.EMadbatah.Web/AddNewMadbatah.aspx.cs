using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Web;

namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class AddNewMadbatah : BasePage
    {
        public static bool checher = false;
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            bool is_test = TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatah(98, "/test/98/", "");
        }
    }
}