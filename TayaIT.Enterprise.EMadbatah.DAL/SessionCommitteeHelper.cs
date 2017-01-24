using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Util;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public class SessionCommitteeHelper
    {
        public static List<SessionCommittee> GetSessionCommitteeBySessionIDAndCommitteeID(long sessionID, long committeeID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<SessionCommittee> SessionCommittees = context.SessionCommittees.Where(c => c.SessionID == sessionID && c.CommitteeID == committeeID).Select(c => c).ToList();
                    return SessionCommittees;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionCommitteeHelper.GetSessionCommitteeBySessionIDAndCommitteeID(" + sessionID.ToString() + "," + committeeID.ToString() + ")");
                return null;
            }
        }

        public static long AddSessionCommittee(SessionCommittee sessionCommObj)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    context.SessionCommittees.AddObject(sessionCommObj);
                    int result = context.SaveChanges();
                    return sessionCommObj.ID;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionCommitteeHelper.AddSessionCommittee(sessionCommObj)");
                return 0;
            }
        }

        public static int DeleteSessionCommittee(long scomm_id)
        {
            try
            {
                SessionCommittee commForDelete = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    commForDelete = context.SessionCommittees.FirstOrDefault(c => c.ID == scomm_id);
                    context.DeleteObject(commForDelete);
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.DeleteSessionCommittee(" + scomm_id.ToString() + ")");
                return 0;
            }
        }

        public int SaveSessionCommitteeAttendance(long attID,long scommId,long status)
        {
            return 0;
        }
    }
}