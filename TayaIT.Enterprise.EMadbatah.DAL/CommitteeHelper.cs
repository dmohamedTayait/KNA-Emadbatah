using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Util;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public class CommitteeHelper
    {
        public static List<Committee> GetAllCommittee()
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Committee> Committees = context.Committees.Where(c=> c.Status == 1).Select(c => c).ToList();
                    return Committees;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.GetAllCommittee()");
                return null;
            }
        }

        public static long AddCommittee(Committee commObj)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Committee> chkIfCommExist = context.Committees.Where(c => c.CommitteeName == commObj.CommitteeName).ToList();
                    if (chkIfCommExist.Count == 0)
                    {
                        context.Committees.AddObject(commObj);
                        int result = context.SaveChanges();
                        return commObj.ID;
                    }
                    else
                        return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.AddNewDefaultAttendant(defAttObj)");
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
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.UpdateCommitteeById(" + comm_id + "," + status .ToString()+ ")");
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

        public static string GetCommitteeByID(int ID)
        {
            string CommitteeName = "";
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Committee> allCommittees = new List<Committee>();
                    if (context.Committees.Count<Committee>() > 0)
                    {
                        CommitteeName = context.Committees.Where(c => c.ID == ID).First().CommitteeName;
                    }
                    return CommitteeName;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.GetCommitteeByID(" + ID + ")");
                return null;
            }
        }

        public static List<CommitteeAttendant> GetCommiteeAttendanceByCommitteeID(long CommitteeID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    return context.CommitteeAttendants.Where(a => a.CommitteeID == CommitteeID && a.Status == 1).Select(c => c).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.GetCommiteeAttendance(" + CommitteeID.ToString() + ")");
                return null;
            }
        }

        public static long AddCommitteeAttendant(long commID, long defAttID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<CommitteeAttendant> chkIfAttExist = context.CommitteeAttendants.Where(c => c.CommitteeID == commID && c.DefaultAttendantID == defAttID).ToList();
                    if (chkIfAttExist.Count == 0)
                    {
                        CommitteeAttendant commAtt = new CommitteeAttendant();
                        commAtt.CommitteeID = commID;
                        commAtt.DefaultAttendantID = defAttID;
                        commAtt.Status = 1;
                        context.CommitteeAttendants.AddObject(commAtt);
                        int result = context.SaveChanges();
                        return commAtt.ID;
                    }
                    else
                        return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.AddCommitteeAttendant("+ commID +" , "+ defAttID +")");
                return 0;
            }
        }

        public static long UpdateCommitteeAttendantStatus(long commID, long defAttID,int status)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    CommitteeAttendant chkIfAttExist = context.CommitteeAttendants.FirstOrDefault(c => c.CommitteeID == commID && c.DefaultAttendantID == defAttID);
                    if (chkIfAttExist != null)
                    {
                        chkIfAttExist.Status = status;
                        int result = context.SaveChanges();
                        return chkIfAttExist.ID;
                    }
                    else
                        return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.UpdateCommitteeAttendantStatus(" + commID + " , " + defAttID + " , " + status + ")");
                return 0;
            }
        }


       /*  public static List<List<DefaultAttendant>> GetSessionCommiteeAttendance(long CommitteeID, long SessionID)
        {
            List<DefaultAttendant> attAbologyLst = new List<DefaultAttendant>();
            List<DefaultAttendant> attMissionLst = new List<DefaultAttendant>();
            List<DefaultAttendant> attAbsentLst = new List<DefaultAttendant>();
            List<List<DefaultAttendant>> attLst = new List<List<DefaultAttendant>>();
            DefaultAttendant att = new DefaultAttendant();
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<CommitteeAttendant> CommitteeAttendantt = context.CommitteeAttendants.Where(a => a.CommitteeID == CommitteeID && a.SessionID == SessionID).Select(c => c).ToList();

                    foreach (CommitteeAttendant commAtt in CommitteeAttendantt)
                    {
                        if (commAtt.AttendantStatus == 2)
                        {
                            att = DefaultAttendantHelper.GetAttendantById((long)commAtt.DefaultAttendantID);
                            attAbsentLst.Add(att);
                        }
                        else if (commAtt.AttendantStatus == 3)
                        {
                            att = DefaultAttendantHelper.GetAttendantById((long)commAtt.DefaultAttendantID);
                            attAbologyLst.Add(att);
                        }
                        else if (commAtt.AttendantStatus == 4)
                        {
                            att = DefaultAttendantHelper.GetAttendantById((long)commAtt.DefaultAttendantID);
                            attMissionLst.Add(att);
                        }
                    }

                    attLst.Add(attAbsentLst);
                    attLst.Add(attAbologyLst);
                    attLst.Add(attMissionLst);
                    return attLst;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.GetCommitteeByCommitteeIDAndSessionID()");
                return null;
            }
        }

       public static int UpdateCommitteeAttendant(int CommitteeID, int DefaultAttendant, int SessionID, int AttendantStatus, DateTime CommitteeDate)
        {
            try
            {
                CommitteeAttendant CommitteeAttendantForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    CommitteeAttendantForUpdate = context.CommitteeAttendants.FirstOrDefault(a => a.CommitteeID == CommitteeID && a.DefaultAttendant.ID == DefaultAttendant && a.SessionID == SessionID);
                    if (CommitteeAttendantForUpdate != null)
                    {
                        CommitteeAttendantForUpdate.AttendantStatus = AttendantStatus;
                    }
                    else
                    {
                        CommitteeAttendantForUpdate = new CommitteeAttendant();
                        CommitteeAttendantForUpdate.CommitteeID = CommitteeID;
                        CommitteeAttendantForUpdate.DefaultAttendantID = DefaultAttendant;
                        CommitteeAttendantForUpdate.SessionID = SessionID;
                        CommitteeAttendantForUpdate.AttendantStatus = AttendantStatus;
                        CommitteeAttendantForUpdate.CommitteeDate = CommitteeDate;
                        context.CommitteeAttendants.AddObject(CommitteeAttendantForUpdate);

                    }
                    int res = context.SaveChanges();
                    return res;
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.UpdateCommitteeAttendant(" + CommitteeID + "," + DefaultAttendant + "," + SessionID + "," + AttendantStatus + ")");
                return -1;
            }
        }
        
        public static List<CommitteeAttendant> GetCommitteeByCommitteeIDAndSessionID(int CommitteeID, Int64 DefaultAttendantID, int SessionID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<CommitteeAttendant> CommitteeAttendantt = context.CommitteeAttendants.Where(a => a.CommitteeID == CommitteeID && a.DefaultAttendant.ID == DefaultAttendantID && a.SessionID == SessionID).Select(c => c).ToList();

                    return CommitteeAttendantt;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.GetCommitteeByCommitteeIDAndSessionID()");
                return null;
            }
        }*/

    }
}