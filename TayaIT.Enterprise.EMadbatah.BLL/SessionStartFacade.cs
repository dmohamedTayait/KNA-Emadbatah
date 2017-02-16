using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.DAL;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Localization;
using TayaIT.Enterprise.EMadbatah.Util;
using System.Globalization;
using System.Data.Objects.DataClasses;
namespace TayaIT.Enterprise.EMadbatah.BLL
{
    public class SessionStartFacade
    {
        public static string madbatahHeader = " عقد مجلس الأمة جلسته العادية العلنية فى تمام الساعة "
            + "%sessionTime%"
            + " "
            + "من صباح يوم "
            + "%hijriDate%"
            + " ، "
            + " الموافق "
            + "%GeorgianDate%"
            + " برئاسة "
            + " %President% "
            + " %PresidentTitle%"
            + ".";

        public static string madbatahStartNotOnTime = "( أخرت الجلسة فى تمام الساعة "
                + "%sessionTime%"
                + " صباحا ثم عقد مجلس الأمة جلسته العادية العلنية فى تمام الساعة "
                + "%sessionTime%"
                + " من صباح يوم "
                + "%hijriDate%"
                + " ، "
                + " الموافق "
                + "%GeorgianDate%"
                + " برئاسة "
                + " %President% "
                + " %PresidentTitle%"
                + ")";

        public static string madbatahStartsection2 = "** وتولى الأمانة العامة السيد علام علي الكندري الأمين العام للمجلس والسيد عادل عيسى اللوغاني الأمين العام المساعد لقطاع الجلسات والسيد محمد عبدالمجيد الخنفر مدير إدارة المضابط .";
        public static string madbatahStartsection3 = "** و حضر الجلسة مندوبو الصحافة و الإعلام و لفيف من السادة المواطنين .";

        public static string presidentStr = "السيد الرئيــــــــــــــــــــــــــــــــــس :";
        public static string tempPresidentStr = "السيد رئيس الجلســــــــــــــــــــــة :";
        public static string sessionAttendantTitle = "* وبحضــور الســــادة الأعضــــــــاء : ";
        public static string sessionAttendantTitle2 = "* تليت بعد افتتاح الجلسة أسماء الأعضاء الحاضرين:";
        public static string attendantWithinSessionTitle = "* و فى أثناء الجلسة حضر كل من السادة الأعضـــاء:";
        public static string absentAttendantTitle = "*الغائبـــــــــون بدون عـــــــذر:";
        public static string abologizeAttendantTitle = "* الغائبــــــــــون بعــــــــــــــذر:";


        // public static string madbatahTuesdayIntro = "بسم الله الرحمن الرحيم و الصلاة و السلام على رسول الله ، تفتح الجلسة و تتلى اسماء الأعضاء ثم أسماء المعتذرين عن جلسة اليوم ثم أسماء الغائبين و المنصرفين عن الجلسة الماضية دون إذن أو اخطار .";
        public static string madbatahWednesdayIntro = "بسم الله ، و الحمد لله ، و الصلاة و السلام على رسول الله ، تفتح الجلسة و تتلى اسماء الأعضاء ثم أسماء السادة الأعضاء المعتذرين عن جلسة اليوم .";
        public static string madbatahTuesdayIntro = madbatahWednesdayIntro;
        public static string madbatahIntro = madbatahTuesdayIntro;

        public static string marginZeroStyle = "margin-top:0em;margin-bottom: 0em;";

        public static string directionStyle = "direction:rtl;";
        public static string defBoldFont = "font-family:AdvertisingBold;";
        public static string defFont = "font-family:AdvertisingMeduim;";
        public static string defFontSize = "font-size:14pt;";
        public static string lineHeight = "line-height:150%;";

        public static string basicPStyle = defBoldFont + defFontSize + lineHeight + marginZeroStyle + directionStyle;

