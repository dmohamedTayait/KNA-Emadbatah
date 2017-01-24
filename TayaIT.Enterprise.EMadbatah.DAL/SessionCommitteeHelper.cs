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
                    List<SessionCommitteeAttendant> lst = context.SessionCommitteeAttendants.Where(c => c.SessionCommitteeID == scomm_id).ToList();
                    foreach (SessionCommitteeAttendant sCommAtt in lst)
                    {
                        context.DeleteObject(sCommAtt);
                    }
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

        public static long SaveSessionCommitteeAttendance(SessionCommitteeAttendant scommAtt)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    SessionCommitteeAttendant SessionCommitteeAtt = context.SessionCommitteeAttendants.Where(c => c.SessionCommitteeID == scommAtt.SessionCommitteeID && c.DefaultAttendantID == scommAtt.DefaultAttendantID).Select(c => c).FirstOrDefault();
                    if (SessionCommitteeAtt != null)
                    {
                        if (scommAtt.Status == 0)
                        {
                            context.DeleteObject(SessionCommitteeAtt);
                        }
                        else
                        {
                            SessionCommitteeAtt.Status = scommAtt.Status;
                        }
                    }
                    else
                    {
                        if (scommAtt.Status != 0)
                        {
                            context.SessionCommitteeAttendants.AddObject(scommAtt);
                        }
                    }
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionCommitteeHelper.AddSessionCommittee(sessionCommObj)");
                return 0;
            }
        }

        public static List<SessionCommitteeAttendant> GetSessionCommitteeAttendance(long scommAttID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<SessionCommitteeAttendant> SessionCommitteeAttLst = context.SessionCommitteeAttendants.Where(c => c.SessionCommitteeID == scommAttID).Select(c => c).ToList();
                    return SessionCommitteeAttLst;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionCommitteeHelper.AddSessionCommittee(sessionCommObj)");
                return null;
            }
        }
    }
}