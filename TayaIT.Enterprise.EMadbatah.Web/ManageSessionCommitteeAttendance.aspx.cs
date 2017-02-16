using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.DAL;
namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class ManageSessionCommitteeAttendance : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblInfo1.Visible = false;
            lblInfo2.Visible = false;
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
                        {
                            liNew.Selected = true;

                            MP3FolderPath.Value = sessionObj.ID.ToString();
                            DateTime time = (DateTime)sessionObj.StartTime;
                            MP3FilePath.Value = string.Format("{0}://{1}:{2}/", Request.Url.Scheme, Request.Url.Host, Request.Url.Port) + "SessionFiles/";
                            string day = time.Day.ToString();
                            string month = time.Month.ToString();
                            if (time.Day < 10)
                                day = "0" + day;
                            if (time.Month < 10)
                                month = "0" + month;

                            MP3FilePath.Value += MP3FolderPath.Value + "/speakers_" + day + "_" + month + "_" + time.Year + "_";
                            if (sessionObj.SessionStartFlag == 1)
                            {
                                MP3FilePath.Value += "01.mp3";
                            }
                            else
                            {
                                MP3FilePath.Value += "02.mp3";
                            }
                        }
                    }
                    ddlSessions.Items.Add(liNew);
                }

            }
        }

        protected void ddlSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSessions.SelectedValue))
            {
                Response.Redirect("ManageSessionCommitteeAttendance.aspx?sid=" + ddlSessions.SelectedValue);
            }
        }

    }
}