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
    public static class MembersVoteHelper
    {
        #region MembersVote

        public static List<MembersVote> GetMembersVoteNonSecretVoteID(int nonSecretVoteID)
        {
            try
            {
                using (EmadbatahVotingEntities context = new EmadbatahVotingEntities())
                {
                    List<MembersVote> members = new List<MembersVote>();
                    if (context.MembersVotes.Count<MembersVote>() > 0)
                    {
                        members = context.MembersVotes.Where(s => s.NonSecretVoteSubjectID == nonSecretVoteID).ToList<MembersVote>();
                    }

                    return members;
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
