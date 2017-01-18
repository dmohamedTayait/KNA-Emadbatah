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
        // public static string madbatahHeader =
        //    "<div style=\"text-align:center;direction:rtl;font-family:arial;font-face:arial;font-size:14pt;clear:both;\"><strong>جدول أعمال %sessionNum% <br>المعقودة يوم : %HijriDate% <br>الموافـــق :  %GeorgianDate%"
        //    + "<br>ــــــــــــــــــــــــــــــــــ"
        //   + "<br>(  الساعة %sessionTime%   )</strong></div>";


        public static string madbatahHeader = " كان محددا لاجتماع مجلس الأمة بجلسته العادية العلنية تمام الساعة "
            + "%sessionTime%"
            + " "
            + "من صباح يوم "
            + "%hijriDate%"
            + " ، "
            + " الموافق "
            + "%GeorgianDate%"
            + " و فى تمام الساعة "
            + "%sessionTime%"
            + " حضر الى منصة الرئاسة "
            + " %President% "
            + " رئيس مجلس الأمة"
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
                + ") برئاسة السيد"
                + " %President% "
                + " رئيس مجلس الأمة"
                + ")";

        public static string madbatahTuesdayIntro = "بسم الله الرحمن الرحيم و الصلاة و السلام على رسول الله ، تفتح الجلسة و تتلى اسماء الأعضاء ثم أسماء المعتذرين عن جلسة اليوم ثم أسماء الغائبين و المنصرفين عن الجلسة الماضية دون إذن أو اخطار .";

        public static string madbatahWednesdayIntro = "بسم الله الرحمن الرحيم و الصلاة و السلام على رسول الله ، تفتح الجلسة و تتلى اسماء الأعضاء ثم أسماء المعتذرين عن جلسة اليوم ثم أسماء الغائبين و المنصرفين عن الجلسة الماضية دون إذن أو اخطار .";


        public static string madbatahIntro = madbatahTuesdayIntro;

        public static List<List<Attendant>> GetSessionAttendantOrderedByStatus(long sessionID, int sessionAttendantType)
        {
            List<Attendant> sessionAttendants = new List<Attendant>();
            List<Attendant> attendants = new List<Attendant>();
            List<Attendant> attendantsWithinSession = new List<Attendant>();
            List<Attendant> absenceAttendants = new List<Attendant>();
            List<Attendant> abologyAttendants = new List<Attendant>();
            List<Attendant> inMissionAttendants = new List<Attendant>();
            List<List<Attendant>> allAttendants = new List<List<Attendant>>();

            sessionAttendants = AttendantHelper.GetAttendantInSession(sessionID, sessionAttendantType);


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
            allAttendants.Add(abologyAttendants);// الغائبون بدون عذر
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
        //public static Hashtable GetSessionAgendaItems(long sessionID)
        //{
        //    return EMadbatahFacade.GetSessionBySessionID(sessionID).AgendaItems;
        //    //((SessionAgendaItem)EMadbatahFacade.GetSessionBySessionID(sessionID).AgendaItems[0]).SubAgendaItems;
        //}

        //public static int UpdateSessionSetSessionStartID(long sessionID, long sessionStartID)
        //{
        //    return SessionHelper.UpdateSessionSetSessionStartID(sessionID, sessionStartID);
        //}
        //public static SessionStart AddSessionStart(string text, long userID)
        //{
        //    return SessionStartHelper.AddSessionStart(text, userID,false);
        //}
        //public static int UpdateSessionStart(long sessionStartID, string text)
        //{
        //    return SessionStartHelper.UpdateSessionStartText(sessionStartID, text);
        //}
        public static string GetAutomaticSessionStartTextOld(long sessionID)
        {


            //new usama
            // the session start naming of الإول الثاني الثالث will be based on the ItemOrder from AgendaTable


            NumberingFormatter fomratterFemale = new NumberingFormatter(false);
            NumberingFormatter fomratterMale = new NumberingFormatter(true);
            Model.SessionDetails details = SessionStartFacade.GetSessionDetails(sessionID);

            //calculate hijri date
            //details.Date = details.Date.Subtract(new TimeSpan(1, 0, 0));
            DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");
            //DateTime hijDate = details.Date.ToString("f", dateFormat);


            string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
            string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month);
            string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
            string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;
            string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;

            /// We format the date structure to whatever we want - LAITH - 11/13/2005 1:05:39 PM -
            //  dateFormat.ShortDatePattern = "dd/MM/yyyy";
            //   return dateConv;//(dateConv.Date.ToString("f", dateFormat));

            //for header
            string sessionNum = details.Subject; //"الخامسة عشره";
            string hijriDate = dayNameAr + " " + dayOfMonthNumHij + " " + monthNameHijAr + " سنة " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
            string gDate = details.Date.Day + " " + monthNameAr + " سنة " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";
            string timeInHour = LocalHelper.GetLocalizedString("strHour" + details.StartTime.Hour) + " " + LocalHelper.GetLocalizedString("strTime" + details.Date.ToString("tt"));//"التاسعة صباحا";
            string seasonType = details.StageType;// "العادي";
            long seasonStage = details.Stage;// "الخامس";
            string sessionSeason = details.Season + "";// "الرابع عشر";
            string sessionHeader = madbatahHeader.Replace("%sessionNum%", sessionNum);
            sessionHeader = sessionHeader.Replace("%HijriDate%", hijriDate);
            sessionHeader = sessionHeader.Replace("%GeorgianDate%", gDate);
            sessionHeader = sessionHeader.Replace("%sessionTime%", timeInHour);

            //if (details.AgendaItems == null)
            //    return "";
            //for gadwal el a3mal
            /*
            Hashtable agendaItems = details.AgendaItems;
            string gadawalA3mal = "";
            int counter = 0;
            foreach (DictionaryEntry Item in agendaItems)
            {

                SessionAgendaItem agendaItem = (SessionAgendaItem)Item.Value;
                string eparliamentID = Item.Key.ToString();
                //List<AgendaSubItem> subItems = SessionStartFacade.GetAgendaSubItemsbyAgendaID(agendaItem.ID);
                List<SessionAgendaItem> subItems = agendaItem.SubAgendaItems;
                string itemNum = "البند " + LocalHelper.GetLocalizedString("str" + (counter + 1)) + " :";
                gadawalA3mal += "<strong>" + itemNum + agendaItem.Text + "</strong><br>";
                if (subItems.Count > 0)
                {
                    for (int i = 0; i < subItems.Count; i++)
                    {
                        gadawalA3mal += (i + 1) + ".  " + subItems[i].Text + "<br>";
                    }
                }
                counter++;
            }
            */
            //for ordered agenda as existing from editor
            List<AgendaItem> agendaItems = SessionContentItemHelper.GetOrderedAgendaItemsBySessionID(sessionID);

            string gadawalA3mal = "<p style=\"text-align:right; direction: rtl; font-family: arial; font-size: 14pt;\">";
            int counter = 1;
            foreach (AgendaItem Item in agendaItems)
            {


                EntityCollection<AgendaSubItem> subItems = Item.AgendaSubItems;


                //string itemNum = "البند " + LocalHelper.GetLocalizedString("str" + (counter + 1)) + " :";

                //string itemNum = "البند " + LocalHelper.GetLocalizedString("str" + Item.Order) + " :"; ////usama new ordering
                string itemNum = "البند " + LocalHelper.GetLocalizedString("str" + counter) + " :"; ////usama new ordering


                //if (Item.Name.Contains("افتتاحية الجلسة ، وكلمة معالي رئيس المجلس"))
                if (Item.IsCustom == true)//for handling eftta7eyet el magles
                {
                    gadawalA3mal += "<strong>" + Item.Name + "</strong><br>";
                    counter--;
                }
                else
                    gadawalA3mal += "<strong>" + itemNum + Item.Name + "</strong><br>";

                if (subItems.Count > 0)
                {
                    int i = 0;
                    foreach (AgendaSubItem sub in subItems)
                    {
                        if (!string.IsNullOrEmpty(sub.QFrom) && !string.IsNullOrEmpty(sub.QTo))
                            sub.Name = new StringBuilder().Append("سؤال موجه إلى معالي / ") + sub.QTo + "من سعادةالعضو /" + sub.QFrom + "حول \"" + sub.Name + "\".";
                        gadawalA3mal += (i + 1) + ".  " + sub.Name + "<br>";
                        i++;
                    }
                    //for (int i = 0; i < subItems.Count; i++)
                    //{
                    //    gadawalA3mal += (i + 1) + ".  " + subItems.[i].Name + "<br>";
                    //}
                }
                counter++;
            }
            gadawalA3mal += "</p>";
            //FOR session introduction
            string sessionIntro = madbatahIntro.Replace("%sessionNum%", sessionNum);
            sessionIntro = sessionIntro.Replace("%HijriDate%", hijriDate);
            sessionIntro = sessionIntro.Replace("%GeorgianDate%", gDate);
            sessionIntro = sessionIntro.Replace("%sessionTime%", timeInHour);
            sessionIntro = sessionIntro.Replace("%sessionType%", seasonType);
            sessionIntro = sessionIntro.Replace("%sessionSeason%", fomratterMale.getResultEnhanced(int.Parse(sessionSeason.Trim())));
            sessionIntro = sessionIntro.Replace("%sessionStage%", fomratterMale.getResultEnhanced((int)seasonStage));

            sessionIntro = sessionIntro.Replace("%sessionPresident%", details.Presidnt);

            //for attendance
            List<SessionAttendant> attendance = details.Attendance;
            string president = details.Presidnt;
            DateTime date = details.Date;

            List<SessionAttendant> apologies = new List<SessionAttendant>();
            List<SessionAttendant> absents = new List<SessionAttendant>();
            List<SessionAttendant> inMission = new List<SessionAttendant>();
            List<SessionAttendant> secertairs = new List<SessionAttendant>();
            List<SessionAttendant> attendingFromMajles = new List<SessionAttendant>();
            List<SessionAttendant> attendingOutOfMajles = new List<SessionAttendant>();
            List<SessionAttendant> attendingFromGovernment = new List<SessionAttendant>();
            List<SessionAttendant> othersAttending = new List<SessionAttendant>();

            foreach (SessionAttendant attendant in attendance)
            {
                switch (attendant.State)
                {
                    case Model.AttendantState.Apology:
                        apologies.Add(attendant);
                        break;
                    case Model.AttendantState.Absent:
                        absents.Add(attendant);
                        break;
                    case Model.AttendantState.Attended:
                        switch (attendant.Type)
                        {
                            case Model.AttendantType.FromOutsideTheCouncil:
                                attendingOutOfMajles.Add(attendant);
                                break;
                            case Model.AttendantType.FromTheCouncilMembers:
                                attendingFromMajles.Add(attendant);
                                break;
                            case Model.AttendantType.GovernmentRepresentative:
                                attendingFromGovernment.Add(attendant);
                                break;
                            case Model.AttendantType.Secretariat:
                                secertairs.Add(attendant);
                                break;
                            case Model.AttendantType.NA:
                                othersAttending.Add(attendant);
                                break;
                        }
                        break;

                    case Model.AttendantState.InMission:
                        inMission.Add(attendant);
                        break;

                }
            }

            string introBody = "";
            if (inMission.Count > 0)
            {
                introBody += "<div style=\"text-align:right;direction:rtl;font-family:arial;font-size:14pt;clear:both;\">";
                introBody += "<strong>وقد اعتذر</strong> عن عدم حضور هذه الجلسة في مهمه رسميه كل من :";
                introBody += "</div><br/>";
                introBody += "<table align=\"center\"  width=\"100%\" style=\"text-align:right; direction: rtl; font-family: arial; font-size: 14pt;\">";
                for (int i = 0; i < inMission.Count; i++)
                {
                    string tdTag = "<td style=\"text-align:right;direction:rtl;unicode-bidi:embed\" dir=\"RTL\" >";
                    if (i == 0)
                        tdTag = "<td width=\"50%\" style=\"text-align:right;direction:rtl;unicode-bidi:embed\" dir=\"RTL\" >";
                    if (i % 2 == 0)
                    {
                        introBody += "<tr>";
                        introBody += tdTag + (i + 1) + ". " + MabatahCreatorFacade.GetAttendantTitle(inMission[i], sessionID) + "</td>";
                    }
                    else
                    {
                        introBody += tdTag + (i + 1) + ". " + MabatahCreatorFacade.GetAttendantTitle(inMission[i], sessionID) + "</td>";
                        introBody += "</tr>";
                    }
                }
                if (inMission.Count % 2 != 0)
                {
                    introBody += "</tr>";
                }
                introBody += "</table>";
            }
            //introBody += "<br>";
            if (apologies.Count > 0)
            {
                introBody += "<div style=\"text-align:right;direction:rtl;font-family:arial;font-size:14pt;clear:both;\">";
                introBody += "<strong>وقد اعتذر</strong> عن عدم حضور هذه الجلسة كل من :";
                introBody += "</div><br/>";
                introBody += "<table align=\"center\" width=\"100%\" style=\"text-align:right; direction: rtl; font-family: arial; font-size: 14pt;\">";
                for (int i = 0; i < apologies.Count; i++)
                {
                    string tdTag = "<td style=\"text-align:right;direction:rtl;unicode-bidi:embed\" dir=\"RTL\" >";
                    if (i == 0)
                        tdTag = "<td width=\"50%\" style=\"text-align:right;direction:rtl;unicode-bidi:embed\" dir=\"RTL\" >";
                    if (i % 2 == 0)
                    {
                        introBody += "<tr>";
                        introBody += tdTag + (i + 1) + ". " + MabatahCreatorFacade.GetAttendantTitle(apologies[i], sessionID) + "</td>";
                    }
                    else
                    {
                        introBody += tdTag + (i + 1) + ". " + MabatahCreatorFacade.GetAttendantTitle(apologies[i], sessionID) + "</td>";
                        introBody += "</tr>";

                    }
                }
                if (apologies.Count % 2 != 0)
                {
                    introBody += "</tr>";
                }
                introBody += "</table>";
            }
            //introBody += "<br>";
            if (absents.Count > 0)
            {
                introBody += "<div style=\"text-align:right;direction:rtl;font-family:arial;font-size:14pt;clear:both;\">";
                introBody += "<strong>وقد تغيب</strong> عن حضور هذه الجلسة :";
                introBody += "</div><br/>";
                introBody += "<table align=\"center\" width=\"100%\" style=\"text-align:right; direction: rtl; font-family: arial; font-size: 14pt;\">";
                for (int i = 0; i < absents.Count; i++)
                {
                    string tdTag = "<td style=\"text-align:right;direction:rtl;unicode-bidi:embed\" dir=\"RTL\" >";
                    if (i == 0)
                        tdTag = "<td width=\"50%\" style=\"text-align:right;direction:rtl;unicode-bidi:embed\" dir=\"RTL\" >";
                    if (i % 2 == 0)
                    {
                        introBody += "<tr>";
                        introBody += tdTag + (i + 1) + ". " + MabatahCreatorFacade.GetAttendantTitle(absents[i], sessionID) + "</td>";
                    }
                    else
                    {
                        introBody += tdTag + (i + 1) + ". " + MabatahCreatorFacade.GetAttendantTitle(absents[i], sessionID) + "</td>";
                        introBody += "</tr>";

                    }
                }
                if (absents.Count % 2 != 0)
                {
                    introBody += "</tr>";
                }
                introBody += "</table>";
            }
            //introBody += "<br>";
            if (attendingFromGovernment.Count > 0)
            {
                introBody += "<div style=\"text-align:right;direction:rtl;font-family:arial;font-size:14pt;clear:both;\">";
                introBody += "<strong>وحضر</strong> هذه الجلسة كل من :";
                introBody += "</div><br/>";
                introBody += "<table align=\"right\"  width=\"100%\" style=\"text-align:right; direction: rtl; font-family: arial; font-size: 14pt;\">";
                for (int i = 0; i < attendingFromGovernment.Count; i++)
                {
                    introBody += "<tr>";
                    string tdTag = "<td style=\"text-align:right;direction:rtl;unicode-bidi:embed\" dir=\"RTL\" >";
                    if (i == 0)
                        tdTag = "<td width=\"50%\" style=\"text-align:right;direction:rtl;unicode-bidi:embed\" dir=\"RTL\" >";
                    introBody += tdTag + (i + 1) + ". " + MabatahCreatorFacade.GetAttendantTitle(attendingFromGovernment[i], sessionID) + "</td>";
                    introBody += tdTag + attendingFromGovernment[i].JobTitle + "</td>";
                    introBody += "</tr>";
                }
                introBody += "</table>";
            }
            //introBody += "<br>";
            if (attendingOutOfMajles.Count > 0)
            {
                introBody += "<div style=\"text-align:right;direction:rtl;font-family:arial;font-size:14pt;clear:both;\">";
                introBody += "<strong>كما حضر</strong> الجلسة كل من :";

                for (int i = 0; i < attendingOutOfMajles.Count; i++)
                {
                    introBody += MabatahCreatorFacade.GetAttendantTitle(attendingFromMajles[i], sessionID) + "." + attendingOutOfMajles[i].JobTitle + "،";
                }
                introBody += "</div>";
            }
            //introBody += "<br>";

            //if (secertairs.Count > 0)
            //{
            //    introBody += "<p style=\"text-align:right; direction: rtl; font-family: arial; font-size: 14pt;\">";
            //    introBody += "<strong>كما حضر</strong> الجلسة كل من :";

            //    for (int i = 0; i < secertairs.Count; i++)
            //    {
            //        introBody += secertairs[i].Name + "–" + secertairs[i].JobTitle + "،";
            //    }
            //    introBody += "</p>";
            //}

            //introBody += "<br>";
            if (secertairs.Count > 0)
            {
                introBody += "<div style=\"text-align:right;direction:rtl;font-family:arial;font-size:14pt;clear:both;\">";
                introBody += "<strong>وتولى</strong> الأمانة العامة :";

                for (int i = 0; i < secertairs.Count; i++)
                {
                    introBody += MabatahCreatorFacade.GetAttendantTitle(secertairs[i], sessionID) + " - " + secertairs[i].JobTitle + "،";
                    //introBody += "</tr>";
                }
                introBody += "</div>";
            }
            //introBody += "<br>";
            string madbatahStart = "<html style=\"direction: rtl; font-family: arial; font-size: 14pt;\">";
            madbatahStart += "<body dir=\"rtl\"><font size=\"14\" face=\"arial\" >";
            madbatahStart += /*"<p style=\"direction: rtl; font-family: arial; font-size: 14pt;\">" +*/ sessionHeader /*+"</p>"*/;
            madbatahStart += gadawalA3mal;// +"<br/>";
            madbatahStart += "<hr class=\"mceNonEditable\"/>";
            madbatahStart += sessionIntro;
            madbatahStart += introBody + "</font></body></html>";
            return madbatahStart;
        }

        public static string GetAutomaticSessionStartText(long sessionID)
        {
            SessionDetails details = GetSessionDetails(sessionID);
            DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");
            string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
            string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month);
            string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
            string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;
            string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;

            //for header
            string sessionNum = details.Subject; //"الخامسة عشره";
            string hijriDate = dayNameAr + " " + dayOfMonthNumHij + " من " + monthNameHijAr + " سنة " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
            string gDate = details.Date.Day + " من " + monthNameAr + " سنة " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";
            string timeInHour = LocalHelper.GetLocalizedString("strHour" + details.StartTime.Hour);// +" " + LocalHelper.GetLocalizedString("strTime" + details.Date.ToString("tt"));//"التاسعة صباحا";
            string seasonType = details.StageType;// "العادي";
            long seasonStage = details.Stage;// "الخامس";
            string sessionSeason = details.Season + "";// "الرابع عشر";

            string sessionStart = "<div style=\"text-align:right;direction:rtl;font-family:AdvertisingMedium;font-size:16pt;line-height:30px;font-weight:bold; text-align: right;clear:both;\">" + madbatahHeader.Replace("%sessionNum%", sessionNum).Replace("%GeorgianDate%", gDate).Replace("%sessionTime%", timeInHour).Replace("%hijriDate%", hijriDate).Replace("%President%", details.Presidnt) + "</div>";

            string body = "<br/>";

            List<Attendant> attendants = new List<Attendant>();
            List<Attendant> attendantsWithinSession = new List<Attendant>();
            List<Attendant> abologAttendants = new List<Attendant>();
            List<Attendant> absenceAttendants = new List<Attendant>();
            List<Attendant> inMissionAttendants = new List<Attendant>();

            List<List<Attendant>> allAttendants = new List<List<Attendant>>();

            //Session Started on Time
            if (details.SessionStartFlag == (int)SessionOpenStatus.OnTime)
            {
                allAttendants = SessionStartFacade.GetSessionAttendantOrderedByStatus(details.SessionID, details.SessionStartFlag);
                if (allAttendants.Count > 0)
                {
                    attendants = allAttendants[0];
                    attendantsWithinSession = allAttendants[1];
                    abologAttendants = allAttendants[2];
                    absenceAttendants = allAttendants[3];
                    inMissionAttendants = allAttendants[4];
                   body += writeAttendantNFile("* و بحضور السادة الأعضاء:", attendants);
                   body +=writeAttendantNFile("* و فى أثناء الجلسة حضر كل من السادة الأعضاء:", attendantsWithinSession);
                   body+= writeAttendantNFile("* الغائبون بعذر:", abologAttendants);
                   body+= writeAttendantNFile("*الغائبون بدون اعتذار:", absenceAttendants);
                }
            }
            //Session Started After Time
            else
            {
                allAttendants = SessionStartFacade.GetSessionAttendantOrderedByStatus(details.SessionID, (int)SessionOpenStatus.OnTime);
                if (allAttendants.Count > 0)
                    body += writeAttendantNFile("* و بحضور السادة الأعضاء:", allAttendants[0]);

                body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl;text-decoration:underline; text-align: right;font-family:AdvertisingMedium'>" + "السيد الرئيس:" + "</div>";
                body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + "بسم الله الرحمن الرحيم و الصلاة و السلام على رسول الله ، نتيجة لعدم اكتمال النصاب تأخر الجلسة لمدة نصف ساعة." + "</div>";
                body += "<br/>";
                body += "<br/>";
              
                string docStartNotOnTime = SessionStartFacade.madbatahStartNotOnTime.Replace("%sessionNum%", sessionNum).Replace("%GeorgianDate%", gDate).Replace("%sessionTime%", timeInHour).Replace("%hijriDate%", hijriDate).Replace("%President%", details.Presidnt);
                body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + docStartNotOnTime + "</div>";
                body += "<br/>";
                body += "<br/>";

                body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;text-decoration:underline;font-family:AdvertisingMedium'>" + "السيد الرئيس:" + "</div>";
                
                if (details.Date.DayOfWeek == DayOfWeek.Tuesday)
                    body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + SessionStartFacade.madbatahTuesdayIntro + "</div>";
                else
                    body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + SessionStartFacade.madbatahWednesdayIntro + "</div>";

                body += "<br/>";
                body += "<br/>";

                allAttendants = SessionStartFacade.GetSessionAttendantOrderedByStatus(details.SessionID, (int)SessionOpenStatus.NotOnTime);
                if (allAttendants.Count > 0)
                {
                    attendants = allAttendants[0];
                    attendantsWithinSession = allAttendants[1];
                    abologAttendants = allAttendants[2];
                    absenceAttendants = allAttendants[3];
                    inMissionAttendants = allAttendants[4];
                    body += writeAttendantNFile("* تليت بعد افتتاح الجلسة أسماء الأعضاء الحاضرين:", attendants);
                    body += writeAttendantNFile("* و فى أثناء الجلسة حضر كل من السادة الأعضاء:", attendantsWithinSession);
                    body += writeAttendantNFile("* الغائبون بعذر:", abologAttendants);
                    body += writeAttendantNFile("*الغائبون بدون اعتذار:", absenceAttendants);

                }
            }

            //Committee Attendance
            bool isCommittee = false;
            List<Committee> committeeLst = CommitteeHelper.GetAllCommittee();
            foreach (Committee comm in committeeLst)
            {
                List<List<DefaultAttendant>> allAtt = CommitteeHelper.GetSessionCommiteeAttendance(comm.ID, details.SessionID);
                if (allAtt[0].Count > 0 || allAtt[1].Count > 0 || allAtt[2].Count > 0)
                {
                    if (!isCommittee)
                    {
                        body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;text-decoration:underline;font-family:AdvertisingMedium'>" + "* أسماء السادة الأعضاء الغائبين بعذر أو من دون عذر عن حضور اجتماعات المجلس:" + "</div>";
                        isCommittee = true;
                    }
                    body+="<br/>";
                    body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;text-decoration:underline;font-family:AdvertisingMedium'>" + comm.CommitteeName + "</div>";
                    body += "<br/>";
                    body += writeAttendantNFile("* الغائبون بعذر:", allAtt[1], allAtt[2]);
                    body += writeAttendantNFile("*الغائبون بدون اعتذار:", allAtt[0] );
                }
            }

            string madbatahStart = "<html style=\"direction: rtl; font-family: AdvertisingMedium; font-size: 16pt; line-height: 20px;\">";
            madbatahStart += "<body dir=\"rtl\">";
            madbatahStart += sessionStart + body;// +"<br/>";
            madbatahStart += "</body></html>";
            return madbatahStart;
        }

        public static string writeAttendantNFile(string head, List<Attendant> attendants)
        {
            string body = "<div style=\"text-align:right;direction:rtl;font-family:AdvertisingMedium;font-size:16pt;line-height:30px; text-align: right;clear:both;\">";
            if (attendants.Count > 0)
            {
                body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl;text-decoration:underline; text-align: right;font-family:AdvertisingMedium'>" + head + "</div>";
                foreach (Attendant att in attendants)
                {
                    if (att.Name != "غير معرف")
                    {
                        body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + " - " + att.Name.Trim() + "</div>";
                        if (att.Type == (int)Model.AttendantType.GovernmentRepresentative)
                            body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + "        (" + att.JobTitle.Trim() + ")" + "</div>";
                    }
                }
                body += "<br/>";
                body += "<br/>";
            }
            return body;
        }

        public static string writeAttendantNFile(string head, List<DefaultAttendant> attendants)
        {
            string body = "<div style=\"text-align:right;direction:rtl;font-family:AdvertisingMedium;font-size:16pt;line-height:30px; text-align: right;clear:both;\">";
            string attStr = "";
            if (attendants.Count > 0)
            {
                body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl;text-decoration:underline; text-align: right;font-family:AdvertisingMedium'>" + head + "</div>";
                foreach (DefaultAttendant att in attendants)
                {
                    if (att.Name != "غير معرف")
                    {
                        attStr = " - " + att.Name.Trim();
                        body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + attStr + "</div>";
                        if (att.Type == (int)Model.AttendantType.GovernmentRepresentative)
                            body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + "        (" + att.JobTitle.Trim() + ")" + "</div>";
                    }
                }
                body += "<br/>";
            }
            return body;
        }

        public static string writeAttendantNFile(string head, List<DefaultAttendant> attendants1, List<DefaultAttendant> attendants2)
        {
            string body = "<div style=\"text-align:right;direction:rtl;font-family:AdvertisingMedium; text-align: right;font-size:16pt;line-height:30px;clear:both;\">";
            string attStr = "";
            if (attendants1.Count > 0 || attendants2.Count > 0)
            {
                body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl;text-decoration:underline; text-align: right;font-family:AdvertisingMedium'>" + head + "</div>";
                foreach (DefaultAttendant att in attendants2)
                {
                    if (att.Name != "غير معرف")
                    {
                        attStr = " - " + att.Name.Trim();
                        attStr += " (مهمة رسمية ) ";
                        body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + attStr + "</div>";
                        if (att.Type == (int)Model.AttendantType.GovernmentRepresentative)
                            body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + "        (" + att.JobTitle.Trim() + ")" + "</div>";
                    }
                }
                foreach (DefaultAttendant att in attendants1)
                {
                    if (att.Name != "غير معرف")
                    {
                        attStr = " - " + att.Name.Trim();
                        body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + attStr + "</div>";
                        if (att.Type == (int)Model.AttendantType.GovernmentRepresentative)
                            body += "<div style='font-weight:bold;font-size:16pt;line-height:30px;text-align:right;direction:rtl; text-align: right;font-family:AdvertisingMedium'>" + "        (" + att.JobTitle.Trim() + ")" + "</div>";
                    }
                }
                body += "<br/>";
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
