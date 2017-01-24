using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.Util;
using TayaIT.Enterprise.EMadbatah.Localization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Globalization;
using System.IO;
using TayaIT.Enterprise.EMadbatah.Pdf2Word;
using TayaIT.Enterprise.EMadbatah.OpenXml.Word;
using System.Xml.Linq;


namespace TayaIT.Enterprise.EMadbatah.BLL
{
    public class MabatahCreatorFacade
    {
        public static bool CreateMadbatah(long sessionID, string SessionWorkingDir, string ServerMapPath)
        {
            try
            {
                int coverSize = 1;
                List<MadbatahIndexItem> index = new List<MadbatahIndexItem>();
                List<SpeakersIndexItem> speakersIndex = new List<SpeakersIndexItem>();
                TayaIT.Enterprise.EMadbatah.Model.SessionDetails details = SessionStartFacade.GetSessionDetails(sessionID);
                //MadbatahCover
                CreateMadbatahCover(details, SessionWorkingDir + "MadbatahcoverDoc.docx", ServerMapPath);

                int sessionStartSize = 1;
                SessionFile start = SessionStartFacade.GetSessionStartBySessionID(sessionID);
                //HtmlToOpenXml.SaveHtmlToWord(start.SessionStartText, SessionWorkingDir + "sessionStartDoc.docx", ServerMapPath+ "\\resources\\", out sessionStartSize);
                HTMLtoDOCX hd = new HTMLtoDOCX();
                hd.CreateFileFromHTML(start.SessionStartText, SessionWorkingDir + "bodyDoc.docx");


                //Madbatah Body
                int bodySize = MabatahCreatorFacade.CreateMadbatahBody(sessionID, SessionWorkingDir + "bodyDoc.docx", SessionWorkingDir, ServerMapPath, details, out index, out speakersIndex);
                if (bodySize == -1)
                    throw new Exception("Madbatah Body Creation Failed.");

                //Madbatah Index
                int indexSize = 0;
                indexSize = MabatahCreatorFacade.CreateMadbatahIndex(index, SessionWorkingDir, indexSize, bodySize, SessionWorkingDir + "indexDoc.docx", ServerMapPath, details);
                if (indexSize == -1)
                    throw new Exception("Index with Attachment Creation Failed.");

                //Speakers Index
                int speakerSize = 0;
                speakerSize = MabatahCreatorFacade.CreateSpeakersIndex(speakersIndex, speakerSize, ServerMapPath, SessionWorkingDir + "indexSpeakers.docx");
                if (speakerSize == -1)
                    throw new Exception("Speakers Index Creation Failed.");

                MabatahCreatorFacade.CreateMadbatahIndex(index, SessionWorkingDir, indexSize + speakerSize + coverSize + sessionStartSize, bodySize, SessionWorkingDir + "indexDoc.docx", ServerMapPath, details);
                MabatahCreatorFacade.CreateSpeakersIndex(speakersIndex, indexSize + speakerSize + coverSize + sessionStartSize, ServerMapPath, SessionWorkingDir + "indexSpeakers.docx");

                //Merga All Generated Files
                List<string> mergeList = new List<string>();
                mergeList.Add(SessionWorkingDir + "indexDoc.docx");
                mergeList.Add(SessionWorkingDir + "indexSpeakers.docx");//done
                mergeList.Add(ServerMapPath + "\\docs\\templates\\MadbatahBodyCover.docx");//ready
                mergeList.Add(SessionWorkingDir + "bodyDoc.docx");
                mergeList.Add(ServerMapPath + "\\docs\\templates\\MadbatahEndCover.docx");//ready
                File.Copy(SessionWorkingDir + "MadbatahcoverDoc.docx", SessionWorkingDir + sessionID + ".docx", true);
                WordprocessingWorker.MergeWithAltChunk(SessionWorkingDir + sessionID + ".docx", mergeList.ToArray());

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatah(" + sessionID + "," + SessionWorkingDir + ")");
                return false;
            }
        }

        public static void CreateMadbatahCover(Model.SessionDetails details, string outCoverPath, string ServerMapPath)
        {
            File.Copy(ServerMapPath + "\\docs\\templates\\MadbatahStartCover.docx", outCoverPath, true);

            //calculate hijri date
            DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");

            string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
            string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month);
            string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
            string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;
            string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;

            //for header
            string sessionNum = details.Subject; //"الخامسة عشره";
            string hijriDate = dayNameAr + " " + dayOfMonthNumHij + " " + monthNameHijAr + " سنة " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
            string gDate = " " + details.Date.Day + " " + monthNameAr + " سنة " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";

            NumberingFormatter fomratterFemale = new NumberingFormatter(false);
            NumberingFormatter fomratterMale = new NumberingFormatter(true);
            string sessionName = details.EparlimentID.ToString() + " / " + details.Type;

            string timeInHour = "الساعة ";
            timeInHour += LocalHelper.GetLocalizedString("strHour" + details.StartTime.Hour);

            string timeInMin = "";
            if (details.StartTime.Minute != 0)
            {
                timeInMin = " و الدقيقة  ";
                timeInMin += fomratterFemale.getResultEnhanced(details.StartTime.Minute);
            }
            // )
            if (details.StartTime.ToString().IndexOf("AM") > 0)
                timeInMin += " صباحاً";

            StringBuilder sb = new StringBuilder();
            sb.Append("<root>");

            sb.Append("<Name>").Append(sessionName).Append("</Name>");
            sb.Append("<Season>").Append(SeasonHelper.GetSeasonByID(details.Season)).Append("</Season>");
            sb.Append("<StageType>").Append("ال" + details.StageType).Append("</StageType>");
            sb.Append("<Stage>").Append(StageHelper.GetStageByID(details.Stage)).Append("</Stage>");//fomratterMale.getResultEnhanced((int)details.Stage)
            sb.Append("<Subject>").Append(details.Subject).Append("</Subject>");
            sb.Append("<DateHijri>").Append(hijriDate).Append("</DateHijri>");
            sb.Append("<DateMilady>").Append(gDate).Append("</DateMilady>");
            sb.Append("<StartTime>").Append(timeInHour + timeInMin).Append("</StartTime>");
            //  sb.Append("<Footer>").Append(footer).Append("</Footer>");
            sb.Append("</root>");

            WordTemplateHandler.replaceCustomXML(outCoverPath, sb.ToString());
        }

