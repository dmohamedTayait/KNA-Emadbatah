using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Web;
using System.Text.RegularExpressions;

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
            //bool is_test = TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatah(116, "/test/116/", "");
            bool is_test = TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatah(98, "/test/98/", "");
           /* string htmlText = "<span>test span</span> <span>test span2</span> test betweeen line<div>test div</div>test between line2<div>test div2</div>";
            Regex regex = new Regex(@"<div[^>]*?>(.*?)</div>", RegexOptions.IgnorePatternWhitespace);
            string pattern = @"<div[^>]*?>(.*?)</div>";

            MatchCollection matches = Regex.Matches(htmlText, pattern);
            // replace all html tags (and consequtive whitespaces) by spaces
            // trim the first and last space

            string[] resultText = regex.Split(htmlText);

            string s = "s";*/

        }
    }
}