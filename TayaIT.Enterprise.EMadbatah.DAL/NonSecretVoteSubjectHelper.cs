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
                using (EmadbatahVotingEntities context = new EmadbatahVotingEntities())
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
                using (EmadbatahVotingEntities context = new EmadbatahVotingEntities())
                {
                    TBLMeeting meeting = new TBLMeeting();
                    if (context.TBLMeetings.Count<TBLMeeting>() > 0)
                    {
                        string xx = eparID.ToString();
                        meeting = context.TBLMeetings.Where(s => s.MeetingNo == xx).FirstOrDefault();
                        if (meeting != null)
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

        public static TBLMeeting GetNonSecretVotesByMeetingDate(DateTime meetingDate)
        {
            try
            {
                using (EmadbatahVotingEntities context = new EmadbatahVotingEntities())
                {
                    TBLMeeting meeting = new TBLMeeting();
                    if (context.TBLMeetings.Count<TBLMeeting>() > 0)
                    {
                        meeting = context.TBLMeetings.Where(s =>EntityFunctions.TruncateTime(s.MeetingDate) == EntityFunctions.TruncateTime(meetingDate)).FirstOrDefault();
                        if (meeting != null)
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

        public static List<TBLNonSecretVoteSubject> GetNonSecretVoteSubjectsByMeetingDate(DateTime meetingDate)
        {
            try
            {
                using (EmadbatahVotingEntities context = new EmadbatahVotingEntities())
                {
                    List<TBLNonSecretVoteSubject> votes = new List<TBLNonSecretVoteSubject>();
                    if (context.TBLNonSecretVoteSubjects.Count<TBLNonSecretVoteSubject>() > 0)
                    {
                        votes = context.TBLNonSecretVoteSubjects.Where(s => EntityFunctions.TruncateTime(s.NonSecretVoteSubjectDate) == EntityFunctions.TruncateTime(meetingDate)).ToList();
                    }
                    return votes;
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
