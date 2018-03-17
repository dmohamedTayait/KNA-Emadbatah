using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Util;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public class AttendantHelper
    {
        public static bool AddNewAttendant(string attendant_name, string job_title, int eparliamentID, int typeID, int stateID, long sessionID, string fName, string sName, string tName, string attendantDegree)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Attendant attendant = new Attendant
                    {
                        Name = attendant_name,
                        JobTitle = job_title,
                        EparlimentID = eparliamentID,
                        Type = typeID,
                        State = stateID,
                        FirstName = fName,
                        SecondName = sName,
                        TribeName = tName,
                        AttendantDegree = attendantDegree
                    };
                    //context.Attendants.AddObject(attendant);

                    Session session = context.Sessions.FirstOrDefault(c => c.ID == sessionID);
                    session.Attendants.Add(attendant);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.AddNewAttendant(" + attendant_name + "," + eparliamentID + ")");
                return false;
            }
        }


        public static int DeleteAttendantByID(long attendant_id)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Attendant attendant = context.Attendants.FirstOrDefault(c => c.ID == attendant_id);
                    context.DeleteObject(attendant);
                    int res = context.SaveChanges();
                    return res;
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.DeleteAttendantByID(" + attendant_id + ")");
                return -1;
            }
        }

        public static int GenerateSessionAttendants(long sid, Session current_session, bool CBSessionStart, bool edit)
        {
            try
            {
                EMadbatahEntities context = new EMadbatahEntities();
                List<DefaultAttendant> DefaultAttendants = context.DefaultAttendants.Select(aa => aa).Where(aa => aa.Status == 1).ToList();
                Attendant chIfAddedb4 = null;
                if (CBSessionStart)
                {
                    for (int i = 0; i < DefaultAttendants.Count; i++)
                    {
                        chIfAddedb4 = null;
                        Attendant attendant = fillAttendant(DefaultAttendants[i], 1, current_session);
                        if (edit)
                            chIfAddedb4 = current_session.Attendants.FirstOrDefault(c => c.SessionAttendantType == 1 && c.DefaultAttendantID == attendant.DefaultAttendantID);

                        if (chIfAddedb4 == null)
                            AddNewSessionAttendant(attendant, sid);
                        else
                        {
                            chIfAddedb4.EparlimentID = current_session.EParliamentID;
                            context.SaveChanges();
                        }
                    }

                    if (edit)
                    {
                        List<Attendant> lstAtts = current_session.Attendants.Where(c => c.SessionAttendantType == 0).ToList();
                        foreach (Attendant att in lstAtts)
                        {
                            context.Attach(att);
                            context.DeleteObject(att);
                            context.SaveChanges();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < DefaultAttendants.Count; i++)
                    {
                        chIfAddedb4 = null;
                        Attendant attendant = fillAttendant(DefaultAttendants[i], 1, current_session);
                        if (edit)
                            chIfAddedb4 = current_session.Attendants.FirstOrDefault(c => c.SessionAttendantType == 1 && c.DefaultAttendantID == attendant.DefaultAttendantID);

                        if (chIfAddedb4 == null)
                            AddNewSessionAttendant(attendant, sid);
                        else
                        {
                            chIfAddedb4.EparlimentID = current_session.EParliamentID;
                            context.SaveChanges();
                        }
                        attendant = fillAttendant(DefaultAttendants[i], 0, current_session);

                        if (edit)
                            chIfAddedb4 = current_session.Attendants.FirstOrDefault(c => c.SessionAttendantType == 0 && c.DefaultAttendantID == attendant.DefaultAttendantID);

                        if (chIfAddedb4 == null)
                            AddNewSessionAttendant(attendant, sid);
                        else
                        {
                            chIfAddedb4.EparlimentID = current_session.EParliamentID;
                            context.SaveChanges();
                        }
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GenerateSessionAttendants()");
                return -1;
            }
        }

        public static Attendant fillAttendant(DefaultAttendant defAtt, int sessionOpenStatus, Session sessionObj)
        {
            Attendant attendant = new Attendant();
            attendant.Name = defAtt.Name;
            attendant.JobTitle = defAtt.JobTitle;
            attendant.DefaultAttendantID = defAtt.ID;
            attendant.SessionAttendantType = sessionOpenStatus;//0
            attendant.EparlimentID = sessionObj.EParliamentID;
            attendant.Type = defAtt.Type;
            attendant.AttendantTitle = defAtt.AttendantTitle;
            attendant.OrderByAttendantType = defAtt.OrderByAttendantType;
            attendant.AttendantAvatar = defAtt.AttendantAvatar;
            attendant.State = 1;
            attendant.ShortName = defAtt.ShortName;
            attendant.LongName = defAtt.LongName;
            attendant.CreatedAt = defAtt.CreatedAt;
            attendant.AttendantDegree = defAtt.AttendantDegree;
            return attendant;
        }

        public static int DeleteAttendantByName(string attendant_name)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Attendant attendant = context.Attendants.FirstOrDefault(c => c.Name == attendant_name);
                    context.DeleteObject(attendant);
                    int res = context.SaveChanges();
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.DeleteAttendantByName(" + attendant_name + ")");
                return -1;
            }
        }

        public static int UpdateAttendant(long attendant_id, string attendant_name, string job_title, int typeID, int stateID)
        {
            try
            {
                Attendant updated_attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    updated_attendant = context.Attendants.FirstOrDefault(c => c.ID == attendant_id || c.Name == attendant_name);
                    if (updated_attendant != null)
                    {
                        //update the Attendant attributes
                        updated_attendant.Name = attendant_name == null ? updated_attendant.Name : attendant_name;
                        updated_attendant.JobTitle = job_title == null ? updated_attendant.JobTitle : job_title;
                        updated_attendant.Type = typeID == null ? updated_attendant.Type : typeID;
                        updated_attendant.State = stateID == null ? updated_attendant.State : stateID;
                    }
                    int res = context.SaveChanges();
                    return res;
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.UpdateAttendant(" + attendant_name + "," + job_title + ")");
                return -1;
            }
        }

        public static Attendant GetAttendantById(long attendant_id)
        {
            try
            {
                Attendant attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendant = context.Attendants.FirstOrDefault(c => c.ID == attendant_id);
                }
                return attendant;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantById(" + attendant_id + ")");
                return null;
            }
        }

        public static Attendant GetAttendantByDefaultAttendantId(long attendant_id)
        {
            try
            {
                Attendant attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendant = context.Attendants.FirstOrDefault(c => c.DefaultAttendantID == attendant_id);
                }
                return attendant;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantById(" + attendant_id + ")");
                return null;
            }
        }

        public static Attendant GetAttendantByDefaultAttendantIdAndEparID(long attendant_id,long eparID)
        {
            try
            {
                Attendant attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendant = context.Attendants.FirstOrDefault(c => c.DefaultAttendantID == attendant_id && c.EparlimentID == eparID);
                }
                return attendant;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantByDefaultAttendantIdAndEparID(" + attendant_id + "," + eparID + ")");
                return null;
            }
        }

        public static long GetUnknownAttendantId(long sessionID)
        {
            try
            {

                Attendant attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendant = context.Sessions.FirstOrDefault(c => c.ID == sessionID).Attendants.FirstOrDefault(c => c.Name == "غير معرف");
                    if (attendant != null)
                        return attendant.ID;
                    else
                    {
                        attendant = new Attendant
                        {
                            Name = "غير معرف",
                            JobTitle = "",
                            EparlimentID = -1,
                            Type = 7,
                            State = 1
                        };
                        //context.Attendants.AddObject(attendant);

                        Session session = context.Sessions.FirstOrDefault(c => c.ID == sessionID);
                        session.Attendants.Add(attendant);
                        context.SaveChanges();
                        return attendant.ID;

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetUnknownAttendantId(" + sessionID + ")");
                return -1;
            }
        }

        public static List<Attendant> GetAttendantsBySessionId(long sessionid)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Session s = context.Sessions.FirstOrDefault(c => c.ID == sessionid);
                    if (s != null)
                        return s.Attendants.ToList<Attendant>();

                }
                return null;

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantsBySessionId(" + sessionid + ")");
                return null;
            }
        }


        public static List<Attendant> GetAttendants(long sessionID, long agendaitemID, long subagendaid, bool isAllSpeakers)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Attendant> attendants = new List<Attendant>();
                    List<SessionContentItem> scis = null;

                    if (!isAllSpeakers && (agendaitemID < 0 || subagendaid < 0))
                    {
                        attendants = (from att in context.Sessions.FirstOrDefault(s => s.ID == sessionID).Attendants
                                      where att.State != null && att.State == 1
                                      select att).ToList<Attendant>();
                        return attendants;
                    }

                    if (subagendaid > 0)
                        scis = context.AgendaSubItems.FirstOrDefault(c => c.ID == subagendaid).SessionContentItems.ToList<SessionContentItem>();
                    else if (agendaitemID > 0)
                        scis = context.AgendaItems.FirstOrDefault(c => c.ID == agendaitemID).SessionContentItems.ToList<SessionContentItem>();

                    if (scis != null)
                    {
                        foreach (SessionContentItem sci in scis)
                        {
                            if (!attendants.Contains(sci.Attendant))
                            {
                                if (isAllSpeakers)
                                    attendants.Add(sci.Attendant);
                                else if (sci.Attendant.State != null && sci.Attendant.State.Value == 1)
                                    attendants.Add(sci.Attendant);
                            }
                        }
                        return attendants;
                    }


                }
                return null;

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantsByAgendaItemID(" + agendaitemID + ")");
                return null;
            }
        }


        public static Attendant GetAttendantByName(string attendant_name)
        {
            try
            {
                Attendant attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendant = context.Attendants.FirstOrDefault(c => c.Name == attendant_name);
                }
                return attendant;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantByName(" + attendant_name + ")");
                return null;
            }
        }

        public static bool AddNewSessionAttendant(Attendant attendant, long SessionIDCreated)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {

                    context.Attendants.AddObject(attendant);
                    Session sss = context.Sessions.FirstOrDefault(s => s.ID == SessionIDCreated);
                    attendant.Sessions.Add(sss);
                    int resu = context.SaveChanges();
                    long AttendantAdded = attendant.ID;
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.AddNewAttendant()");
                return false;
            }
        }

        public static bool AddNewSessionAttendant(Attendant newAttendant, long SessionID, out long AttendantID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Attendant attendant = context.Sessions.FirstOrDefault(c => c.ID == SessionID).Attendants.FirstOrDefault(c => c.Name == newAttendant.LongName);
                    if (attendant != null)
                        AttendantID = attendant.ID;
                    else
                    {
                        Session session = context.Sessions.FirstOrDefault(c => c.ID == SessionID);
                        session.Attendants.Add(newAttendant);
                        context.SaveChanges();
                        AttendantID = newAttendant.ID;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                AttendantID = 0;
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetUnknownAttendantId(" + SessionID + ")");
                return false;
            }
        }

        public static List<Attendant> GetAttendantInSession(long SessionID, int SessionAttendantType)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Attendant> attendantsInTime = context.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == SessionAttendantType && ww.Type != 8 && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).OrderBy(s => s.LongName).ThenBy(s => s.CreatedAt).ToList();
                    return attendantsInTime;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Attendant> GetAttendantInSession(long SessionID, int SessionAttendantType, bool excluded)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    long sessionPresidentID = 0;
                    List<Attendant> attendantsInTime = new List<Attendant>();
                    if (excluded)
                    {
                        Session sessionObj = SessionHelper.GetSessionByID(SessionID);
                        if (sessionObj.PresidentID != null && sessionObj.PresidentID != 0)
                        {
                            Attendant attObj = AttendantHelper.GetAttendantByDefaultAttendantId(long.Parse(sessionObj.PresidentID.ToString()));
                            sessionPresidentID = attObj.ID;
                            attendantsInTime = context.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == SessionAttendantType && ww.Type != 8 && ww.Type != 9 && ww.DefaultAttendantID != sessionObj.PresidentID && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).OrderBy(s => s.LongName).ThenBy(s => s.CreatedAt).ToList();
                        }
                    }
                    else
                    {
                        attendantsInTime = context.Attendants.Select(aa => aa).Where(ww => ww.SessionAttendantType == SessionAttendantType && ww.Type != 8 && ww.Type != 9 && ww.Sessions.Any(aaaa => aaaa.ID == SessionID)).OrderBy(s => s.LongName).ThenBy(s => s.CreatedAt).ToList();
                    }
                    return attendantsInTime;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static bool UpdateAttendantState(int AttendantStatus, int DefaultAttendantId, DateTime? AttendDate)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Attendant atendant = context.Attendants.Select(eee => eee).Where(aaa => aaa.ID == DefaultAttendantId).FirstOrDefault();
                    atendant.State = AttendantStatus;
                    if (AttendDate != null)
                    {
                        atendant.AttendDate = AttendDate;
                    }

                    if (AttendantStatus != 0)
                    {
                        int res = context.SaveChanges();
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