          public static int CreateMadbatahBody(long sessionID, string outFilePath, string SessionWorkingDir, string ServerMapPath, Model.SessionDetails details, out List<MadbatahIndexItem> index, out List<SpeakersIndexItem> speakersIndex)
        {

            string docPath = outFilePath;


            string resfolderpath = ServerMapPath + "\\resources\\";
            DocXmlParts xmlFilesPaths = new DocXmlParts();
            xmlFilesPaths.CoreFilePropertiesPart = resfolderpath + "core.xml";
            xmlFilesPaths.EndNotes = resfolderpath + "endnotes.xml";
            xmlFilesPaths.FilePropPartPath = resfolderpath + "app.xml";
            xmlFilesPaths.FontTablePart = resfolderpath + "fontTable.xml";
            xmlFilesPaths.FooterPartPath = resfolderpath + "footer1.xml";
            xmlFilesPaths.FootnotesPart = resfolderpath + "footnotes.xml";
            xmlFilesPaths.NumberingDefinitionsPartPath = resfolderpath + "numbering.xml";
            xmlFilesPaths.SettingsPartPath = resfolderpath + "settings.xml";
            xmlFilesPaths.StylePartPath = resfolderpath + "styles.xml";
            xmlFilesPaths.ThemePartPath = resfolderpath + "theme1.xml";
            xmlFilesPaths.WebSettingsPartPath = resfolderpath + "webSettings.xml";
            xmlFilesPaths.HeaderPartPath = resfolderpath + "header.xml";
            try
            {
                WordprocessingWorker doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.Open, true);
                {
                    doc.DocHeaderString = "ــــــــــــــــــــ        " + details.EparlimentID.ToString() + " / " + details.Type + "        ــــــــــــــــــــ";
                    doc.ApplyHeaderAndFooter();

                    index = new List<MadbatahIndexItem>();
                    speakersIndex = new List<SpeakersIndexItem>();

                    List<SessionContentItem> allItems = SessionContentItemHelper.GetItemsBySessionIDOrderedBySessionFile(sessionID);

                    int pageNum = 0;
                    int ii = 1000;
                    int ctr = 0;
                    int j = 0;
                    int k = 0;

                    List<List<SessionContentItem>> speakerGroup = new List<List<SessionContentItem>>();
                    List<SessionContentItem> newGroup = GroupSpeakerSimilarArticles(allItems, out speakerGroup);
                    List<SessionContentItem> contentItemGrp = new List<SessionContentItem>();

                    foreach (SessionContentItem sessionItem in newGroup)
                    {
                        string text = "";
                        string contentItemAsText = "";
                        foreach (SessionContentItem contentItem in speakerGroup[j])
                        {
                            if (!doc.IsOpen)
                                doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.Open);


                            AgendaItem updatedAgenda = contentItem.AgendaItem;
                            if (updatedAgenda.Name != "غير معرف")
                            {
                                pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);
                                int ind = index.FindIndex(curIndexItem => curIndexItem.ID == updatedAgenda.ID);
                                if (ind == -1)
                                {
                                    index.Add(new MadbatahIndexItem(updatedAgenda.ID, updatedAgenda.Name, pageNum + "", true, "", "", false, int.Parse(updatedAgenda.IsIndexed.ToString()), false));
                                    if (updatedAgenda.IsIndexed == 1)
                                    {
                                        doc.AddParagraph(TextHelper.StripHTML(updatedAgenda.Name.Trim()), ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                                        doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                    }
                                }
                                else
                                    index[ind].PageNum += ", " + pageNum;
                            }

                            if (k == 0)
                            {
                                pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);
                                if (!sessionItem.MergedWithPrevious.Value)
                                {
                                    Attendant att = sessionItem.Attendant;
                                    if (att.Type != (int)Model.AttendantType.UnAssigned)
                                    {
                                        // WriteAttendantInWord(sessionItem, att, doc);

                                        if (att.Type == (int)Model.AttendantType.FromTheCouncilMembers)
                                        {
                                            int itemIndex = speakersIndex.IndexOf(new SpeakersIndexItem(MabatahCreatorFacade.GetAttendantTitleNSpeakersIndex(att, sessionID), pageNum.ToString(), att.Type));
                                            if (itemIndex == -1)
                                                speakersIndex.Add(new SpeakersIndexItem(MabatahCreatorFacade.GetAttendantTitleNSpeakersIndex(att, sessionID), pageNum.ToString() + ",", att.Type));
                                            else
                                                speakersIndex[itemIndex].PageNum += pageNum + ", ";
                                        }
                                    }
                                }
                            }

                            text += " " + contentItem.Text.ToLower().Trim();
                            contentItemAsText = text.ToLower().Replace("<br/>", "#!#!#!").Replace("<br>", "#!#!#!").Replace("<br >", "#!#!#!").Replace("<br />", "#!#!#!");
                            contentItemGrp.Add(contentItem);
                            k++;
                            if (contentItem.AttachementID != null)
                            {
                                string attachName = "";
                                Attachement attach = AttachmentHelper.GetAttachementByID(int.Parse(contentItem.AttachementID.ToString()));
                                attachName = "attach_" + attach.ID.ToString() + ".pdf";
                                System.IO.File.WriteAllBytes(SessionWorkingDir + attachName, attach.FileContent);
                                FileInfo fInfo = new FileInfo(SessionWorkingDir + attachName);

                                if (fInfo.Extension.ToLower().Equals(".pdf"))
                                {
                                    String pdfFilePath = SessionWorkingDir + attachName;
                                    pdf2ImageConvert.convertPdfFile(pdfFilePath);
                                    string wordAttFilePath = SessionWorkingDir + attachName.ToLower().Replace(".pdf", ".pdf.docx");
                                    string[] files = Directory.GetFiles(SessionWorkingDir + fInfo.Name.Replace(fInfo.Extension, "")).OrderBy(p => new FileInfo(p).CreationTime).ToArray();

                                    WriteParagraphInWord(sessionItem, contentItemAsText, doc, contentItemGrp);
                                    text = "";
                                    contentItemAsText = "";
                                    contentItemGrp.Clear();
                                    foreach (string f in files)
                                    {
                                        ImageWriter.AddImage(doc.DocMainPart.Document.Body, doc.DocMainPart, f, "rId" + ii);
                                        ii++;
                                    }
                                }
                            }
                            if (speakerGroup[j].Count == k)
                            {
                                WriteParagraphInWord(sessionItem, contentItemAsText, doc, contentItemGrp);
                                contentItemGrp.Clear();
                            }
                        }
                        k = 0;
                        text = "";
                        j++;
                        ctr++;
                        if (j == 0)
                        {
                            /*List<MembersVote> mems = MembersVoteHelper.GetMembersVoteNonSecretVoteID(155);
                            List<SessionMembersVote> membersVoteLst = new List<SessionMembersVote>();
                            foreach (MembersVote mem in mems)
                            {
                                SessionMembersVote s = new SessionMembersVote(mem.AutoID, mem.NonSecretVoteSubjectID, mem.PersonID,(int) mem.MemberVoteID, mem.MemberFullName);
                                membersVoteLst.Add(s);
                            }

                            doc.AddCustomTable(membersVoteLst);

                             doc.AddTable(new string[,] 
                                  { { "Texas", "1" }, 
                                  { "California", "2" }, 
                                  { "New York", "3" }, 
                                  { "Massachusetts", "4" } });*/
                        }
                    }///loop sessionitem

