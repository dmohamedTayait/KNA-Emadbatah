using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Script.Serialization;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.Config;
using TayaIT.Enterprise.EMadbatah.Util.Web;
using System.IO;
using System.Security.Principal;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Localization;
using System.Web.Security;

namespace TayaIT.Enterprise.EMadbatah.Web.Framework
{
    public class CommitteeHandler : BaseHandler
    {

        protected override void HandleRequest()
        {
            WebFunctions.CommitteeFunctions function;

            if (AjaxFunctionName != null && Enum.TryParse<WebFunctions.CommitteeFunctions>(AjaxFunctionName, true, out function))
            {
                string jsonStringOut = null;
                long sId = 0;
                long commId = 0;
                long scommid = 0;
                DateTime scommcreatedAt;
                long res = 0;
                switch (function)
                {
                    case WebFunctions.CommitteeFunctions.AddSessionCommittee:
                        if (CommitteeID != null && SessionID != null && SessionCommitteeCreatedAt != null &&
                            long.TryParse(CommitteeID, out commId) && long.TryParse(SessionID, out sId) && DateTime.TryParse(SessionCommitteeCreatedAt, out scommcreatedAt))
                        {
                            SessionCommittee sessionComm = new SessionCommittee();
                            sessionComm.CommitteeID = commId;
                            sessionComm.SessionID = sId;
                            sessionComm.CommitteeName = SessionCommitteeName != null ? SessionCommitteeName : "";
                            sessionComm.CreatedAt = scommcreatedAt;
                            sessionComm.AddedDetails = SessionCommitteeDetails != null ? SessionCommitteeDetails : "";
                            res = SessionCommitteeHelper.AddSessionCommittee(sessionComm);

                        }
                        jsonStringOut = SerializeObjectInJSON(res);
                        break;
                    case WebFunctions.CommitteeFunctions.DeleteSessionCommittee:
                        if (SessionCommitteeID != null && long.TryParse(SessionCommitteeID, out scommid))
                        {
                            res = SessionCommitteeHelper.DeleteSessionCommittee(scommid);
                        }
                        jsonStringOut = SerializeObjectInJSON(res);
                        break;
                    case WebFunctions.CommitteeFunctions.GetSessionCommAtt:
                        List<DefaultAttendant> defAttlst = new List<DefaultAttendant>();
                        if (CommitteeID != null && long.TryParse(CommitteeID, out commId))
                        {
                            List<CommitteeAttendant> commAttlst = CommitteeHelper.GetCommiteeAttendanceByCommitteeID(long.Parse(CommitteeID));
                            List<SessionCommitteeAttendant> scommAttLst = SessionCommitteeHelper.GetSessionCommitteeAttendance(long.Parse(SessionCommitteeID));
                            DefaultAttendant att = new DefaultAttendant();
                            foreach (CommitteeAttendant commAtt in commAttlst)
                            {

                                DefaultAttendant tempAtt = DefaultAttendantHelper.GetAttendantById((long)commAtt.DefaultAttendantID);

                                SessionCommitteeAttendant tt = scommAttLst.FirstOrDefault(c => c.DefaultAttendantID == (long)commAtt.DefaultAttendantID && c.SessionCommitteeID == long.Parse(SessionCommitteeID));

                                int tmpStatus = 0;
                                if (tt != null)
                                    tmpStatus = (int)tt.Status;
                                att = new DefaultAttendant();
                                att.ID = tempAtt.ID;
                                att.Name = tempAtt.Name;
                                att.JobTitle = tempAtt.JobTitle;
                                att.Type = tempAtt.Type;
                                att.ShortName = tempAtt.ShortName;
                                att.LongName = tempAtt.LongName;
                                att.AttendantTitle = tempAtt.AttendantTitle;
                                att.Status = tmpStatus;
                                att.AttendantDegree = tempAtt.AttendantDegree;
                                defAttlst.Add(att);
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(defAttlst);
                        break;
                    case WebFunctions.CommitteeFunctions.TakeSessionCommAttendance:
                        if (JsonStr != null && SessionCommitteeID != null)
                        {
                            List<SessionCommitteeAttendant> sCommAttlst = serializer.Deserialize<List<SessionCommitteeAttendant>>(JsonStr);
                            foreach (SessionCommitteeAttendant sCommAtt in sCommAttlst)
                            {
                                SessionCommitteeAttendant sCommAttTemp = new SessionCommitteeAttendant();
                                sCommAttTemp.Status = sCommAtt.Status;
                                sCommAttTemp.DefaultAttendantID = sCommAtt.DefaultAttendantID;
                                sCommAttTemp.SessionCommitteeID = long.Parse(SessionCommitteeID);
                                SessionCommitteeHelper.SaveSessionCommitteeAttendance(sCommAttTemp);
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(JsonStr);
                        break;
                }

                if (jsonStringOut != null)
                {
                    _context.Response.AddHeader("Encoding", "UTF-8");
                    _context.Response.Write(jsonStringOut);
                }
            }
            else
                return;
        }

        public string CommitteeID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.COMMITTEE_ID, _context);                
            }
        }

        public string JsonStr
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.JSON_STR, _context);
            }
        }

        public string SessionCommitteeName
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.SESSION_COMMITTEE_NAME, _context);                 
            }
        }

        public string SessionCommitteeID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.SESSION_COMMITTEE_ID, _context);
            }
        }
        public string SessionCommitteeCreatedAt
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.SESSION_COMM_CREATEDAT, _context);
            }
        }

        public string SessionCommitteeDetails
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.SESSION_COMM_ADDED_DETAILS, _context);
            }
        }
    }//end class
}//end namespace
