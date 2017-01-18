using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Util;
using System.Data.Objects.DataClasses;
//using TayaIT.Enterprise.EMadbatah.Model;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public static class NonSecretVoteSubjectHelper
    {
        #region NonSecretVoteSubject

        public static TBLNonSecretVoteSubject GetSessionVoteByVoteID(int voteID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    TBLNonSecretVoteSubject vote = new TBLNonSecretVoteSubject();
                    if (context.TBLNonSecretVoteSubjects.Count<TBLNonSecretVoteSubject>() > 0)
                    {
                        vote = context.TBLNonSecretVoteSubjects.Where(s => s.NonSecretVoteSubjectID == voteID).First();
                    }
                    return vote;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.NonSecretVoteSubjectHelper.NonSecretVotesByEparID()");
                return null;
            }
        }

        public static TBLMeeting GetNonSecretVotesByEparID(long eparID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    TBLMeeting meeting = new TBLMeeting();
                    if (context.TBLMeetings.Count<TBLMeeting>() > 0)
                    {
                        string xx = eparID.ToString();
                        meeting = context.TBLMeetings.Where(s => s.MeetingNo == xx).First();
                        meeting.TBLNonSecretVoteSubjects.Load();
                    }
                    return meeting;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.NonSecretVoteSubjectHelper.NonSecretVotesByEparID()");
                return null;
            }
        }
        
        #endregion

    }
}