        public static string textJustify = "text-align: justify;";
        public static string textJustifyKashida = "text-justify:kashida;";
        public static string textRight = "text-align: right;";
        public static string textCenter = "text-align: center;";
        public static string textunderline = "text-decoration:underline;";
        public static string pagebreak = "page-break-inside:avoid;";

        public static string tableWidth = "width:100%;";
        public static string tableStyle = tableWidth + directionStyle + marginZeroStyle;
        public static string tdJustifyStyle = basicPStyle + textRight + textJustifyKashida;
        public static string tdCenterStyle = "text-indent: 70px;" + basicPStyle + textRight;
        //public static string tdCenterStyle = basicPStyle + textCenter;

        public static string emptyParag = "<p style='" + basicPStyle + "'>&nbsp;</p>";

        public static List<List<Attendant>> GetSessionAttendantOrderedByStatus(long sessionID, int sessionAttendantType)
        {
            List<Attendant> sessionAttendants = new List<Attendant>();
            List<Attendant> attendants = new List<Attendant>();
            List<Attendant> attendantsWithinSession = new List<Attendant>();
            List<Attendant> absenceAttendants = new List<Attendant>();
            List<Attendant> abologyAttendants = new List<Attendant>();
            List<Attendant> inMissionAttendants = new List<Attendant>();
            List<List<Attendant>> allAttendants = new List<List<Attendant>>();

            sessionAttendants = AttendantHelper.GetAttendantInSession(sessionID, sessionAttendantType, true);


            foreach (Attendant attendant in sessionAttendants)
            {
                switch ((Model.AttendantState)attendant.State)
                {
                    case Model.AttendantState.Apology:
                        abologyAttendants.Add(attendant);
                        break;
                    case Model.AttendantState.Absent:
                        absenceAttendants.Add(attendant);
                        break;
                    case Model.AttendantState.Attended:
                        attendants.Add(attendant);
                        break;
                    case Model.AttendantState.InMission:
                        inMissionAttendants.Add(attendant);
                        break;
                    case Model.AttendantState.AttendWithinSession:
                        attendantsWithinSession.Add(attendant);
                        break;
                }
            }
            allAttendants.Add(attendants); //بحضور السادة الاعضاء
            allAttendants.Add(attendantsWithinSession); // حضر أثناء الجلسة
            allAttendants.Add(abologyAttendants);//الغائبون بعذر
            allAttendants.Add(absenceAttendants);// الغائبون بدون عذر
            allAttendants.Add(inMissionAttendants);//مهمة


            return allAttendants;
        }

        public static SessionDetails GetSessionDetails(long sessionID)
        {
            return EMadbatahFacade.GetSessionDetailsBySessionID(sessionID);
        }

        public static List<AgendaSubItem> GetAgendaSubItemsbyAgendaID(long agendaItemID)
        {
            return AgendaHelper.GetAgendaSubItemsByAgendaID(agendaItemID);
        }

        public static string GetAutomaticSessionStartText(long sessionID)
        {
            SessionDetails details = GetSessionDetails(sessionID);
            DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");
            string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
            string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month);
            string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
            string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;

            try
            {
                int dayOfMonthNumHijNum = int.Parse(dayOfMonthNumHij);
                dayOfMonthNumHij = dayOfMonthNumHijNum.ToString();
            }
            catch
            {
            }

            string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;
            string sessionNum = details.Subject; //"الخامسة عشره";
            string hijriDate = dayNameAr + " " + dayOfMonthNumHij + " من " + monthNameHijAr + " سنة " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
            string gDate = details.Date.Day + " من " + monthNameAr + " سنة " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";
            string timeInHour = LocalHelper.GetLocalizedString("strHour" + details.StartTime.Hour);// +" " + LocalHelper.GetLocalizedString("strTime" + details.Date.ToString("tt"));//"التاسعة صباحا";
            string seasonType = details.StageType;// "العادي";
            long seasonStage = details.Stage;// "الخامس";
            string sessionSeason = details.Season + "";// "الرابع عشر";
            string president = "";
            string presidentTitle = "";
            DefaultAttendant att = DefaultAttendantHelper.GetAttendantById(details.PresidentID);
            if (att != null)
            {
                president = att.AttendantTitle + " " + att.LongName;
                if (att.Type == (int)Model.AttendantType.President)
                    presidentTitle = "رئيس مجلس الأمة";
                else if (att.Type != (int)Model.AttendantType.President && att.JobTitle != null)
                    presidentTitle = att.JobTitle;

            }

