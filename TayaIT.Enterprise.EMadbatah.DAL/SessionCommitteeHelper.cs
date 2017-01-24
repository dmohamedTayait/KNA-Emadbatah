using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Util;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public class SessionCommitteeHelper
    {
        public static List<SessionCommittee> GetSessionCommitteeBySessionIDAndCommitteeID(long sessionID,long committeeID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<SessionCommittee> SessionCommittees = context.SessionCommittees.Where(c => c.SessionID == sessionID && c.SessionID == committeeID).Select(c => c).ToList();
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

        public static int UpdateCommitteeStatusById(long comm_id, int status)
        {
            try
            {
                Committee commForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    commForUpdate = context.Committees.FirstOrDefault(c => c.ID == comm_id);
                    if (commForUpdate != null)
                    {
                        commForUpdate.Status = status;
                    }

                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.UpdateCommitteeById(" + comm_id + "," + status.ToString() + ")");
                return 0;
            }
        }

        public static int UpdateCommitteeById(long comm_id, string commName)
        {
            try
            {
                Committee commForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    commForUpdate = context.Committees.FirstOrDefault(c => c.ID == comm_id);
                    if (commForUpdate != null)
                    {
                        commForUpdate.CommitteeName = commName;
                    }

                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.UpdateCommitteeById(" + comm_id + "," + commName + ")");
                return 0;
            }
        }
    }
}