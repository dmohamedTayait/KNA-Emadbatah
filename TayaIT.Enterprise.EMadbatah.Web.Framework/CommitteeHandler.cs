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
                        if (CommitteeID != null && SessionID != null && SessionCommitteeName != null && SessionCommitteeCreatedAt != null && SessionCommitteeDetails != null &&
                            long.TryParse(CommitteeID, out commId) && long.TryParse(SessionID, out sId) && DateTime.TryParse(SessionCommitteeCreatedAt, out scommcreatedAt))
                        {
                            SessionCommittee sessionComm = new SessionCommittee();
                            sessionComm.CommitteeID = commId;
                            sessionComm.SessionID = sId;
                            sessionComm.CommitteeName = SessionCommitteeName;
                            sessionComm.CreatedAt = scommcreatedAt;
                            sessionComm.AddedDetails = SessionCommitteeDetails;
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
                          
                            DefaultAttendant att = new DefaultAttendant();
                            foreach (CommitteeAttendant commAtt in commAttlst)
                            {
                                DefaultAttendant tempAtt = DefaultAttendantHelper.GetAttendantById((long)commAtt.DefaultAttendantID);
                                att.ID = tempAtt.ID;
                                att.Name = tempAtt.Name;
                                att.JobTitle = tempAtt.JobTitle;
                                att.Type = tempAtt.Type;
                                att.ShortName = tempAtt.ShortName;
                                att.LongName = tempAtt.LongName;
                                att.AttendantTitle = tempAtt.AttendantTitle;
                                att.Status = tempAtt.Status;
                                defAttlst.Add(att);
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(defAttlst);
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