            string sessionStart = "<p style='" + basicPStyle + textJustify + "'>" + madbatahHeader.Replace("%sessionNum%", sessionNum).Replace("%GeorgianDate%", gDate).Replace("%sessionTime%", timeInHour).Replace("%hijriDate%", hijriDate).Replace("%President%", president).Replace("%PresidentTitle%", presidentTitle) + "</p>";

            string body = emptyParag;

            List<Attendant> attendants = new List<Attendant>();
            List<Attendant> attendantsWithinSession = new List<Attendant>();
            List<Attendant> abologAttendants = new List<Attendant>();
            List<Attendant> absenceAttendants = new List<Attendant>();
            List<Attendant> inMissionAttendants = new List<Attendant>();

            List<List<Attendant>> allAttendants = new List<List<Attendant>>();

            //Session Started on Time
            if (details.SessionStartFlag == (int)SessionOpenStatus.OnTime)
            {
                body += "<p style='" + basicPStyle + textunderline + "'>" + presidentStr + "</p>";
                if (details.Date.DayOfWeek == DayOfWeek.Tuesday)
                    body += "<p style='" + basicPStyle + textJustify + "'>" + SessionStartFacade.madbatahTuesdayIntro + "</p>";
                else
                    body += "<p style='" + basicPStyle + textJustify + "'>" + SessionStartFacade.madbatahWednesdayIntro + "</p>";

                body += emptyParag;
                allAttendants = SessionStartFacade.GetSessionAttendantOrderedByStatus(details.SessionID, details.SessionStartFlag);
                if (allAttendants.Count > 0)
                {
                    attendants = allAttendants[0];
                    attendantsWithinSession = allAttendants[1];
                    abologAttendants = allAttendants[2];
                    absenceAttendants = allAttendants[3];
                    inMissionAttendants = allAttendants[4];
                    body += writeAttendantNFile(sessionAttendantTitle2, attendants);
                    body += writeAttendantNFile(attendantWithinSessionTitle, attendantsWithinSession);
                    body += writeAttendantNFile(abologizeAttendantTitle, abologAttendants);
                    body += writeAttendantNFile(absentAttendantTitle, absenceAttendants);
                }
            }
            //Session Started After Time
            else
            {
                allAttendants = SessionStartFacade.GetSessionAttendantOrderedByStatus(details.SessionID, (int)SessionOpenStatus.OnTime);
                if (allAttendants.Count > 0)
                    body += writeAttendantNFile(sessionAttendantTitle, allAttendants[0]);

                body += "<p style='" + basicPStyle + textunderline + textRight + "'>" + presidentStr + "</p>";
                body += "<p style='" + basicPStyle + textJustify + "'>" + "بسم الله الرحمن الرحيم و الصلاة و السلام على رسول الله ، نتيجة لعدم اكتمال النصاب تأخر الجلسة لمدة نصف ساعة." + "</p>";
                body += emptyParag;

                string docStartNotOnTime = SessionStartFacade.madbatahStartNotOnTime.Replace("%sessionNum%", sessionNum).Replace("%GeorgianDate%", gDate).Replace("%sessionTime%", timeInHour).Replace("%hijriDate%", hijriDate).Replace("%President%", president).Replace("%PresidentTitle%", presidentTitle);
                body += "<p style='" + basicPStyle + textJustify + "'>" + docStartNotOnTime + "</p>";
                body += emptyParag;

                body += "<p style='" + basicPStyle + textRight + textunderline + "'>" + presidentStr + "</p>";

                if (details.Date.DayOfWeek == DayOfWeek.Tuesday)
                    body += "<p style='" + basicPStyle + textJustify + "'>" + SessionStartFacade.madbatahTuesdayIntro + "</p>";
                else
                    body += "<p style='" + basicPStyle + textJustify + "'>" + SessionStartFacade.madbatahWednesdayIntro + "</p>";

                body += emptyParag;

                allAttendants = SessionStartFacade.GetSessionAttendantOrderedByStatus(details.SessionID, (int)SessionOpenStatus.NotOnTime);
                if (allAttendants.Count > 0)
                {
                    attendants = allAttendants[0];
                    attendantsWithinSession = allAttendants[1];
                    abologAttendants = allAttendants[2];
                    absenceAttendants = allAttendants[3];
                    inMissionAttendants = allAttendants[4];
                    body += writeAttendantNFile(sessionAttendantTitle2, attendants);
                    body += writeAttendantNFile(attendantWithinSessionTitle, attendantsWithinSession);
                    body += writeAttendantNFile(abologizeAttendantTitle, abologAttendants);
                    body += writeAttendantNFile(absentAttendantTitle, absenceAttendants);

                }
            }

