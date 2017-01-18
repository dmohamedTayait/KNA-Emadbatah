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

           TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatah(137, "/test/137/", "");
            // string mad = TayaIT.Enterprise.EMadbatah.BLL.SessionStartFacade.GetAutomaticSessionStartText(155);

            string htmlText = "<span>test span</span> <span>test span2</span> test betweeen line<p procedure-id>test div</p>test between line2<p procedure-id>test div2</p>";
            Regex regex = new Regex(@"<p procedure-id[^>]*?>(.*?)</p>", RegexOptions.IgnorePatternWhitespace);
            string pattern = @"<p procedure-id[^>]*?>(.*?)</p>";

            MatchCollection matches = Regex.Matches(htmlText, pattern);
            // replace all html tags (and consequtive whitespaces) by spaces
            // trim the first and last space

            string[] resultText = create(htmlText);//regex.Split(htmlText);

            string s = "s";
        }

        public string [] create(string str)
        {
            string pattern = @"<p procedure-id[^>]*?>(.*?)</p>";
            MatchCollection matches = Regex.Matches(str, pattern);

            for (int i = 0; i < matches.Count; i++)
            {
                str = str.Replace(matches[i].ToString(), "<%#####%>");
            }

            string[] sep = new string[1] { "<%#####%>" };
            return str.Split(sep, StringSplitOptions.RemoveEmptyEntries);

        }
    }
}