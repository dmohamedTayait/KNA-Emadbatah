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
    public partial class ManageCommittees : BasePage
    {
        public string CommitteeTitle = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.Role != Model.UserRole.Admin)
                Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);
            if (!Page.IsPostBack)
            {
                gvbind();

            }
        }

        protected void gvbind()
        {
            EMadbatahEntities context = new EMadbatahEntities();
            List<Committee> CommitteeLst = CommitteeHelper.GetAllCommittee();
            if (CommitteeLst.Count == 0)
            {
                CommitteeLst = new List<Committee>();
                Committee procObj = new Committee();
                procObj.CommitteeName = "";
                CommitteeLst.Add(procObj);
                gvCommittees.DataSource = CommitteeLst;
                gvCommittees.DataBind();
                int columncount = gvCommittees.Rows[0].Cells.Count;
                gvCommittees.Rows[0].Cells.Clear();
                gvCommittees.Rows[0].Cells.Add(new TableCell());
                gvCommittees.Rows[0].Cells[0].ColumnSpan = columncount;
                gvCommittees.Rows[0].Cells[0].Text = "لا يوجد لجان تحت هذه القائمة";
            }
            else
            {
                gvCommittees.DataSource = CommitteeLst;
                gvCommittees.DataBind();
            }
        }

        protected void gvCommittees_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCommittees.EditIndex = e.NewEditIndex;
            gvbind();
        }

        protected void gvCommittees_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            long CommitteeID = Convert.ToInt64(gvCommittees.DataKeys[e.RowIndex].Value.ToString());
            GridViewRow row = (GridViewRow)gvCommittees.Rows[e.RowIndex];

            string textCommitteeName = ((TextBox)gvCommittees.Rows[e.RowIndex].FindControl("textCommitteeName")).Text;
            CommitteeHelper.UpdateCommitteeById(CommitteeID, textCommitteeName);

            gvCommittees.EditIndex = -1;
            gvbind();

        }

        protected void gvCommittees_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCommittees.EditIndex = -1;
            gvbind();
        }

        protected void gvCommittees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label textCommitteeStatus = (Label)e.Row.FindControl("textCommitteeStatus");

                LinkButton lnkRemove = (LinkButton)e.Row.FindControl("lnkRemove");
                LinkButton lnkUnRemove = (LinkButton)e.Row.FindControl("lnkUnRemove");

                Label lblCommitteeName = (Label)e.Row.FindControl("lblCommitteeName");
                LinkButton lnkCommitteeName = (LinkButton)e.Row.FindControl("lnkCommitteeName");

                DataControlFieldCell lbkBtn = (DataControlFieldCell)e.Row.Controls[0]; //here use the cell no in which your edit command button is there.

                if (textCommitteeStatus.Text == "1")
                {
                    lnkRemove.Visible = true;
                    lnkUnRemove.Visible = false;

                    if (lnkCommitteeName != null)
                        lnkCommitteeName.Visible = true;
                    if (lblCommitteeName != null)
                        lblCommitteeName.Visible = false;

                    lbkBtn.Enabled = true;//write a logic to disable or enable according to privilages. true;
                }
                else
                {
                    lnkRemove.Visible = false;
                    lnkUnRemove.Visible = true;

                    if (lnkCommitteeName != null)
                        lnkCommitteeName.Visible = false;
                    if (lblCommitteeName != null)
                        lblCommitteeName.Visible = true;

                    lbkBtn.Enabled = false;//write a logic to disable or enable according to privilages.
                }

            }
        }

        protected void gvCommittees_RowDeleting(object sender, EventArgs e)
        {
            LinkButton lnkRemove = (LinkButton)sender;
            long CommitteeID = long.Parse(lnkRemove.CommandArgument);//Convert.ToInt64(gvCommittees.DataKeys[e.RowIndex].Value.ToString());
            CommitteeHelper.UpdateCommitteeStatusById(CommitteeID,(int)Model.AttendantStatus.Deleted);
            gvbind();

        }

        protected void gvCommittees_RowUnDeleting(object sender, EventArgs e)
        {
            LinkButton lnkRemove = (LinkButton)sender;
            long CommitteeID = long.Parse(lnkRemove.CommandArgument);//Convert.ToInt64(gvCommittees.DataKeys[e.RowIndex].Value.ToString());
            CommitteeHelper.UpdateCommitteeStatusById(CommitteeID, (int)Model.AttendantStatus.Active);
            gvbind();

        }

        protected void AddNewCommittee(object sender, EventArgs e)
        {
            string textCommitteeName = ((TextBox)gvCommittees.FooterRow.FindControl("textCommitteeName")).Text;

            //Add new Committee
            Committee CommitteeObj = new Committee();
            CommitteeObj.CommitteeName = textCommitteeName;
            CommitteeObj.Status = (int)Model.AttendantStatus.Active;
            CommitteeHelper.AddCommittee(CommitteeObj);

            gvbind();
        }

    }
}