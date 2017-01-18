using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.DAL;
namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class CommitteeAttendance : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                EMadbatahEntities ee = new EMadbatahEntities();
                List<Session> Sessions = ee.Sessions.Where(c => c.SessionStatusID != (int)Model.SessionStatus.FinalApproved).Select(a => a).OrderByDescending(aa => aa.ID).ToList();
                ListItem liNew = new ListItem("-- اختر --", "0");
                ddlSessions.Items.Insert(0, liNew);
                foreach (Session sessionObj in Sessions)
                {
                    liNew = new ListItem("( " + sessionObj.EParliamentID.ToString() + " / " + sessionObj.Type + " )", sessionObj.ID.ToString());
                    if (SessionID != null)
                    {
                        if (sessionObj.ID == long.Parse(SessionID))
                            liNew.Selected = true;
                    }
                    ddlSessions.Items.Add(liNew);
                }

                List<Committee> Committees = CommitteeHelper.GetAllCommittee();
                ListItem liNewCommittee = new ListItem("-- اختر --", "0");
                ddlCommittee.Items.Insert(0, liNewCommittee);
                foreach (Committee committeeObj in Committees)
                {
                    liNewCommittee = new ListItem("( " + committeeObj.CommitteeName.ToString(), committeeObj.ID.ToString());
                    ddlCommittee.Items.Add(liNewCommittee);
                }

                // Get All DefaultAttendants and bind them to GV
                List<DefaultAttendant> DefaultAttendants = DefaultAttendantHelper.GetAllDefaultAttendants();
                GVDefaultAttendants.DataSource = DefaultAttendants;
                GVDefaultAttendants.DataBind();
            }
        }


        // Action excuted when ddl changed
        protected void ddlCommittee_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<DefaultAttendant> DefaultAttendants = DefaultAttendantHelper.GetAllDefaultAttendants();
            GVDefaultAttendants.DataSource = DefaultAttendants;
            GVDefaultAttendants.DataBind();
        }


        // Update DefaultAttendants status in GV from DB 
        protected void GVDefaultAttendants_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int CommitteeSelected = int.Parse(ddlCommittee.SelectedValue);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Int64 DefaultAttendantID = (Int64)DataBinder.Eval(e.Row.DataItem, "ID");

                int SessionId = int.Parse(ddlSessions.SelectedValue);
                List<CommitteeAttendant> CommitteeAttendantt = CommitteeHelper.GetCommitteeByCommitteeIDAndSessionID(CommitteeSelected, DefaultAttendantID, SessionId);

                RadioButtonList rb = (RadioButtonList)e.Row.FindControl("RBLAttendantStates");
                if (CommitteeAttendantt.Count != 0)
                {
                    if (rb.Items.FindByValue(CommitteeAttendantt[0].AttendantStatus.ToString()) != null)
                    {
                        rb.Items.FindByValue(CommitteeAttendantt[0].AttendantStatus.ToString()).Selected = true;
                    }
                }
            }
        }


        // Update DB from userinput in GV
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int SessionId = int.Parse(ddlSessions.SelectedValue);

            foreach (GridViewRow item in GVDefaultAttendants.Rows)
            {
                int CommitteeSelected = int.Parse(ddlCommittee.SelectedValue);

                HiddenField HFID = item.Cells[0].FindControl("HFID") as HiddenField;
                int DefaultAttendantId = int.Parse(HFID.Value);

                RadioButtonList rdlist = item.Cells[2].FindControl("RBLAttendantStates") as RadioButtonList;

                int AttendantStatus = 0;
                if (rdlist.SelectedItem != null)
                {
                    AttendantStatus = int.Parse(rdlist.SelectedValue);
                }

                int result = CommitteeHelper.UpdateCommitteeAttendant(CommitteeSelected, DefaultAttendantId, SessionId, AttendantStatus);

            }
        }

        protected void ddlSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<DefaultAttendant> DefaultAttendants = DefaultAttendantHelper.GetAllDefaultAttendants();
            GVDefaultAttendants.DataSource = DefaultAttendants;
            GVDefaultAttendants.DataBind();
        }
    }
}