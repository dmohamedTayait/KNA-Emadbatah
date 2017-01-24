using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.Model.VecSys;
using TayaIT.Enterprise.EMadbatah.Vecsys;
using TayaIT.Enterprise.EMadbatah.BLL;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Util.Web;
using TayaIT.Enterprise.EMadbatah.Config;
using System.Text;
namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class ManageCommitteeAttendance : BasePage
    {
        public string CommitteeName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.Role != Model.UserRole.Admin)
                Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);
            if (!Page.IsPostBack)
            {
                chbbind();
                bindAttvalues();
            }
        }

        protected void chbbind()
        {
            if (!string.IsNullOrEmpty(CommitteeID))
            {
                CommitteeName = CommitteeHelper.GetCommitteeByID(int.Parse(CommitteeID));
                EMadbatahEntities context = new EMadbatahEntities();
                List<DefaultAttendant> defAttLst = DefaultAttendantHelper.GetAllDefaultAttendants((int)Model.AttendantType.FromTheCouncilMembers);
                chbDefAttlst.DataSource = defAttLst;
                chbDefAttlst.DataValueField = "ID";
                chbDefAttlst.DataTextField = "LongName";
                chbDefAttlst.DataBind();
            }
        }

        protected void bindAttvalues()
        {
            List<CommitteeAttendant> commAttlst = CommitteeHelper.GetCommiteeAttendanceByCommitteeID(long.Parse(CommitteeID));
            foreach (CommitteeAttendant commAttObj in commAttlst)
            {
                chbDefAttlst.Items.FindByValue(commAttObj.DefaultAttendantID.ToString()).Selected = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            CommitteeName = CommitteeHelper.GetCommitteeByID(int.Parse(CommitteeID));
            try
            {
                foreach (ListItem chAtt in chbDefAttlst.Items)
                {
                    if (chAtt.Selected)
                        CommitteeHelper.AddCommitteeAttendant(long.Parse(CommitteeID), long.Parse(chAtt.Value));
                    else
                        CommitteeHelper.UpdateCommitteeAttendantStatus(long.Parse(CommitteeID), long.Parse(chAtt.Value), (int)Model.AttendantStatus.Deleted);
                }
                lblInfo1.Text = "تم الحفظ بنجاح";
                lblInfo1.Visible = true;
                lblInfo2.Text = "تم الحفظ بنجاح";
                lblInfo2.Visible = true;
            }
            catch (Exception ex)
            {
                lblInfo1.Text = "حدث خطأ";
                lblInfo1.Visible = true;
                lblInfo2.Text = "حدث خطأ";
                lblInfo2.Visible = true;
            }
        }
    }
}