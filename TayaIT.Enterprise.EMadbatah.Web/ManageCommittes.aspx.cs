using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Model;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Config;

namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class ManageCommittes : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.Role != Model.UserRole.Admin)
                Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);

            UserId.Value = CurrentUser.ID.ToString();
            if (!Page.IsPostBack)
            {
                List<Committee> Committees = CommitteeHelper.GetAllCommittee();
                StringBuilder sb = new StringBuilder();
                foreach (Committee comm in Committees)
                {
                    string committeeRow = Application[Constants.HTMLTemplateFileNames.CommitteeRow].ToString();
                    committeeRow = committeeRow.Replace("<%CommitteeID%>", comm.ID.ToString() + "");
                    committeeRow = committeeRow.Replace("<%CommitteeName%>", comm.CommitteeName);
                    committeeRow = committeeRow.Replace("<%Show%>", "<div class=\"editbuttons\"><a href=\"#\" class=\"edit\">[تعديل]</a> <a class=\"remove\" href=\"#inline2\">[حذف]</a></div>");
                    sb.Append(committeeRow);
                }

                db_committees.InnerHtml = sb.ToString();
            }
        }
    }
}