            //Committee Attendance
            bool isCommittee = false;
            List<Committee> committeeLst = CommitteeHelper.GetAllCommittee();
            DefaultAttendant defAtt = new DefaultAttendant();
            List<DefaultAttendant> lstAbsenceDefAtt = new List<DefaultAttendant>();
            List<DefaultAttendant> lstAbologizeDefAtt = new List<DefaultAttendant>();
            List<DefaultAttendant> lstInMissioneDefAtt = new List<DefaultAttendant>();
            int i = 0;
            foreach (Committee comm in committeeLst)
            {
                List<SessionCommittee> scomms = SessionCommitteeHelper.GetSessionCommitteeBySessionIDAndCommitteeID(sessionID, comm.ID);
                string commNameStr = "";
                int ctr = 0;
                foreach (SessionCommittee scommobj in scomms)
                {
                    List<SessionCommitteeAttendant> sCommAtt = SessionCommitteeHelper.GetSessionCommitteeAttendance(scommobj.ID);
                    if (sCommAtt.Count > 0)
                    {
                        if (!isCommittee)
                        {
                            body += "<p style='" + basicPStyle + textRight + textunderline + "'>" + "* أسماء السادة الأعضاء الذين تغيبوا باعتذار سابق أو بدونه عن عدم حضور اجتماعات لجان المجلس الدائمة و المؤقتة :" + "</p>";
                            body += emptyParag;
                            isCommittee = true;
                        }

                        lstAbsenceDefAtt = new List<DefaultAttendant>();
                        lstAbologizeDefAtt = new List<DefaultAttendant>();
                        lstInMissioneDefAtt = new List<DefaultAttendant>();
                        foreach (SessionCommitteeAttendant sCommAttObj in sCommAtt)
                        {
                            defAtt = new DefaultAttendant();
                            defAtt = DefaultAttendantHelper.GetAttendantById((long)sCommAttObj.DefaultAttendantID);
                            if (sCommAttObj.Status == (int)Model.AttendantState.Absent)
                                lstAbsenceDefAtt.Add(defAtt);

                            if (sCommAttObj.Status == (int)Model.AttendantState.Apology)
                                lstAbologizeDefAtt.Add(defAtt);

                            if (sCommAttObj.Status == (int)Model.AttendantState.InMission)
                                lstInMissioneDefAtt.Add(defAtt);
                        }

                        commNameStr = "";

                        string dateFormatted = FormatDate((DateTime)scommobj.CreatedAt);
                        if (scomms.Count == 1)
                        {
                            commNameStr += comm.CommitteeName + " اجتماع " + scommobj.CommitteeName + " ";
                            commNameStr += dateFormatted + " ";

                            if (!string.IsNullOrEmpty(scommobj.AddedDetails))
                                commNameStr += " ( " + scommobj.AddedDetails.ToString() + ").";
                            else commNameStr += " : ";
                            commNameStr = (i + 1).ToString() + " - " + commNameStr;

                            body += "<p style='" + basicPStyle + textunderline + textRight + "'>" + commNameStr + "</p>";
                        }
                        else
                        {
                            if (ctr == 0)
                            {
                                commNameStr += comm.CommitteeName + " : ";
                                commNameStr = (i + 1).ToString() + " - " + commNameStr;

                                body += "<p style='" + basicPStyle + textunderline + textRight + "'>" + commNameStr + "</p>";
                            }

                            commNameStr = "- الاجتماع ";
                            commNameStr += scommobj.CommitteeName + " بتاريخ ";
                            commNameStr += dateFormatted + " ";

                            if (!string.IsNullOrEmpty(scommobj.AddedDetails))
                                commNameStr += " ( " + scommobj.AddedDetails.ToString() + ").";
                            else commNameStr += " : ";

                            body += "<p style='" + basicPStyle + textRight + "'>" + commNameStr + "</p>";
                        }

                        if (ctr == 0)
                        {
                            i++;
                            ctr++;
                        }

                        // body += emptyParag;
                        List<List<DefaultAttendant>> AllInLst = new List<List<DefaultAttendant>>();
                        AllInLst.Add(lstInMissioneDefAtt);
                        AllInLst.Add(lstAbologizeDefAtt);
                        AllInLst.Add(lstAbsenceDefAtt);
                        body += writeAttendantNFile(AllInLst);
                    }
                }
            }