                    doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    doc.AddParagraph("الرئيس", ParagraphStyle.ParagraphTitle, ParagrapJustification.LTR, false, "");
                    doc.AddParagraph("الأمين العام", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                    doc.Save();
                    int num = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);//GetCurrentPageNumber();
                    doc.Dispose();
                    return num;
                }
            }
            catch (Exception ex)
            {
                index = new List<MadbatahIndexItem>();
                speakersIndex = new List<SpeakersIndexItem>();
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahBody(" + sessionID + "," + outFilePath + ")");
                return -1;
            }
        }

        
        public static int CreateMadbatahBody1(long sessionID, string outFilePath, string SessionWorkingDir, string ServerMapPath, Model.SessionDetails details, out List<MadbatahIndexItem> index, out List<SpeakersIndexItem> speakersIndex)
        {

            string docPath = outFilePath;


            string resfolderpath = ServerMapPath + "\\resources\\";
            DocXmlParts xmlFilesPaths = new DocXmlParts();
            xmlFilesPaths.CoreFilePropertiesPart = resfolderpath + "core.xml";
            xmlFilesPaths.EndNotes = resfolderpath + "endnotes.xml";
            xmlFilesPaths.FilePropPartPath = resfolderpath + "app.xml";
            xmlFilesPaths.FontTablePart = resfolderpath + "fontTable.xml";
            xmlFilesPaths.FooterPartPath = resfolderpath + "footer1.xml";
            xmlFilesPaths.FootnotesPart = resfolderpath + "footnotes.xml";
            xmlFilesPaths.NumberingDefinitionsPartPath = resfolderpath + "numbering.xml";
            xmlFilesPaths.SettingsPartPath = resfolderpath + "settings.xml";
            xmlFilesPaths.StylePartPath = resfolderpath + "styles.xml";
            xmlFilesPaths.ThemePartPath = resfolderpath + "theme1.xml";
            xmlFilesPaths.WebSettingsPartPath = resfolderpath + "webSettings.xml";
            xmlFilesPaths.HeaderPartPath = resfolderpath + "header.xml";
            try
            {
                WordprocessingWorker doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.Open, true);
                {
                    doc.DocHeaderString = "ــــــــــــــــــــ        " + details.EparlimentID.ToString() + " / " + details.Type + "        ــــــــــــــــــــ";
                    doc.ApplyHeaderAndFooter();

                   // doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                    index = new List<MadbatahIndexItem>();
                    speakersIndex = new List<SpeakersIndexItem>();
                    List<string> writtenAgendaItems = new List<string>();

                    List<List<SessionContentItem>> allItems = SessionContentItemHelper.GetItemsBySessionIDGroupedBySessionFile(sessionID);

                    SessionContentItem lastIteminPrevList = null;

                    string fileText = "";
                    string fileName = "";
                    SessionFile sf = new SessionFile();

                    int pageNum = 0;
                    int ii = 1000;
                    List<long> WrittenAgendaItemWithGroupedSubItems = new List<long>();
                    List<string> WrittenAgendaItemForNewOrder = new List<string>();
                    foreach (List<SessionContentItem> groupedItems in allItems)
                    {

                     //   pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);
                        int ctr = 0;

                        List<List<SessionContentItem>> speakerGroup = new List<List<SessionContentItem>>();
                        List<SessionContentItem> newGroup = GroupSpeakerSimilarArticles(groupedItems, out speakerGroup);
                        List<SessionContentItem> contentItemGrp = new List<SessionContentItem>();
                        //foreach (SessionContentItem sessionItem in groupedItems)
                        int j = 0;
                        int k = 0;
                        foreach (SessionContentItem sessionItem in newGroup)
                        {
                            //a check should be done of files, meshmesh
                            if (lastIteminPrevList != null && lastIteminPrevList.AttendantID == sessionItem.AttendantID)
                            {
                                sessionItem.MergedWithPrevious = true;
                            }
                            else
                                sessionItem.MergedWithPrevious = false;
                            lastIteminPrevList = sessionItem;

                            string text = "";
                            string contentItemAsText = "";
                            foreach (SessionContentItem contentItem in speakerGroup[j])
                            {
                                if (!doc.IsOpen)
                                    doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.Open);

                              
                                AgendaItem updatedAgenda = contentItem.AgendaItem;
                                if (updatedAgenda.Name != "غير معرف")
                                { 
                                    pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);
                                    int ind = index.FindIndex(curIndexItem => curIndexItem.ID == updatedAgenda.ID);
                                    if (ind == -1)
                                    {
                                        index.Add(new MadbatahIndexItem(updatedAgenda.ID, updatedAgenda.Name, pageNum + "", true, "", "", false, int.Parse(updatedAgenda.IsIndexed.ToString()), false));
                                        if (updatedAgenda.IsIndexed == 1)
                                        {
                                            doc.AddParagraph(TextHelper.StripHTML(updatedAgenda.Name.Trim()), ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                                            doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                        }
                                    }
                                    else
                                        index[ind].PageNum += ", " + pageNum;
                                }

                                if (k == 0)
                                {
                                    pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);//GetCurrentPageNumber();
                                    if (!sessionItem.MergedWithPrevious.Value)
                                    {
                                        Attendant att = sessionItem.Attendant;
                                        if (att.Type != (int)Model.AttendantType.UnAssigned)
                                        {
                                            // WriteAttendantInWord(sessionItem, att, doc);

                                            if (att.Type == (int)Model.AttendantType.FromTheCouncilMembers)
                                            {
                                                int itemIndex = speakersIndex.IndexOf(new SpeakersIndexItem(MabatahCreatorFacade.GetAttendantTitleNSpeakersIndex(att, sessionID), pageNum.ToString(), att.Type));
                                                if (itemIndex == -1)
                                                    speakersIndex.Add(new SpeakersIndexItem(MabatahCreatorFacade.GetAttendantTitleNSpeakersIndex(att, sessionID), pageNum.ToString() + ",", att.Type));
                                                else
                                                    speakersIndex[itemIndex].PageNum += pageNum + ", ";
                                            }
                                        }
                                    }
                                }

                                text += " " + contentItem.Text.ToLower().Trim();
                                contentItemAsText = text.ToLower().Replace("<br/>", "#!#!#!").Replace("<br>", "#!#!#!").Replace("<br >", "#!#!#!").Replace("<br />", "#!#!#!");
                                contentItemGrp.Add(contentItem);
                                k++;
                                if (contentItem.AttachementID != null)
                                {
                                    string attachName = "";
                                    Attachement attach = AttachmentHelper.GetAttachementByID(int.Parse(contentItem.AttachementID.ToString()));
                                    attachName = "attach_" + attach.ID.ToString() + ".pdf";
                                    System.IO.File.WriteAllBytes(SessionWorkingDir + attachName, attach.FileContent);
                                    FileInfo fInfo = new FileInfo(SessionWorkingDir + attachName);

                                    if (fInfo.Extension.ToLower().Equals(".pdf"))
                                    {
                                        String pdfFilePath = SessionWorkingDir + attachName;
                                        pdf2ImageConvert.convertPdfFile(pdfFilePath);
                                        string wordAttFilePath = SessionWorkingDir + attachName.ToLower().Replace(".pdf", ".pdf.docx");
                                        string[] files = Directory.GetFiles(SessionWorkingDir + fInfo.Name.Replace(fInfo.Extension, "")).OrderBy(p => new FileInfo(p).CreationTime).ToArray(); ;

                                        WriteParagraphInWord(sessionItem, contentItemAsText, doc, contentItemGrp);
                                        text = "";
                                        contentItemAsText = "";
                                        contentItemGrp.Clear();
                                        foreach (string f in files)
                                        {
                                            ImageWriter.AddImage(doc.DocMainPart.Document.Body, doc.DocMainPart, f, "rId" + ii);
                                            ii++;
                                        }
                                    }
                                }
                                if (speakerGroup[j].Count == k)
                                {
                                    WriteParagraphInWord(sessionItem, contentItemAsText, doc, contentItemGrp);
                                    contentItemGrp.Clear();
                                }
                            }
                            k = 0;
                            text = "";

                            if (j == 0)
                            {
                                /*List<MembersVote> mems = MembersVoteHelper.GetMembersVoteNonSecretVoteID(155);
                                List<SessionMembersVote> membersVoteLst = new List<SessionMembersVote>();
                                foreach (MembersVote mem in mems)
                                {
                                    SessionMembersVote s = new SessionMembersVote(mem.AutoID, mem.NonSecretVoteSubjectID, mem.PersonID,(int) mem.MemberVoteID, mem.MemberFullName);
                                    membersVoteLst.Add(s);
                                }

                                doc.AddCustomTable(membersVoteLst);

                                 doc.AddTable(new string[,] 
                                      { { "Texas", "1" }, 
                                      { "California", "2" }, 
                                      { "New York", "3" }, 
                                      { "Massachusetts", "4" } });*/
                            }
                            j++;

                            if (!string.IsNullOrEmpty(sessionItem.CommentOnText))
                            {
                                doc.AddParagraph("(" + sessionItem.CommentOnText + ")", ParagraphStyle.NormalArabic, ParagrapJustification.Center, false, "");
                            }
                            ctr++;
                            string contentItemAsHtml = sessionItem.Text.ToLower().Replace("<br/>", "#####").Replace("<br>", "#####");
                            string[] parts = contentItemAsHtml.Split(new string[] { "#####" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string part in parts)
                            {
                                string tt = TextHelper.StripHTML(part.ToLower()).Trim() + " ";

                                fileText = tt;

                                sf = SessionFileHelper.GetSessionFileByID((int)sessionItem.SessionFileID);

                                if (sf.FileError == 1)  // added by ihab
                                {
                                    try
                                    {
                                        string tempDir = ServerMapPath + "Corrupted files\\" + System.IO.Path.GetFileName(sf.Name.Replace(".mp3", ""));
                                        System.IO.Directory.CreateDirectory(tempDir);
                                        fileName = sf.Name.Split('\\')[3].Replace("mp3", "txt");
                                        FileStream fs = System.IO.File.Create(tempDir + "\\" + fileName);
                                        fs.Close();
                                        System.IO.File.WriteAllText(tempDir + "\\" + fileName, fileText);
                                        System.IO.File.Copy(ServerMapPath + sf.Name, tempDir + "\\" + System.IO.Path.GetFileName(sf.Name), true);
                                        System.IO.File.Copy(ServerMapPath + sf.Name.Replace("mp3", "trans.xml"), tempDir + "\\" + System.IO.Path.GetFileName(sf.Name.Replace("mp3", "trans.xml")), true);
                                    }
                                    catch { }
                                }
                            }

                        }///loop sessionitem
                    }


                    doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    doc.AddParagraph("الرئيس", ParagraphStyle.ParagraphTitle, ParagrapJustification.LTR, false, "");
                    doc.AddParagraph("الأمين العام", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                    doc.Save();
                    int num = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);//GetCurrentPageNumber();
                    doc.Dispose();
                    return num;
                }
            }
            catch (Exception ex)
            {
                index = new List<MadbatahIndexItem>();
                speakersIndex = new List<SpeakersIndexItem>();
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahBody(" + sessionID + "," + outFilePath + ")");
                return -1;
            }
        }
        
        public static void WriteAttendantInWord(SessionContentItem contentItem, Attendant att, WordprocessingWorker doc)
        {
            if (att.Type != (int)Model.AttendantType.UnAssigned)
            {
                string attFullPresentationName = "";
                if ((Model.AttendantType)att.Type == Model.AttendantType.President)
                {
                    doc.AddParagraph("السيد الرئيـــــــــــــــــــــــــــس :", ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                }
                else
                {
                    if (contentItem.IsSessionPresident == 1)
                    {
                        doc.AddParagraph("السيد رئيـس الجلســـــــــــــــــــــــة :", ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                        if (att.AttendantTitle == null)
                            attFullPresentationName = "السيد " + att.ShortName.Trim();
                        else attFullPresentationName = att.AttendantTitle.Trim() + " " + att.ShortName.Trim();
                        attFullPresentationName = "( " + attFullPresentationName;
                        if(String.IsNullOrEmpty(att.JobTitle))//if (att.Type != 3)
                            attFullPresentationName = attFullPresentationName + ")";
                        doc.AddParagraph(attFullPresentationName, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                        if (!String.IsNullOrEmpty(att.JobTitle))
                            doc.AddParagraph("    " + att.JobTitle + ")", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                        if (!String.IsNullOrEmpty(contentItem.CommentOnAttendant))
                            doc.AddParagraph("(" + contentItem.CommentOnAttendant.Trim() + " )", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    }
                    else
                    {
                        if (att.AttendantTitle == null)
                            attFullPresentationName = "السيد " + att.ShortName.Trim() + " : ";
                        else attFullPresentationName = att.AttendantTitle.Trim() + " " + att.ShortName.Trim() + " : ";
                        doc.AddParagraph(attFullPresentationName, ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                        if (!String.IsNullOrEmpty(att.JobTitle))
                            doc.AddParagraph("    (" + att.JobTitle + ")", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                        if (!String.IsNullOrEmpty(contentItem.CommentOnAttendant))
                            doc.AddParagraph("    (" + contentItem.CommentOnAttendant.Trim() + ")", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    }
                }
            }
        }

        public static void WriteParagraphInWord(SessionContentItem sessionItem, string contentItemAsText, WordprocessingWorker doc, List<SessionContentItem> grp, bool ttt)
        {
            string[] p = new string[] { };
            string[] paragraphs = GetParagraphsArr(contentItemAsText, out p);
            string[] procedureArr = new string[] { };
            string parag = "";
            List<string> myCollection = new List<string>();
            List<string> myCollection2 = new List<string>();
            for (int pp = 0; pp < paragraphs.Length; pp++)
            {
                parag = TextHelper.StripHTML(paragraphs[pp].Replace("#!#!#!", " ").ToLower()).Trim();
                if (parag != "")
                {
                    myCollection.Add(parag.Replace("&nbsp;", " "));
                    myCollection2.Add(sessionItem.PageFooter);
                    if (pp != 0)
                        WriteAttendantInWord(sessionItem, sessionItem.Attendant, doc);
                    if (sessionItem.PageFooter != "")
                        doc.AddParagraph(myCollection, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, true, myCollection2);
                    else doc.AddParagraph(parag.Replace("&nbsp;", " "), ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                }


                if (pp < p.Length)
                {
                    parag = TextHelper.StripHTML(p[pp].ToString()).Trim();
                    if (parag != "")
                    {
                        string[] sep = new string[1] { "#!#!#!" };
                        procedureArr = parag.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        ParagrapJustification align = ParagrapJustification.RTL;
                        if (p[pp].ToString().IndexOf("center") > 0)
                            align = ParagrapJustification.Center;

                        for (int x = 0; x < procedureArr.Length; x++)
                        {
                            doc.AddParagraph(procedureArr[x].Replace("&nbsp;", " "), ParagraphStyle.ParagraphTitle, align, false, "");
                        }

                        doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    }
                }
            }
        }

        public static void WriteParagraphInWord(SessionContentItem sessionItem, string contentItemAsText, WordprocessingWorker doc, List<SessionContentItem> grp)
        {
            try
            {
                List<string> myCollection = new List<string>();
                List<string> myCollection2 = new List<string>();
                string fullparag = "";
                string parag = "";
                string proc = "";
                for (int i = 0; i < grp.Count; i++)
                {
                    fullparag = TextHelper.StripHTML(grp[i].Text.Replace("#!#!#!", " ").ToLower()).Trim();
                    string[] p = new string[] { };
                    string[] paragraphs = GetParagraphsArr(grp[i].Text.ToLower().Replace("<br/>", "#!#!#!").Replace("<br>", "#!#!#!").Replace("<br >", "#!#!#!").Replace("<br />", "#!#!#!"), out p);
                    string[] procedureArr = new string[] { };

                    if (fullparag != "")
                    {
                        myCollection2.Add(grp[i].PageFooter);
                      
                        for (int pp = 0; pp < paragraphs.Length; pp++)
                        {
                            parag += TextHelper.StripHTML(paragraphs[pp].Replace("#!#!#!", " ").ToLower()).Trim().Replace("&nbsp;", " ");
                            myCollection.Add(TextHelper.StripHTML(paragraphs[pp].Replace("#!#!#!", " ").ToLower()).Trim().Replace("&nbsp;", " "));
                            if (paragraphs.Length != 1 && pp < p.Length)
                            {
                                if (parag.Trim() != "")
                                {
                                    WriteAttendantInWord(sessionItem, sessionItem.Attendant, doc);
                                    doc.AddParagraph(myCollection, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, true, myCollection2);
                                    doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                    myCollection.Clear();
                                    myCollection2.Clear();
                                    parag = "";
                                }
                            }



                            if (pp < p.Length)
                            {
                                proc = TextHelper.StripHTML(p[pp].ToString()).Trim();
                                if (proc != "")
                                {
                                    string[] sep = new string[1] { "#!#!#!" };
                                    procedureArr = proc.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                                    ParagrapJustification align = ParagrapJustification.RTL;
                                    if (p[pp].ToString().IndexOf("center") > 0)
                                        align = ParagrapJustification.Center;

                                    for (int x = 0; x < procedureArr.Length; x++)
                                    {
                                        doc.AddParagraph(procedureArr[x].Replace("&nbsp;", " "), ParagraphStyle.ParagraphTitle, align, false, "");
                                    }

                                    doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                }
                            }
                        }
                    }
                }
                if (myCollection.Count > 0)
                {
                    string lastparag = "";
                    for (int x = 0; x < myCollection.Count; x++)
                    {
                        lastparag += myCollection[x].Replace("&nbsp;", " ");
                    }
                    if (lastparag.Trim() != "")
                    {
                        WriteAttendantInWord(sessionItem, sessionItem.Attendant, doc);
                        doc.AddParagraph(myCollection, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, true, myCollection2);
                        doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    }
                    myCollection.Clear();
                    myCollection2.Clear();
                }
            }
            catch (Exception e)
            {
            }
        }

        public static string[] GetParagraphsArr(string parag, out string[] p)
        {
            parag = ClearParagFromEmptyProc(parag);
            MatchCollection matches1, matches2;
            string pattern1 = @"<p (.*?) procedure-id[^>]*?>(.*?)</p>";
            matches1 = Regex.Matches(parag, pattern1);
            string pattern2 = @"<p procedure-id[^>]*?>(.*?)</p>";
            matches2 = Regex.Matches(parag, pattern2);

            p = new string[matches1.Count + matches2.Count];

            for (int i = 0; i < matches1.Count; i++)
            {
                parag = parag.Replace(matches1[i].ToString(), "<%#####%>");
                p[i] = matches1[i].ToString();
            }

            for (int i = 0; i < matches2.Count; i++)
            {
                parag = parag.Replace(matches2[i].ToString(), "<%#####%>");
                p[i + matches1.Count] = matches2[i].ToString();
            }

            string[] sep = new string[1] { "<%#####%>" };
            return parag.Split(sep, StringSplitOptions.None);

        }

        public static string ClearParagFromEmptyProc(string parag)
        {
            MatchCollection matches1, matches2;
            string pattern1 = @"<p (.*?) procedure-id[^>]*?>(.*?)</p>";
            matches1 = Regex.Matches(parag, pattern1);
            string pattern2 = @"<p procedure-id[^>]*?>(.*?)</p>";
            matches2 = Regex.Matches(parag, pattern2);
            string procStr = "";
            string tempstr = "";

            for (int i = 0; i < matches1.Count; i++)
            {
                procStr = matches1[i].ToString();
                tempstr = TextHelper.StripHTML(procStr.Replace("&nbsp;", "")).Trim();
                if (string.IsNullOrEmpty(tempstr))
                    parag = parag.Replace(procStr, "");
            }

            for (int i = 0; i < matches2.Count; i++)
            {

                procStr = matches2[i].ToString();
                tempstr = TextHelper.StripHTML(procStr.Replace("&nbsp;", "")).Trim();
                if (string.IsNullOrEmpty(tempstr))
                    parag = parag.Replace(procStr, "");
            }
            return parag;
        }

        public static string tblBorder = "border:2px solid #000;";
        public static string pRightDefStyle = SessionStartFacade.marginZeroStyle + SessionStartFacade.defBoldFont + SessionStartFacade.defFontSize + SessionStartFacade.textRight;
        public static string pCenterDefStyle = SessionStartFacade.marginZeroStyle + SessionStartFacade.defBoldFont + SessionStartFacade.defFontSize + SessionStartFacade.textCenter;
        public static string pUnderLineCenterDefStyle = SessionStartFacade.marginZeroStyle + SessionStartFacade.defBoldFont + SessionStartFacade.defFontSize + SessionStartFacade.textCenter + SessionStartFacade.textunderline;
        public static string emptyParag = "<p style='" + SessionStartFacade.marginZeroStyle + "'>&nbsp;</p>";
        public static int CreateMadbatahIndex(List<MadbatahIndexItem> index, string folderPath, int indexSize, int bodySize, string outIndexPath, string ServerMapPath, Model.SessionDetails details)
        {
            try
            {
                //calculate hijri date
                DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");
                string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
                string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month);
                string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
                string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;
                string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;


                //for header
                string sessionNum = details.Subject; //"الخامسة عشره";
                string hijriDate = dayNameAr + " " + dayOfMonthNumHij + " من " + monthNameHijAr + " سنة " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
                string gDate = " " + details.Date.Day + " من " + monthNameAr + " سنة " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";
                string sessionName = details.Subject + " رقم (" + details.Type + ")";

                string indexHeader = @"<table width='100%' border='0' align='right' style='writing-mode: tb-rl; " + SessionStartFacade.lineHeight + SessionStartFacade.textRight + SessionStartFacade.directionStyle + SessionStartFacade.defBoldFont + "'>"
                  + " <tr>"
                  + "  <p style='" + pCenterDefStyle + "'>بسم الله الرحمن الرحيم</p>"
                  + "  <p style='" + pCenterDefStyle + "'>مجلس الأمة</p>"
                  + "  <p style='" + pCenterDefStyle + "'>الأمانة العامة</p>"
                  + "  <p style='" + pCenterDefStyle + "'>ادارة شؤون المضابط</p>"
                  + emptyParag
                  + "<p style='" + pUnderLineCenterDefStyle + "'>ملخص الموضوعات التى نظرت فى  " + sessionName + " </p>"
                  + "<p style='" + pUnderLineCenterDefStyle + "'>المعقودة يوم  " + hijriDate + "</p>"
                  + "<p style='" + pUnderLineCenterDefStyle + "'>الموافق  " + gDate + "</p>"
                  + emptyParag
                  + "<p style='" + pCenterDefStyle + "'>فهرس الموضوعات</p>"
                  + " </tr>"
                  + " <tr>"
                  + "  <table border='1' style='width:100%;border-collapse:collapse; " + tblBorder + SessionStartFacade.lineHeight + SessionStartFacade.textCenter + SessionStartFacade.directionStyle + SessionStartFacade.defBoldFont + SessionStartFacade.defFontSize + SessionStartFacade.marginZeroStyle + "' align='center' cellpadding='5' cellspacing='0'>"
                  + " <tr style='" + SessionStartFacade.pagebreak + "'>"
                  + "   <th style='" + tblBorder + "'><p style='" + SessionStartFacade.marginZeroStyle + "'>م</p></th>"
                  + "   <th style='" + tblBorder + "'><p style='" + SessionStartFacade.marginZeroStyle + "'>الموضوع</p></th>"
                  + "   <th style='" + tblBorder + "'><p style='" + SessionStartFacade.marginZeroStyle + "'>الصفحات</p></th>"
                  + " </tr>";


                int i = 1, j = 1;
                string indx = "";
                string emptyRowBold = "<tr style='" + SessionStartFacade.pagebreak + "'>" +
                                          "<td style='" + tblBorder + "'><p style='" + pCenterDefStyle + "'>ItemNum</p></td>" +
                                          "<td style='" + tblBorder + "'><p style='font-family: UsedFont;" + SessionStartFacade.marginZeroStyle + SessionStartFacade.textRight + SessionStartFacade.defFontSize + "'>ItemName</p></td>" +
                                          "<td style='" + tblBorder + "'><p style='" + pCenterDefStyle + "'>PageNum</p></td>" +
                                          "</tr>";
                StringBuilder sb = new StringBuilder();
                sb.Append(indexHeader);

                string toBeReplaced = emptyRowBold.Replace("ItemNum", "1").Replace("ItemName", "- أسماء السادة الأعضاء").Replace("PageNum", (indexSize + 1).ToString()).Replace("UsedFont", "AdvertisingBold");
                sb.Append(toBeReplaced);

                foreach (MadbatahIndexItem item in index)
                {
                    string font = "AdvertisingBold";
                    string name = "";
                    string pageNum = "";
                    i++;
                    name = item.Name;
                    pageNum = item.PageNum.ToString();
                    string[] pageNums = pageNum.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    pageNum = (int.Parse(pageNums[0]) + indexSize).ToString();
                    if (item.IsIndexed == 1)
                    {
                        j++;
                        indx = j.ToString();
                    }
                    else
                    {
                        indx = "";
                        font = "AdvertisingMedium";
                    }
                    toBeReplaced = emptyRowBold.Replace("ItemNum", indx).Replace("ItemName", " - " + name).Replace("PageNum", pageNum).Replace("UsedFont", font);
                    sb.Append(toBeReplaced);
                }

                DocXmlParts xmlFilesPaths = WordprocessingWorker.GetDocParts(ServerMapPath + "\\resources\\");
                sb.Append("</table> </tr></table>");

                int stats = 0;

                HTMLtoDOCX hd = new HTMLtoDOCX();
                hd.CreateFileFromHTML(sb.ToString(), @outIndexPath);

                using (WordprocessingWorker doc = new WordprocessingWorker(outIndexPath, xmlFilesPaths, DocFileOperation.Open))
                {
                    WordprocessingWorker doctmp = doc;
                    stats = doc.CountPagesUsingOpenXML(doc, outIndexPath, xmlFilesPaths, ServerMapPath, out doctmp);//CountPagesUsingOpenXML(DocumentType.DOCX, folderPath + att.Name);
                    doctmp.Dispose();
                }

                return stats;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahIndexWithAttachment");
                return -1;
            }
        }

        public static int CreateSpeakersIndex(List<SpeakersIndexItem> index, int increment, string ServerMapPath, string outPath)
        {
            try
            {
                string indexHeader = @"<p style='" + pUnderLineCenterDefStyle + SessionStartFacade.lineHeight + "'>فهـــــرس المتحــــــدثــين</p>"
                    + "<p style='" + SessionStartFacade.defBoldFont + SessionStartFacade.lineHeight + SessionStartFacade.textCenter + " font-size: 12pt;'>(السادة الاعضاء المتحدثون على الموضوعات التى تم مناقشتها أثناء انعقاد الجلسة)</p>"
                    + emptyParag
                    + "<table border='1' style='width:100%; border-collapse:collapse;" + tblBorder + SessionStartFacade.lineHeight + SessionStartFacade.textCenter + SessionStartFacade.directionStyle + SessionStartFacade.defBoldFont + SessionStartFacade.defFontSize + SessionStartFacade.marginZeroStyle + "' align='center' cellpadding='3' cellspacing='0'>"
                    + "  <tr style='" + SessionStartFacade.pagebreak + "'>"
                    + "   <th style='" + tblBorder + "width:80%'><p style='" + pCenterDefStyle + "'>اسم المتحدث</p></th>"
                    + "   <th style='" + tblBorder + "'><p style='" + pCenterDefStyle + "'>رقم الصفحة</p></th>"
                    + " </tr>";


                int i = 1;
                string emptyRowBold = @"<tr style='" + SessionStartFacade.pagebreak + "'>" +
                            "<td style='" + SessionStartFacade.textRight + tblBorder + ";width:80%'><p style='" + pRightDefStyle + "'>ItemName</p></td>" +
                            "<td style='" + tblBorder + "'><p style='" + pCenterDefStyle + "'>PageNum</p></td></tr>";
                StringBuilder sb = new StringBuilder();
                sb.Append(indexHeader);

                foreach (SpeakersIndexItem item in index)
                {

                    string[] parts = item.PageNum.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> pages = new List<string>();
                    string pagesStr = "";
                    foreach (string part in parts)
                    {
                        if (pages.IndexOf(part) == -1)
                        {
                            pagesStr += (int.Parse(part) + increment) + ", ";
                            pages.Add(part);
                        }
                    }
                    pages = pages.Distinct().ToList();
                   
                    if (pagesStr.Length > 2)
                        pagesStr = pagesStr.Remove(pagesStr.Length - 2);
                    sb.Append(emptyRowBold.Replace("ItemName", item.Name.Trim()).Replace("PageNum", pagesStr));
                    i++;
                }
                sb.Append("</table>");

                int stats = 0;
                HTMLtoDOCX hd = new HTMLtoDOCX();
                hd.CreateFileFromHTML(sb.ToString(), outPath);

                DocXmlParts xmlFilesPaths = WordprocessingWorker.GetDocParts(ServerMapPath + "\\resources\\");

                using (WordprocessingWorker doc = new WordprocessingWorker(outPath, xmlFilesPaths, DocFileOperation.Open))
                {
                    WordprocessingWorker doctmp = doc;
                    stats = doc.CountPagesUsingOpenXML(doc, outPath, xmlFilesPaths, ServerMapPath, out doctmp);//CountPagesUsingOpenXML(DocumentType.DOCX, folderPath + att.Name);
                    doctmp.Dispose();
                }
                return stats;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahIndex(" + ")");
                return -1;
            }
        }

        /*
        public static int CreateMadbatahIndex(List<MadbatahIndexItem> index, string folderPath, int indexSize, int bodySize, string outIndexPath, string ServerMapPath, Model.SessionDetails details)
        {
            try
            {
                //calculate hijri date
                DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");
                string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
                string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month);
                string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
                string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;
                string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;


                //for header
                string sessionNum = details.Subject; //"الخامسة عشره";
                string hijriDate = dayNameAr + " " + dayOfMonthNumHij + " من " + monthNameHijAr + " سنة " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
                string gDate = " " + details.Date.Day + " من " + monthNameAr + " سنة " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";
                string sessionName = details.Subject + " رقم (" + details.Type + ")";

                string indexHeader = @"<table width='100%' border='0' align='right' style='writing-mode: tb-rl; text-align:right; direction: rtl; font-family: AdvertisingBold; font-size: 16pt;'>
                   <tr>
                    <p style='text-align:center; font-family: AdvertisingBold; font-size: 14pt;'>بسم الله الرحمن الرحيم</p>
                    <p style='text-align:center; font-family: AdvertisingBold; font-size: 14pt;'>مجلس الأمة</p>
                    <p style='text-align:center; font-family: AdvertisingBold; font-size: 14pt;'>الأمانة العامة</p>
                    <p style='text-align:center; font-family: AdvertisingBold; font-size: 14pt;'>ادارة شؤون المضابط</p>
                    <br/>
                    <p style='text-align:center;text-decoration:underline; font-family: AdvertisingBold; font-size: 14pt; direction:rtl;'>ملخص الموضوعات التى نظرت فى  " + sessionName + " </p>"
                  + "<p style='text-align:center;text-decoration:underline; font-family: AdvertisingBold; font-size: 14pt; direction:rtl;'>المعقودة يوم  " + hijriDate + "</p>"
                  + "<p style='text-align:center;text-decoration:underline; font-family: AdvertisingBold; font-size: 14pt; direction:rtl;'>الموافق  " + gDate + "</p>"
                  + "<br/>"
                  + "<div style='text-align:center; font-family: AdvertisingBold; font-size: 14pt;'>فهرس الموضوعات</div>"
                  + "<br/>"
                  + " </tr><tr><table border='1' style='width:100%; border:2px solid #000; border-collapse:collapse; text-align:center; direction: rtl; font-family: AdvertisingBold; font-size: 14pt;margin-top: 0em;margin-bottom: 0em;' align='center' cellpadding='5' cellspacing='0'>"
                  + " <tr style='page-break-inside:avoid;'>"
                  + "   <th style='border:2px solid #000'>م</p></th>"
                  + "   <th style='border:2px solid #000'>الموضوع</p></th>"
                  + "   <th style='border:2px solid #000'>الصفحات</p></th>"
                  + " </tr>";


                int i = 1, j = 1;
                string indx = "";
                string emptyRowBold = "<tr style='page-break-inside:avoid;'><td style='border:2px solid #000;font-family: AdvertisingBold;font-size: 14pt;'><p style=';margin-top: 0em;margin-bottom: 0em;'>ItemNum</p></td>" +
                                          "<td style='text-align:right;border:2px solid #000;font-family: UsedFont;font-size: 14pt !important;'><p style='width:200px'>ItemName</p></td>"+
                                          "<td style='border:2px solid #000;font-family: AdvertisingBold;font-size: 14pt;'><p style=';margin-top: 0em;margin-bottom: 0em;'>PageNum</p></td></tr>";
                StringBuilder sb = new StringBuilder();
                sb.Append(indexHeader);

                string toBeReplaced = emptyRowBold.Replace("ItemNum", "1").Replace("ItemName", "- أسماء السادة الأعضاء").Replace("PageNum", (indexSize + 1).ToString()).Replace("UsedFont", "AdvertisingBold");
                sb.Append(toBeReplaced);

                foreach (MadbatahIndexItem item in index)
                {
                    string font = "AdvertisingBold";
                    string name = "";
                    string pageNum = "";
                    i++;
                    name = item.Name;
                    pageNum = item.PageNum.ToString();
                    string[] pageNums = pageNum.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    pageNum = (int.Parse(pageNums[0]) + indexSize).ToString();
                    if (item.IsIndexed == 1)
                    {
                        j++;
                        indx = j.ToString();
                    }
                    else
                    {
                        indx = "";
                        font = "AdvertisingMedium";
                    }
                    toBeReplaced = emptyRowBold.Replace("ItemNum", indx).Replace("ItemName", " - " + name).Replace("PageNum", pageNum).Replace("UsedFont", font);
                    sb.Append(toBeReplaced);
                }

                DocXmlParts xmlFilesPaths = WordprocessingWorker.GetDocParts(ServerMapPath + "\\resources\\");
                sb.Append("</table> </tr></table>");

                int stats = 0;

                HTMLtoDOCX hd = new HTMLtoDOCX();
                hd.CreateFileFromHTML(sb.ToString(), @outIndexPath);

                using (WordprocessingWorker doc = new WordprocessingWorker(outIndexPath, xmlFilesPaths, DocFileOperation.Open))
                {
                    WordprocessingWorker doctmp = doc;
                    stats = doc.CountPagesUsingOpenXML(doc, outIndexPath, xmlFilesPaths, ServerMapPath, out doctmp);//CountPagesUsingOpenXML(DocumentType.DOCX, folderPath + att.Name);
                    doctmp.Dispose();
                }

                return stats;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahIndexWithAttachment");
                return -1;
            }
        }

        public static int CreateSpeakersIndex(List<SpeakersIndexItem> index, int increment, string ServerMapPath, string outPath)
        {
            try
            {
                string indexHeader = @"<p style='text-align:center;text-decoration:underline; font-family: AdvertisingBold; font-size: 14pt; '>فهـــــرس المتحــــــدثــين</p>
                    <p style='text-align:center; font-family: AdvertisingBold; font-size: 12pt; f'>(السادة الاعضاء المتحدثون على الموضوعات التى تم مناقشتها أثناء انعقاد الجلسة)</p>
                    <br><br>
                    <table border='1' style='width:100%; border:2px solid #000; border-collapse:collapse; text-align:center; direction: rtl; font-family: AdvertisingBold; font-size: 14pt;margin-top: 0em;margin-bottom: 0em;' align='center' cellpadding='3' cellspacing='0'>
                        <tr  style='page-break-inside:avoid;'>
                            <th style='border:2px solid #000;width:80%'><p style='font-family: AdvertisingBold; font-size: 14pt;;margin-top: 0em;margin-bottom: 0em;'>اسم المتحدث</p></th>
                            <th style='border:2px solid #000'><p style='font-family: AdvertisingBold; font-size: 14pt;;margin-top: 0em;margin-bottom: 0em;'>رقم الصفحة</p></th>
                        </tr>";


                int i = 1;
                string emptyRowBold = @"<tr  style='page-break-inside:avoid;'>
                            <td style='text-align:right;border:2px solid #000;;width:80%'><p style='font-family: AdvertisingBold; font-size: 14pt;text-align:right ;margin-top: 0em;margin-bottom: 0em;'>ItemName</p></td>
                            <td style='border:2px solid #000'><p style='font-family: AdvertisingBold; font-size: 14pt; text-align:center;margin-top: 0em;margin-bottom: 0em;'>PageNum</p></td></tr>";
                StringBuilder sb = new StringBuilder();
                sb.Append(indexHeader);

                foreach (SpeakersIndexItem item in index)
                {

                    string[] parts = item.PageNum.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                 
                    List<string> pages = new List<string>();
                    string pagesStr = "";
                    foreach (string part in parts)
                    {
                        if (pages.IndexOf(part) == -1)
                        {
                            pagesStr += (int.Parse(part) + increment) + ", ";
                            pages.Add(part);
                        }
                    }

                    pages = pages.Distinct().ToList();
                    if (pagesStr.Length > 2)
                        pagesStr = pagesStr.Remove(pagesStr.Length - 2);
                    sb.Append(emptyRowBold.Replace("ItemName", item.Name.Trim()).Replace("PageNum", pagesStr));
                    i++;
                }
                sb.Append("</table>");

                int stats = 0;
                HTMLtoDOCX hd = new HTMLtoDOCX();
                hd.CreateFileFromHTML(sb.ToString(), outPath);

                DocXmlParts xmlFilesPaths = WordprocessingWorker.GetDocParts(ServerMapPath + "\\resources\\");

                using (WordprocessingWorker doc = new WordprocessingWorker(outPath, xmlFilesPaths, DocFileOperation.Open))
                {
                    WordprocessingWorker doctmp = doc;
                    stats = doc.CountPagesUsingOpenXML(doc, outPath, xmlFilesPaths, ServerMapPath, out doctmp);//CountPagesUsingOpenXML(DocumentType.DOCX, folderPath + att.Name);
                    doctmp.Dispose();
                }
                return stats;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahIndex(" + ")");
                return -1;
            }
        }
        */
        public static List<SessionContentItem> GroupSpeakerSimilarArticles(List<SessionContentItem> groupedItems, out List<List<SessionContentItem>> speakerContentItem)
        {
            List<SessionContentItem> newGroup = new List<SessionContentItem>();
            List<SessionContentItem> newSpeaker = new List<SessionContentItem>();
            List<List<SessionContentItem>> speakerSessionContentItem = new List<List<SessionContentItem>>();
            speakerContentItem = new List<List<SessionContentItem>>();
            int i = 0;
            long speakerID = 0, nextSpeakerID = 0;
            foreach (SessionContentItem item in groupedItems)
            {
                if (newGroup.Count == 0)
                {
                    item.PageFooter = item.PageFooter == null ? "" : item.PageFooter;
                    newGroup.Add(item);
                    newSpeaker.Add(item);
                    speakerContentItem.Add(newSpeaker);
                    speakerID = item.AttendantID;
                    nextSpeakerID = item.AttendantID;
                }
                else
                {
                    nextSpeakerID = item.AttendantID;
                    if (nextSpeakerID == speakerID)
                    {
                        newSpeaker.Add(item);
                        speakerContentItem[i] = newSpeaker;

                        //  newGroup[newGroup.Count - 1].Text += " " + item.Text;

                        if (!string.IsNullOrEmpty(item.CommentOnAttendant)
                            && !string.IsNullOrEmpty(newGroup[newGroup.Count - 1].CommentOnAttendant)
                            && !newGroup[newGroup.Count - 1].CommentOnAttendant.Contains(item.CommentOnAttendant))
                            newGroup[newGroup.Count - 1].CommentOnAttendant += " - " + item.CommentOnAttendant;

                        if (!string.IsNullOrEmpty(item.CommentOnText)
                            && !string.IsNullOrEmpty(newGroup[newGroup.Count - 1].CommentOnText)
                            && !newGroup[newGroup.Count - 1].CommentOnText.Contains(item.CommentOnText))
                            newGroup[newGroup.Count - 1].CommentOnText += " - " + item.CommentOnText;

                        /*   if (!string.IsNullOrEmpty(item.PageFooter)
                               && !string.IsNullOrEmpty(newGroup[newGroup.Count - 1].PageFooter)
                               && !newGroup[newGroup.Count - 1].PageFooter.Contains(item.PageFooter))
                               newGroup[newGroup.Count - 1].PageFooter += " - " + item.PageFooter;*/
                    }
                    else
                    {
                        newSpeaker = new List<SessionContentItem>();
                        item.PageFooter = item.PageFooter == null ? "" : item.PageFooter;
                        newSpeaker.Add(item);
                        speakerContentItem.Add(newSpeaker);
                        newGroup.Add(item);
                        speakerID = item.AttendantID;
                        nextSpeakerID = item.AttendantID;
                        i++;
                    }
                }
            }
            return newGroup;
        }

        public static List<SessionContentItem> GroupSpeakerSimilarArticles(List<SessionContentItem> groupedItems)
        {
            List<SessionContentItem> newGroup = new List<SessionContentItem>();

            foreach (SessionContentItem item in groupedItems)
            {
                if (newGroup.Count == 0)

                    newGroup.Add(item);

                else
                    if (item.MergedWithPrevious == true)
                    {
                        newGroup[newGroup.Count - 1].Text += " " + item.Text;

                        if (!string.IsNullOrEmpty(item.CommentOnAttendant)
                            && !string.IsNullOrEmpty(newGroup[newGroup.Count - 1].CommentOnAttendant)
                            && !newGroup[newGroup.Count - 1].CommentOnAttendant.Contains(item.CommentOnAttendant))
                            newGroup[newGroup.Count - 1].CommentOnAttendant += " - " + item.CommentOnAttendant;

                        if (!string.IsNullOrEmpty(item.CommentOnText)
                            && !string.IsNullOrEmpty(newGroup[newGroup.Count - 1].CommentOnText)
                            && !newGroup[newGroup.Count - 1].CommentOnText.Contains(item.CommentOnText))
                            newGroup[newGroup.Count - 1].CommentOnText += " - " + item.CommentOnText;

                        if (!string.IsNullOrEmpty(item.PageFooter)
                            && !string.IsNullOrEmpty(newGroup[newGroup.Count - 1].PageFooter)
                            && !newGroup[newGroup.Count - 1].PageFooter.Contains(item.PageFooter))
                            newGroup[newGroup.Count - 1].PageFooter += " - " + item.PageFooter;



                    }
                    else

                        newGroup.Add(item);
            }
            return newGroup;
        }

        public static string GetAttendantTitle(Object SomeObj)
        {
            Attendant att = null;
            if (SomeObj is SessionAttendant)
            {
                SessionAttendant sAtt = (SessionAttendant)SomeObj;
                att = new Attendant()
                {
                    ID = sAtt.ID,
                    Name = sAtt.Name,
                    FirstName = sAtt.FirstName,
                    SecondName = sAtt.SecondName,
                    TribeName = sAtt.TribeName,
                    Type = (int)sAtt.Type
                };
            }
            else
                att = (Attendant)SomeObj;

            string attName = "";
            if (string.IsNullOrEmpty(att.FirstName) && string.IsNullOrEmpty(att.SecondName) && string.IsNullOrEmpty(att.TribeName))
                attName = att.Name.Trim();
            else
                attName = ((att.FirstName == null ? "" : att.FirstName) + " " + (att.SecondName == null ? "" : att.SecondName) + " " + (att.TribeName == null ? "" : att.TribeName)).Replace("   ", " ").Replace("  ", " ").Trim();
            int? type = att.Type;

            string title = "";
            if (attName.Trim().StartsWith("سعادة") || attName.Trim().StartsWith("سعاده") ||
                            attName.Trim().StartsWith("معالي") || attName.Trim().StartsWith("معالى"))
                title = "";
            //return "";
            else
                if (type != null)
                {
                    //string title = "";
                    Model.AttendantType attType = (Model.AttendantType)type.Value;
                    switch (attType)
                    {
                        case Model.AttendantType.FromOutsideTheCouncil:
                            title = "سعادة/ ";
                            break;
                        case Model.AttendantType.FromTheCouncilMembers:
                            title = "سعادة/ ";
                            break;
                        case Model.AttendantType.GovernmentRepresentative:

                            title = "معالي/ ";
                            break;
                        case Model.AttendantType.Secretariat:
                            title = "سعادة/ ";
                            break;
                        case Model.AttendantType.President:
                            title = "معالي/ ";
                            break;
                        case Model.AttendantType.NA:
                            title = "سعادة/ ";
                            break;
                    }
                    //return title;
                }
                else
                    //return "سعادة/ ";
                    title = "سعادة/ ";

            return (title + attName).Trim();

        }

        public static string GetAttendantTitle(Object SomeObj, long session_id)
        {
            Attendant att = null;
            if (SomeObj is SessionAttendant)
            {
                SessionAttendant sAtt = (SessionAttendant)SomeObj;
                att = new Attendant()
                {
                    ID = sAtt.ID,
                    Name = sAtt.Name,
                    FirstName = sAtt.FirstName,
                    SecondName = sAtt.SecondName,
                    TribeName = sAtt.TribeName,
                    Type = (int)sAtt.Type
                };
            }
            else
                att = (Attendant)SomeObj;

            int attendant_title_id = 1;// BLL.EMadbatahFacade.get_session_attendant_title_id(session_id, att.ID);
            string attName = "";
            if (string.IsNullOrEmpty(att.FirstName) && string.IsNullOrEmpty(att.SecondName) && string.IsNullOrEmpty(att.TribeName))
                attName = att.Name.Trim();
            else
                attName = ((att.FirstName == null ? "" : att.FirstName) + " " + (att.SecondName == null ? "" : att.SecondName) + " " + (att.TribeName == null ? "" : att.TribeName)).Replace("   ", " ").Replace("  ", " ").Trim();
            int? type = att.Type;

            string title = "";
            if (attName.Trim().StartsWith("سعادة") || attName.Trim().StartsWith("سعاده") ||
                            attName.Trim().StartsWith("معالي") || attName.Trim().StartsWith("معالى") ||
                attName.Trim().StartsWith("المستشار"))
                title = "";
            //return "";

            else
                if (attendant_title_id > 0)
                {
                    //string title = "";
                    Model.AttendantTitle attType = (Model.AttendantTitle)attendant_title_id;
                    switch (attType)
                    {
                        case Model.AttendantTitle.M3alyPresident:
                            title = "معالى الرئيس/ ";
                            break;
                        case Model.AttendantTitle.S3adetSessionPresident:
                            title = "سعادة رئيس الجلسة/ ";
                            break;
                        case Model.AttendantTitle.S3adet:
                            title = "سعادة/ ";
                            break;
                        case Model.AttendantTitle.M3aly:
                            title = "معالى/ ";
                            break;
                        case Model.AttendantTitle.AlOstaz:
                            title = "الأستاذ/ ";
                            break;
                        case Model.AttendantTitle.Almostashaar:
                            title = "المستشار/ ";
                            break;
                        case Model.AttendantTitle.NoTitle:
                            title = " ";
                            break;
                    }
                    //return title;
                }
                else
                    //return "سعادة/ ";
                    title = "سعادة/ ";

            //  return ("السيد العضو : " + attName).Trim();
            string attFullPresentationName = "";
            if (att.AttendantTitle == null)
            {
                if (att.Type != (int)Model.AttendantType.UnAssigned)
                    attFullPresentationName = "السيد " + att.Name.Trim();
                else attFullPresentationName = att.Name.Trim();
            }
            else attFullPresentationName = att.AttendantTitle.Trim() + " " + att.Name.Trim();
            return attFullPresentationName;
        }

        public static string GetAttendantTitleNSpeakersIndex(Object SomeObj, long session_id)
        {
            Attendant att = null;
            string title = "السيد العضو  ";
            if (SomeObj is SessionAttendant)
            {
                SessionAttendant sAtt = (SessionAttendant)SomeObj;
                att = new Attendant()
                {
                    ID = sAtt.ID,
                    Name = sAtt.Name,
                    FirstName = sAtt.FirstName,
                    SecondName = sAtt.SecondName,
                    TribeName = sAtt.TribeName,
                    Type = (int)sAtt.Type
                };
            }
            else
                att = (Attendant)SomeObj;

            string attName = "";
            if (string.IsNullOrEmpty(att.FirstName) && string.IsNullOrEmpty(att.SecondName) && string.IsNullOrEmpty(att.TribeName))
                attName = att.Name.Trim();
            else
                attName = ((att.FirstName == null ? "" : att.FirstName) + " " + (att.SecondName == null ? "" : att.SecondName) + " " + (att.TribeName == null ? "" : att.TribeName)).Replace("   ", " ").Replace("  ", " ").Trim();

            return (title + attName).Trim();

        }


        public static int CreateMadbatahBody2(long sessionID, string outFilePath, string SessionWorkingDir, string ServerMapPath, Model.SessionDetails details, out List<MadbatahIndexItem> index, out List<SpeakersIndexItem> speakersIndex
          )
        {
            string docPath = outFilePath;
            string resfolderpath = ServerMapPath + "\\resources\\";
            DocXmlParts xmlFilesPaths = new DocXmlParts();
            xmlFilesPaths.CoreFilePropertiesPart = resfolderpath + "core.xml";
            xmlFilesPaths.EndNotes = resfolderpath + "endnotes.xml";
            xmlFilesPaths.FilePropPartPath = resfolderpath + "app.xml";
            xmlFilesPaths.FontTablePart = resfolderpath + "fontTable.xml";
            xmlFilesPaths.FooterPartPath = resfolderpath + "footer1.xml";
            xmlFilesPaths.FootnotesPart = resfolderpath + "footnotes.xml";
            xmlFilesPaths.NumberingDefinitionsPartPath = resfolderpath + "numbering.xml";
            xmlFilesPaths.SettingsPartPath = resfolderpath + "settings.xml";
            xmlFilesPaths.StylePartPath = resfolderpath + "styles.xml";
            xmlFilesPaths.ThemePartPath = resfolderpath + "theme1.xml";
            xmlFilesPaths.WebSettingsPartPath = resfolderpath + "webSettings.xml";

            try
            {
                WordprocessingWorker doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.CreateNew);
                {

                    index = new List<MadbatahIndexItem>();
                    speakersIndex = new List<SpeakersIndexItem>();
                    List<string> writtenAgendaItems = new List<string>();

                    List<List<SessionContentItem>> allItems = SessionContentItemHelper.GetItemsBySessionIDGrouped(sessionID);

                    SessionContentItem lastIteminPrevList = null;

                    string fileText = "";    // added by ihab
                    string fileName = "";    // added by ihab
                    SessionFile sf = new SessionFile();             //added by ihab 

                    int pageNum = 0;
                    int ii = 0;
                    List<long> WrittenAgendaItemWithGroupedSubItems = new List<long>();
                    List<string> WrittenAgendaItemForNewOrder = new List<string>();
                    foreach (List<SessionContentItem> groupedItems in allItems)
                    {
                        AgendaItem curAgendaItem = groupedItems[0].AgendaItem;
                        AgendaSubItem curAgendaSubItem = groupedItems[0].AgendaSubItem;

                        //if (writtenAgendaItems.IndexOf(curAgendaItem.Name) == -1)//commeneted 12-04-2012
                        {
                            ii++;
                            pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);
                            string originalName = curAgendaItem.Name;
                            writtenAgendaItems.Add(curAgendaItem.Name);
                            string updatedAgendaName = "";

                            //if (originalName.Contains("افتتاحية الجلسة ، وكلمة معالي رئيس المجلس"))
                            if (curAgendaItem.IsCustom != null && curAgendaItem.IsCustom == true)
                            {
                                updatedAgendaName = curAgendaItem.Name;
                            }
                            else
                            {
                                //updatedAgendaName = "البند " + LocalHelper.GetLocalizedString("str" + (writtenAgendaItems.Count - 1)) + ": " + curAgendaItem.Name;////usama new ordering
                                //ibrahim 11-06-2012
                                int order = WrittenAgendaItemForNewOrder.IndexOf(curAgendaItem.Name);
                                if (order == -1)
                                {

                                    WrittenAgendaItemForNewOrder.Add(curAgendaItem.Name);
                                    order = WrittenAgendaItemForNewOrder.Count;

                                }
                                else
                                    order += 1;
                                //updatedAgendaName = "البند " + LocalHelper.GetLocalizedString("str" + (curAgendaItem.Order)) + ": " + curAgendaItem.Name;////usama new ordering
                                updatedAgendaName = "البند " + LocalHelper.GetLocalizedString("str" + order) + ": " + curAgendaItem.Name;////usama new ordering
                            }

                            doc.AddParagraph("* " + updatedAgendaName + ":", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                            //commeneted 12-04-2012
                            //index.Add(new MadbatahIndexItem(curAgendaItem.ID , originalName, pageNum, true, "", "", curAgendaItem.IsCustom, curAgendaItem.IsGroupSubAgendaItems));

                            //int ind = index.IndexOf(new MadbatahIndexItem(curAgendaItem.ID, originalName, pageNum+"", true, "", "", curAgendaItem.IsCustom, curAgendaItem.IsGroupSubAgendaItems));
                            int ind = index.FindIndex(curIndexItem => curIndexItem.ID == curAgendaItem.ID);


                            if (ind == -1)
                                index.Add(new MadbatahIndexItem(curAgendaItem.ID, originalName, pageNum + "", true, "", "", curAgendaItem.IsCustom, int.Parse(curAgendaItem.IsIndexed.ToString()), curAgendaItem.IsGroupSubAgendaItems));
                            else
                                index[ind].PageNum += ", " + pageNum;

                        }
                        if (curAgendaItem.IsGroupSubAgendaItems && WrittenAgendaItemWithGroupedSubItems.IndexOf(curAgendaItem.ID) == -1)
                        {
                            List<AgendaSubItem> list = DAL.AgendaHelper.GetAgendaSubItemsByAgendaID(curAgendaItem.ID);
                            foreach (AgendaSubItem sub in list)
                            {
                                pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);//GetCurrentPageNumber();
                                string usedName = sub.Name;
                                if (!string.IsNullOrEmpty(sub.QFrom) && !string.IsNullOrEmpty(sub.QTo))
                                    usedName = new StringBuilder().Append("سؤال موجه إلى معالي / ") + sub.QTo + "من سعادةالعضو /" + sub.QFrom + "حول \"" + sub.Name + "\".";

                                doc.AddParagraph("- " + usedName + ":", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                //commeneted 12-04-2012
                                //index.Add(new MadbatahIndexItem(curAgendaSubItem.ID , curAgendaSubItem.Name, pageNum, false, curAgendaSubItem.QFrom, curAgendaSubItem.QTo, curAgendaSubItem.IsCustom, curAgendaItem.IsGroupSubAgendaItems));

                                int ind = index.FindIndex(curIndexItem => curIndexItem.ID == curAgendaSubItem.ID);

                                if (ind == -1)
                                    index.Add(new MadbatahIndexItem(curAgendaSubItem.ID, curAgendaSubItem.Name, pageNum + "", false, curAgendaSubItem.QFrom, curAgendaSubItem.QTo, curAgendaSubItem.IsCustom, 0, curAgendaItem.IsGroupSubAgendaItems));
                                else
                                    index[ind].PageNum += ", " + pageNum;


                            }
                            WrittenAgendaItemWithGroupedSubItems.Add(curAgendaItem.ID);
                        }

                        if (curAgendaSubItem != null && !curAgendaItem.IsGroupSubAgendaItems)
                        {

                            pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);//GetCurrentPageNumber();
                            string usedName = curAgendaSubItem.Name;
                            if (!string.IsNullOrEmpty(curAgendaSubItem.QFrom) && !string.IsNullOrEmpty(curAgendaSubItem.QTo))
                                usedName = new StringBuilder().Append("سؤال موجه إلى معالي / ") + curAgendaSubItem.QTo + "من سعادةالعضو /" + curAgendaSubItem.QFrom + "حول \"" + curAgendaSubItem.Name + "\".";

                            doc.AddParagraph("- " + usedName + ":", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                            //commeneted 12-04-2012
                            //index.Add(new MadbatahIndexItem(curAgendaSubItem.ID, curAgendaSubItem.Name, pageNum, false, curAgendaSubItem.QFrom, curAgendaSubItem.QTo, curAgendaSubItem.IsCustom,  curAgendaItem.IsGroupSubAgendaItems));
                            int ind = index.FindIndex(curIndexItem => curIndexItem.ID == curAgendaSubItem.ID);

                            if (ind == -1)
                                index.Add(new MadbatahIndexItem(curAgendaSubItem.ID, curAgendaSubItem.Name, pageNum + "", false, curAgendaSubItem.QFrom, curAgendaSubItem.QTo, curAgendaSubItem.IsCustom, 0, curAgendaItem.IsGroupSubAgendaItems));
                            else
                                index[ind].PageNum += ", " + pageNum;

                        }
                        int ctr = 0;

                        //meshmesh: to handle merged with prev issue
                        List<SessionContentItem> newGroup = GroupSpeakerSimilarArticles(groupedItems);
                        //foreach (SessionContentItem sessionItem in groupedItems)
                        foreach (SessionContentItem sessionItem in newGroup)
                        {
                            //a check should be done of files, meshmesh
                            if (lastIteminPrevList != null && lastIteminPrevList.AttendantID == sessionItem.AttendantID &&
                                lastIteminPrevList.AgendaItemID == sessionItem.AgendaItemID &&
                                lastIteminPrevList.AgendaSubItemID == sessionItem.AgendaSubItemID)
                            {
                                sessionItem.MergedWithPrevious = true;
                                //doc.typeBackspace();
                            }
                            else
                                sessionItem.MergedWithPrevious = false;
                            lastIteminPrevList = sessionItem;

                            pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);//GetCurrentPageNumber();
                            if (!sessionItem.MergedWithPrevious.Value)
                            {
                                Attendant att = sessionItem.Attendant;
                                if (!string.IsNullOrEmpty(sessionItem.CommentOnAttendant))
                                {
                                    //doc.insertText("(" + sessionItem.CommentOnAttendant + ")", 14, format, FontColor.Black, TextFont.AdvertisingBold);
                                    doc.AddParagraph(MabatahCreatorFacade.GetAttendantTitle(att, sessionID) + ": " + "(" + sessionItem.CommentOnAttendant + ")", ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                                }
                                else
                                    doc.AddParagraph(MabatahCreatorFacade.GetAttendantTitle(att, sessionID) + ":", ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");

                                int itemIndex = speakersIndex.IndexOf(new SpeakersIndexItem(MabatahCreatorFacade.GetAttendantTitleNSpeakersIndex(att, sessionID), pageNum.ToString(), att.Type));
                                if (itemIndex == -1)
                                    speakersIndex.Add(new SpeakersIndexItem(MabatahCreatorFacade.GetAttendantTitleNSpeakersIndex(att, sessionID), pageNum.ToString() + ",", att.Type));
                                else
                                {
                                    speakersIndex[itemIndex].PageNum += pageNum + ", ";
                                }
                            }
                            string contentItemAsHtml = sessionItem.Text.ToLower().Replace("<br/>", "#####").Replace("<br>", "#####");
                            string[] parts = contentItemAsHtml.Split(new string[] { "#####" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string part in parts)
                            {

                                string tt = part.ToLower().Trim() + " ";
                                HtmlToText htmltotxt = new HtmlToText();
                                tt += " " + TextHelper.StripHTML(sessionItem.Text.ToLower()).Trim();
                                tt = htmltotxt.Convert(tt);
                                doc.AddParagraph(tt.Replace("&nbsp;", " "), ParagraphStyle.NormalArabic, ParagrapJustification.Both, false, "");
                                //doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                            }

                            if (sessionItem.AttachementID != null)
                            {
                                Attachement attach = AttachmentHelper.GetAttachementByID(int.Parse(sessionItem.AttachementID.ToString()));
                                System.IO.File.WriteAllBytes(SessionWorkingDir + attach.Name, attach.FileContent);
                                FileInfo fInfo = new FileInfo(SessionWorkingDir + attach.Name);

                                if (fInfo.Extension.ToLower().Equals(".pdf"))
                                {
                                    String pdfFilePath = SessionWorkingDir + attach.Name;
                                    pdf2ImageConvert.convertPdfFile(pdfFilePath);
                                    string wordAttFilePath = SessionWorkingDir + attach.Name.ToLower().Replace(".pdf", ".pdf.docx");
                                    string[] files = Directory.GetFiles(SessionWorkingDir + fInfo.Name.Replace(fInfo.Extension, ""));

                                    foreach (string f in files)
                                    {
                                        ImageWriter.AddImage(doc.DocMainPart.Document.Body, doc.DocMainPart, f, "rId" + ii);
                                    }
                                }
                            }

                            if (!sessionItem.MergedWithPrevious.Value)
                                doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                            if (!string.IsNullOrEmpty(sessionItem.PageFooter))
                            {
                                doc.AddFootnote(sessionItem.PageFooter);
                            }
                            if (!string.IsNullOrEmpty(sessionItem.CommentOnText))
                            {
                                doc.AddParagraph("(" + sessionItem.CommentOnText + ")", ParagraphStyle.NormalArabic, ParagrapJustification.Center, false, "");
                            }
                            ctr++;
                            foreach (string part in parts)
                            {
                                string tt = TextHelper.StripHTML(part.ToLower()).Trim() + " ";

                                fileText = tt;                                     // added by ihab
                                sf = SessionFileHelper.GetSessionFileByID((int)sessionItem.SessionFileID);   // added by ihab

                                if (sf.FileError == 1)  // added by ihab
                                {
                                    try
                                    {
                                        string tempDir = ServerMapPath + "Corrupted files\\" + System.IO.Path.GetFileName(sf.Name.Replace(".mp3", ""));
                                        System.IO.Directory.CreateDirectory(tempDir);
                                        fileName = sf.Name.Split('\\')[3].Replace("mp3", "txt");
                                        FileStream fs = System.IO.File.Create(tempDir + "\\" + fileName);
                                        fs.Close();
                                        System.IO.File.WriteAllText(tempDir + "\\" + fileName, fileText);
                                        System.IO.File.Copy(ServerMapPath + sf.Name, tempDir + "\\" + System.IO.Path.GetFileName(sf.Name), true);
                                        System.IO.File.Copy(ServerMapPath + sf.Name.Replace("mp3", "trans.xml"), tempDir + "\\" + System.IO.Path.GetFileName(sf.Name.Replace("mp3", "trans.xml")), true);
                                    }
                                    catch { }
                                }
                            }

                        }

                    }

                    //07-06-2012
                    doc.AddParagraph("الأمين العام للمجلس								رئيس المجلس"
                         , ParagraphStyle.NormalArabic, ParagrapJustification.Both, false, "");

                    doc.Save();
                    int num = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);//GetCurrentPageNumber();
                    doc.Dispose();
                    return num;
                }
            }
            catch (Exception ex)
            {

                index = new List<MadbatahIndexItem>();
                speakersIndex = new List<SpeakersIndexItem>();
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahBody(" + sessionID + "," + outFilePath + ")");
                return -1;
            }
        }

        public static string setFormatAtrrInSpan(string text)
        {
            string temp = text;
            MatchCollection collections = Regex.Matches(text, "<strong.*?</strong>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            foreach (Match match in collections)
            {
                MatchCollection subCollections = Regex.Matches(match.Value, "<span.*?</span>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                foreach (Match submatch in subCollections)
                    temp = temp.Replace(submatch.Value, submatch.Value.Replace("<span ", "<span bold=\"1\" "));
            }
            collections = Regex.Matches(temp, "<em.*?</em>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            foreach (Match match in collections)
            {
                MatchCollection subCollections = Regex.Matches(match.Value, "<span.*?</span>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                foreach (Match submatch in subCollections)
                    temp = temp.Replace(submatch.Value, submatch.Value.Replace("<span ", "<span italic=\"1\" "));
            }

            return temp.Replace("<strong><", "<").Replace("></strong>", ">").Replace("<em>", "").Replace("</em>", "");


        }
    }
}
