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
                    List<Committee> Committees = context.Committees.Select(c => c).ToList();
                    return Committees;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.CommitteeHelper.GetAllCommittee()");
                return null;
            }
        }


        public static int UpdateCommitteeAttendant(int CommitteeID, int DefaultAttendant, int SessionID, int AttendantStatus,DateTime CommitteeDate)
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
        }

        public static List<List<DefaultAttendant>> GetSessionCommiteeAttendance(long CommitteeID, long SessionID)
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
    }
}