using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Script.Serialization;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Config;
using System.IO;
using TayaIT.Enterprise.EMadbatah.Util.Web;
using TayaIT.Enterprise.EMadbatah.Util;
using System.Threading;
using System.Collections;
using System.Data.SqlClient;
using System.Data;


namespace TayaIT.Enterprise.EMadbatah.Web.Framework
{
    public class VotingHandler : BaseHandler
    {

        protected override void HandleRequest()
        {
            string jsonStringOut = null;
            WebFunctions.VotingFunctions function;

            if ((AjaxFunctionName != null && Enum.TryParse<WebFunctions.VotingFunctions>(AjaxFunctionName, true, out function)))
            {
                switch (function)
                {
                    case WebFunctions.VotingFunctions.GetSessionVotes:
                        List<SessionNonSecretVoteSubject> votesLst = new List<SessionNonSecretVoteSubject>();
                        if (votesLst != null)
                        {
                            TBLMeeting meeting = NonSecretVoteSubjectHelper.GetNonSecretVotesByEparID(long.Parse(SessionIDEParliment));
                            if (meeting != null && meeting.TBLNonSecretVoteSubjects != null && meeting.TBLNonSecretVoteSubjects.Count > 0)
                            {
                                foreach (TBLNonSecretVoteSubject voteSubject in meeting.TBLNonSecretVoteSubjects)
                                {
                                    SessionNonSecretVoteSubject sessionVoteSubject = new SessionNonSecretVoteSubject();
                                    sessionVoteSubject.ID = voteSubject.NonSecretVoteSubjectID;
                                    sessionVoteSubject.MeetingID = (int)voteSubject.MeetingID;
                                    sessionVoteSubject.NonSecretVoteSubjectNumber = voteSubject.NonSecretVoteSubjectNumber;
                                    sessionVoteSubject.NonSecretVoteSubject = voteSubject.NonSecretVoteSubject;
                                    sessionVoteSubject.NonSecretVoteSubjectDate = (DateTime)voteSubject.NonSecretVoteSubjectDate;
                                    sessionVoteSubject.NonSecreatVoteSubjectIsClosed = (bool)voteSubject.NonSecreatVoteSubjectIsClosed;
                                    sessionVoteSubject.NofAgree = voteSubject.NofAgree;
                                    sessionVoteSubject.NofDisagree = voteSubject.NofDisagree;
                                    sessionVoteSubject.NofNoVote = voteSubject.NofNoVote;
                                    sessionVoteSubject.NofAttendance = voteSubject.NofAttendance;
                                    votesLst.Add(sessionVoteSubject);
                                }
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(votesLst);
                        break;
                    default:
                        break;
                }
            }

            if (jsonStringOut != null)
            {
                _context.Response.AddHeader("Encoding", "UTF-8");
                _context.Response.Write(jsonStringOut);
            }
        }
    }
}
