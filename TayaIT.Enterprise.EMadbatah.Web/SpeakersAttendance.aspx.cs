using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.DAL;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Config;
using TayaIT.Enterprise.EMadbatah.Model;
using System.IO;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Localization;
using TayaIT.Enterprise.EMadbatah.Util;


namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class SpeakersAttendance : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                EMadbatahEntities ee = new EMadbatahEntities();
                List<Session> Sessions = ee.Sessions.Where(c => c.SessionStatusID !=(int)Model.SessionStatus.FinalApproved).Select(a => a).OrderByDescending(aa => aa.ID).ToList();

                ListItem liNew = new ListItem("-- اختر --", "0");
                ddlSessions.Items.Insert(0, liNew);
                foreach(Session sessionObj in Sessions)
                {
                    liNew = new ListItem( "( " + sessionObj.EParliamentID.ToString() + " / " + sessionObj.Type + " )",sessionObj.ID.ToString());
                    if (SessionID != null)
                    {
                        if (sessionObj.ID == long.Parse(SessionID))
                            liNew.Selected = true;
                    }
                    ddlSessions.Items.Add(liNew);
                }
              

                if (SessionID != null)
                {
                    ddlSessions.SelectedValue = SessionID;
                    Session SessionSelected = SessionHelper.GetSessionByID(long.Parse(SessionID));

                    ddlTimes.Items.Clear();

                    //بدات في موعدها
                    if (SessionSelected.SessionStartFlag == 1)
                    {
                        ddlTimes.Items.Insert(0, new ListItem("النصاب الأول", "1"));
                        ddlTimes.Items[0].Selected = true;
                        ddlTimes.Style.Add("display", "block");
                        List<Attendant> AttendantInTimes = AttendantHelper.GetAttendantInSession(long.Parse(SessionID), 1);
                        GVAttendants.DataSource = AttendantInTimes;
                        GVAttendants.DataBind();
                    }
                    else
                    {
                        ddlTimes.Style.Add("display", "block");
                        ddlTimes.Items.Insert(0, new ListItem("النصاب الأول", "1"));
                        ddlTimes.Items.Insert(1, new ListItem("النصاب الثانى", "0"));
                        ddlTimes.Items[0].Selected = true;
                        int selectedtime = int.Parse(ddlTimes.SelectedValue);
                        if (selectedtime == 1)
                        {
                            List<Attendant> AttendantInTimes = AttendantHelper.GetAttendantInSession(long.Parse(SessionID), 1);

                            GVAttendants.DataSource = AttendantInTimes;
                            GVAttendants.DataBind();
                        }
                        else
                        {
                            List<Attendant> AttendantAfterTimes = AttendantHelper.GetAttendantInSession(long.Parse(SessionID), 0);
                            GVAttendants.DataSource = AttendantAfterTimes;
                            GVAttendants.DataBind();
                        }
                    }
                    btnSave.Style.Add("display", "");
                }
                else
                {
                    ddlTimes.Style.Add("display", "none");
                }
            }
        }

        protected void ddlSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblInfo1.Visible = false;
            lblInfo2.Visible = false;
            EMadbatahEntities ee = new EMadbatahEntities();
            int SessionID = int.Parse(ddlSessions.SelectedValue);
            Session SessionSelected = ee.Sessions.Where(aaaa => aaaa.ID == SessionID).Select(a => a).FirstOrDefault();

            ddlTimes.Items.Clear();

            //بدات في موعدها
            if (SessionSelected.SessionStartFlag == 1)
            {
                ddlTimes.Style.Add("display", "block");
                ddlTimes.Items.Insert(0, new ListItem("النصاب الأول", "1"));
                ddlTimes.Items[0].Selected = true;
                List<Attendant> AttendantInTimes = AttendantHelper.GetAttendantInSession(SessionID, 1); //ee.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == 1 && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).ToList();
                GVAttendants.DataSource = AttendantInTimes;
                GVAttendants.DataBind();
            }
            else
            {
                ddlTimes.Style.Add("display", "block");
                ddlTimes.Items.Insert(0, new ListItem("النصاب الأول", "1"));
                ddlTimes.Items.Insert(1, new ListItem("النصاب الثانى", "0"));
                ddlTimes.Items[0].Selected = true;
                int selectedtime = int.Parse(ddlTimes.SelectedValue);
                if (selectedtime == 1)
                {
                    List<Attendant> AttendantInTimes = AttendantHelper.GetAttendantInSession(SessionID, 1); // ee.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == 1 && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).ToList();
                    GVAttendants.DataSource = AttendantInTimes;
                    GVAttendants.DataBind();
                }
                else
                {
                    List<Attendant> AttendantAfterTimes = AttendantHelper.GetAttendantInSession(SessionID, 0);//ee.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == 0 && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).ToList();
                    GVAttendants.DataSource = AttendantAfterTimes;
                    GVAttendants.DataBind();
                }
            }
            btnSave.Style.Add("display", "");
        }

        protected void ddlTimes_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblInfo1.Visible = false;
            lblInfo2.Visible = false;
            EMadbatahEntities ee = new EMadbatahEntities();
            int SessionID = int.Parse(ddlSessions.SelectedValue);
            Session SessionSelected = ee.Sessions.Where(aaaa => aaaa.ID == SessionID).Select(a => a).FirstOrDefault();

            //بدات في موعدها
            if (SessionSelected.SessionStartFlag == 1)
            {
                List<Attendant> AttendantInTimes = AttendantHelper.GetAttendantInSession(SessionID, 1);// ee.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == 1 && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).ToList();
                GVAttendants.DataSource = AttendantInTimes;
                GVAttendants.DataBind();
            }
            else
            {
                int selectedtime = int.Parse(ddlTimes.SelectedValue);
                if (selectedtime == 1)
                {
                    List<Attendant> AttendantInTimes = AttendantHelper.GetAttendantInSession(SessionID, 1);// ee.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == 1 && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).ToList();
                    GVAttendants.DataSource = AttendantInTimes;
                    GVAttendants.DataBind();
                }
                else
                {
                    List<Attendant> AttendantAfterTimes = AttendantHelper.GetAttendantInSession(SessionID, 0);// ee.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == 0 && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).ToList();
                    GVAttendants.DataSource = AttendantAfterTimes;
                    GVAttendants.DataBind();
                }
            }
            btnSave.Style.Add("display", "");
        }

        protected void GVAttendants_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int SessionID = int.Parse(ddlSessions.SelectedValue);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Int64 AttendantID = (Int64)DataBinder.Eval(e.Row.DataItem, "ID");

                EMadbatahEntities ee = new EMadbatahEntities();

                int selectedtime = int.Parse(ddlTimes.SelectedValue);

                List<Attendant> attendant = ee.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == selectedtime && ww.ID == AttendantID && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).ToList();

                RadioButtonList rb = (RadioButtonList)e.Row.FindControl("RBLAttendantStates");
                if (attendant.Count != 0)
                {
                    if (rb.Items.FindByValue(attendant[0].State.ToString()) != null)
                    {
                        rb.Items.FindByValue(attendant[0].State.ToString()).Selected = true;
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblInfo1.Text = "يتم الان حفظ بياناتك برجاء الاتنظار";
            lblInfo1.Visible = true;
            lblInfo2.Text = "يتم الان حفظ بياناتك برجاء الاتنظار";
            lblInfo2.Visible = true;
            foreach (GridViewRow item in GVAttendants.Rows)
            {

                int SessionID = int.Parse(ddlSessions.SelectedValue);
                int SelectedTime = int.Parse(ddlTimes.SelectedValue);
                HiddenField HFID = item.Cells[0].FindControl("HFID") as HiddenField;

                int DefaultAttendantId = int.Parse(HFID.Value);

                DateTime? AttendDate = null;

                RadioButtonList rdlist = item.Cells[2].FindControl("RBLAttendantStates") as RadioButtonList;

                int AttendantStatus = 0;
                if (rdlist.SelectedItem != null)
                {
                    AttendantStatus = int.Parse(rdlist.SelectedValue);
                }

                if (AttendantStatus != 0)
                {
                    bool resul = AttendantHelper.UpdateAttendantState(AttendantStatus, DefaultAttendantId, AttendDate);
                }

            }
            lblInfo1.Text = "تم الحفظ بنجاح";
            lblInfo1.Visible = true;
            lblInfo2.Text = "تم الحفظ بنجاح";
            lblInfo2.Visible = true;
        }

        protected void testHide(object sender, GridViewRowEventArgs e)
        {
            EMadbatahEntities ee = new EMadbatahEntities();
            int SessionID = int.Parse(ddlSessions.SelectedValue);
            Session SessionSelected = ee.Sessions.Where(aaaa => aaaa.ID == SessionID).Select(a => a).FirstOrDefault();
            //  ddlTimes.Items[0].Selected = true;
            int ddd = int.Parse(ddlTimes.SelectedValue);
            //بدات في موعدها
        }
    }
}