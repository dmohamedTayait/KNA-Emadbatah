using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.DAL;

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


             /*   ddlSessions.DataSource = Sessions;
                ddlSessions.DataValueField = "ID";
                ddlSessions.DataTextField = "EParliamentID";
                ddlSessions.DataBind();*/

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
                        ddlTimes.Items.Insert(0, new ListItem("اكتمل النصاب القانونى فى الموعد الأول", "1"));
                        ddlTimes.Items[0].Selected = true;
                        ddlTimes.Style.Add("display", "block");
                        List<Attendant> AttendantInTimes = AttendantHelper.GetAttendantInSession(long.Parse(SessionID), 1);
                        GVAttendants.DataSource = AttendantInTimes;
                        GVAttendants.DataBind();
                    }
                    else
                    {
                        ddlTimes.Style.Add("display", "block");
                        ddlTimes.Items.Insert(0, new ListItem("اكتمل النصاب القانونى فى الموعد الأول", "1"));
                        ddlTimes.Items.Insert(1, new ListItem("لم يكتمل النصاب القانونى فى الموعد الأول", "0"));
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

            EMadbatahEntities ee = new EMadbatahEntities();
            int SessionID = int.Parse(ddlSessions.SelectedValue);
            Session SessionSelected = ee.Sessions.Where(aaaa => aaaa.ID == SessionID).Select(a => a).FirstOrDefault();

            ddlTimes.Items.Clear();

            //بدات في موعدها
            if (SessionSelected.SessionStartFlag == 1)
            {
                ddlTimes.Style.Add("display", "block");
                ddlTimes.Items.Insert(0, new ListItem("اكتمل النصاب القانونى فى الموعد الأول", "1"));
                ddlTimes.Items[0].Selected = true;
                List<Attendant> AttendantInTimes = AttendantHelper.GetAttendantInSession(SessionID, 1); //ee.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == 1 && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).ToList();
                GVAttendants.DataSource = AttendantInTimes;
                GVAttendants.DataBind();
            }
            else
            {
                ddlTimes.Style.Add("display", "block");
                ddlTimes.Items.Insert(0, new ListItem("اكتمل النصاب القانونى فى الموعد الأول", "1"));
                ddlTimes.Items.Insert(1, new ListItem("لم يكتمل النصاب القانونى فى الموعد الأول", "0"));
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
            foreach (GridViewRow item in GVAttendants.Rows)
            {

                int SessionID = int.Parse(ddlSessions.SelectedValue);
                int SelectedTime = int.Parse(ddlTimes.SelectedValue);
                HiddenField HFID = item.Cells[0].FindControl("HFID") as HiddenField;

                int DefaultAttendantId = int.Parse(HFID.Value);

                DateTime? AttendDate = null;

                if (Request.Form[item.FindControl("txtDOB").UniqueID] != "" && Request.Form[item.FindControl("txtDOB").UniqueID] != null)
                {
                    AttendDate = DateTime.Parse(Request.Form[item.FindControl("txtDOB").UniqueID]);
                }
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
        }

        // Write Data in file
        protected void btnSaveToFile_Click(object sender, EventArgs e)
        {
            int SessionID = int.Parse(ddlSessions.SelectedValue);
            EMadbatahEntities ee = new EMadbatahEntities();

            List<Attendant> attendantsinsession = ee.Attendants.Select(aa => aa).Where(rr => rr.Sessions.Any(aaaa => aaaa.ID == SessionID)).OrderBy(qq => qq.ID).ToList();

            Session s = ee.Sessions.Select(aa => aa).Where(qq => qq.ID == SessionID).FirstOrDefault();

            // بدات في موعدها
            // هنكتب مره واحده بس الفايل
            if (s.SessionStartFlag == 1)
            {
                string datatofile = "الساده الحضور اثناء فتح الجلسه في موعدها" + Environment.NewLine;
                for (int i = 0; i < attendantsinsession.Count; i++)
                {
                    if (attendantsinsession[i].AttendantState != null)
                    {
                        datatofile += attendantsinsession[i].ID + "    " + attendantsinsession[i].Name + "    " + attendantsinsession[i].AttendantState.Name + Environment.NewLine;
                    }
                    else
                    {
                        datatofile += attendantsinsession[i].ID + "    " + attendantsinsession[i].Name + "    " + "No Status" + Environment.NewLine;
                    }
                }

                using (StreamWriter _testData = new StreamWriter(Server.MapPath("~/data.txt"), true))
                {
                    _testData.WriteLine(datatofile); // Write the file.
                }

            }

             // مبداتش في موعدها
            // هنكتب مرتين في الفايل    
            else
            {
                List<Attendant> attendantsinsessioninfirst = attendantsinsession.Select(aa => aa).Where(rr => rr.SessionAttendantType == 1).OrderBy(qq => qq.ID).ToList();

                string datatofile = "الساده الحضور اثناء فتح الجلسه في موعدها" + Environment.NewLine;

                for (int i = 0; i < attendantsinsessioninfirst.Count; i++)
                {
                    if (attendantsinsessioninfirst[i].AttendantState != null)
                    {
                        datatofile += attendantsinsessioninfirst[i].ID + "    " + attendantsinsessioninfirst[i].Name + "    " + attendantsinsessioninfirst[i].AttendantState.Name + Environment.NewLine;
                    }
                    else
                    {
                        datatofile += attendantsinsessioninfirst[i].ID + "    " + attendantsinsessioninfirst[i].Name + "    " + "No Status" + Environment.NewLine;
                    }
                }

                datatofile += Environment.NewLine + "الساده الحضور بعد فتح الجلسه " + Environment.NewLine;
                attendantsinsessioninfirst = attendantsinsession.Select(aa => aa).Where(rr => rr.SessionAttendantType == 0).OrderBy(qq => qq.ID).ToList();

                for (int i = 0; i < attendantsinsessioninfirst.Count; i++)
                {
                    if (attendantsinsessioninfirst[i].AttendantState != null)
                    {
                        datatofile += attendantsinsessioninfirst[i].ID + "    " + attendantsinsessioninfirst[i].Name + "    " + attendantsinsessioninfirst[i].AttendantState.Name + Environment.NewLine;
                    }
                    else
                    {
                        datatofile += attendantsinsessioninfirst[i].ID + "    " + attendantsinsessioninfirst[i].Name + "    " + "No Status" + Environment.NewLine;
                    }
                }
                using (StreamWriter _testData = new StreamWriter(Server.MapPath("~/data.txt"), true))
                {
                    _testData.WriteLine(datatofile);
                }
            }
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