            string madbatahStart = "<html style='" + directionStyle + "'>";
            madbatahStart += "<body dir='" + directionStyle + "'>";
            madbatahStart += sessionStart + body;
            madbatahStart += "<p style='" + basicPStyle + textJustify + "'>" + madbatahStartsection2 + "</p>" + emptyParag;
            madbatahStart += "<p style='" + basicPStyle + textJustify + "'>" + madbatahStartsection3 + "</p>" + emptyParag;
            madbatahStart += "</body></html>";
            return madbatahStart;
        }

        public static string FormatDate(DateTime date)
        {
            DateTime fdate = new DateTime(date.Year, date.Month, date.Day);
            string s = fdate.ToString("d/M/yyyy", CultureInfo.InvariantCulture);

            return s + "  م";
        }

        public static string writeAttendantNFile(string head, List<Attendant> attendants)
        {
            string body = "";
            if (attendants.Count > 0)
            {
                body += "<p style='" + basicPStyle + textunderline + textRight + "'>" + head + "</p>";
                body += "<table style='" + tableStyle + "'>";
                foreach (Attendant att in attendants)
                {
                    if (att.Name != "غير معرف")
                    {
                        body += "<tr style='" + pagebreak + "'><td><p style=' " + tdJustifyStyle + " '>" + "   - " + att.Name.Trim() + "</p>";
                        if (!String.IsNullOrEmpty(att.JobTitle))
                            body += "<p style=' " + tdCenterStyle + " '>" + "(" + att.JobTitle.Trim() + ")" + "</p>";
                        body += "</td></tr>";
                    }
                }
                body += "</table>";
                body += emptyParag;
            }
            return body;
        }

        public static string writeAttendantNFile(List<List<DefaultAttendant>> allAttendantsLst)
        {
            string body = "";
            string attStr = "";
            string head = "";

            List<DefaultAttendant> lstInMissioneDefAtt = allAttendantsLst[0];
            List<DefaultAttendant> lstAbologizeDefAtt = allAttendantsLst[1];
            List<DefaultAttendant> lstAbsenceDefAtt = allAttendantsLst[2];

            if (lstInMissioneDefAtt.Count > 0 || lstAbologizeDefAtt.Count > 0 | lstAbsenceDefAtt.Count > 0)
            {
                body += "<table style='" + tableStyle + "'>";
                foreach (DefaultAttendant att in lstInMissioneDefAtt)
                {
                    if (att.Name != "غير معرف")
                    {
                        attStr = "   - " + att.Name.Trim();
                        head = "مهمة رسمية";
                        body += "<tr style='" + pagebreak + "'><td style='width:65%'><p style='" + tdJustifyStyle + "'>" + attStr + "</p></td><td><p style='" + tdJustifyStyle + "'>" + head + "</p></td></tr>";
                        if (!String.IsNullOrEmpty(att.JobTitle))
                            body += "<tr style='" + pagebreak + "'><td><p style=' " + tdCenterStyle + " '>" + "(" + att.JobTitle.Trim() + ")" + "</p></td><td></td></tr>";
                    }
                }

                foreach (DefaultAttendant att in lstAbologizeDefAtt)
                {
                    if (att.Name != "غير معرف")
                    {
                        attStr = "   - " + att.Name.Trim();
                        head = "بعذر";
                        body += "<tr style='" + pagebreak + "'><td style='width:65%'><p style='" + tdJustifyStyle + "'>" + attStr + "</p></td><td><p style='" + tdJustifyStyle + "'>" + head + "</p></td></tr>";
                        if (!String.IsNullOrEmpty(att.JobTitle))
                            body += "<tr style='" + pagebreak + "'><td><p style=' " + tdCenterStyle + " '>" + "(" + att.JobTitle.Trim() + ")" + "</p></td><td></td></tr>";
                    }
                }

                foreach (DefaultAttendant att in lstAbsenceDefAtt)
                {
                    if (att.Name != "غير معرف")
                    {
                        attStr = "   - " + att.Name.Trim();
                        head = "بدون عذر";
                        body += "<tr style='" + pagebreak + "'><td style='width:65%'><p style='" + tdJustifyStyle + "'>" + attStr + "</p></td><td><p style='" + tdJustifyStyle + "'>" + head + "</p></td></tr>";
                        if (!String.IsNullOrEmpty(att.JobTitle))
                            body += "<tr style='" + pagebreak + "'><td><p style=' " + tdCenterStyle + " '>" + "(" + att.JobTitle.Trim() + ")" + "</p></td><td></td></tr>";
                    }
                }
                body += "</table>";
                body += emptyParag;
            }
            return body;
        }

        public static bool AddUpdateSessionStart(long sessionId, string sessionStartText, long userID, string startName)
        {

            //Session session = SessionHelper.GetSessionByID(sessionId);
            SessionFile sessionStart = SessionStartHelper.GetSessionStartBySessionId(sessionId);
            if (sessionStart == null)
            {
                DAL.SessionFile start = SessionStartHelper.AddSessionStart(sessionStartText, userID, sessionId, startName);
                if (start != null)
                    return true;
                else
                    return false;
                //SessionStartFacade.UpdateSessionSetSessionStartID(sessionId, start.ID);
            }
            else
            {
                if (SessionStartHelper.UpdateSessionStartText(sessionStart.ID, sessionStartText, userID) > 0)
                    return true;
                else
                    return false;
            }


        }

        public static bool AddNewSessionStart(long sessionId, string sessionStartText, string startName)
        {


            DAL.SessionFile start = SessionStartHelper.AddSessionStart(sessionStartText, sessionId, startName);
            if (start != null)
                return true;
            else
                return false;
            //SessionStartFacade.UpdateSessionSetSessionStartID(sessionId, start.ID);


        }

        public static SessionFile GetSessionStartBySessionID(long sessionID)
        {
            return SessionStartHelper.GetSessionStartBySessionId(sessionID);
        }
    }
}
