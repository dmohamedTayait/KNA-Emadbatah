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

                int sessionStartSize = 0;
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
                int speakerSize = MabatahCreatorFacade.CreateSpeakersIndex(speakersIndex,
                  coverSize + sessionStartSize + indexSize - 1, //2 for att cover + speakers cover
                  ServerMapPath, SessionWorkingDir + "indexSpeakers.docx");
                if (speakerSize == -1)
                    throw new Exception("Speakers Index Creation Failed.");

                //Merga All Generated Files
                List<string> mergeList = new List<string>();
                mergeList.Add(SessionWorkingDir + "indexDoc.docx");
                mergeList.Add(SessionWorkingDir + "indexSpeakers.docx");//done
                mergeList.Add(ServerMapPath + "\\docs\\templates\\MadbatahBodyCover.docx");//ready
                mergeList.Add(SessionWorkingDir + "bodyDoc.docx");
                mergeList.Add(ServerMapPath + "\\docs\\templates\\MadbatahEndCover.docx");//ready
                File.Copy(SessionWorkingDir + "MadbatahcoverDoc.docx", SessionWorkingDir + sessionID + ".docx", true);
                WordprocessingWorker.MergeWithAltChunk(SessionWorkingDir + sessionID + ".docx", mergeList.ToArray());



                //List<byte[]> docs = new List<byte[]>();
                //foreach (string file in mergeList)
                //    docs.Add(File.ReadAllBytes(file));

                //File.WriteAllBytes(SessionWorkingDir + "merged.docx", WordprocessingWorker.OpenAndCombine(docs));
                //WordMerger.MergeDocs(mergeList.ToArray(), SessionWorkingDir + sessionID + ".docx", true);

                //PdfMaker.ConvertDocxToPdf(SessionWorkingDir, ServerMapPath, SessionWorkingDir + sessionID + ".docx", SessionWorkingDir + sessionID + ".pdf");
                //doc = new WordDocument();
                //doc.Open(SessionWorkingDir + sessionID + ".doc");

                //string sessionSerial = EMadbatahFacade.GetSessionNameForWord(13, details.Stage, details.Season);
                //doc.InsertPageNumInFooterWithSessionNum(sessionSerial);//"5/5/14");
                //Console.WriteLine(index.Count.ToString());
                //doc.SaveDocument();
                //doc.Quit();

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
            string sessionName = details.Type + " / " + details.EparlimentID.ToString();//EMadbatahFacade.GetSessionName(details.Season, details.Stage, details.Serial);

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
            sb.Append("<Season>").Append(fomratterMale.getResultEnhanced((int)details.Season)).Append("</Season>");
            sb.Append("<StageType>").Append("ال" + details.StageType).Append("</StageType>");
            sb.Append("<Stage>").Append(fomratterMale.getResultEnhanced((int)details.Stage)).Append("</Stage>");
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
                WordprocessingWorker doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.Open,true);
                {
                    doc.DocHeaderString = "ــــــــــــــــــــ        " + details.EparlimentID.ToString() + " / " + details.Type + "        ــــــــــــــــــــ";
                    doc.ApplyHeaderAndFooter();

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
                        
                        pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);
                        int ctr = 0;
                        //meshmesh: to handle merged with prev issue
                        List<List<SessionContentItem>> speakerGroup = new List<List<SessionContentItem>>();
                        List<SessionContentItem> newGroup = GroupSpeakerSimilarArticles(groupedItems, out speakerGroup);
                        //foreach (SessionContentItem sessionItem in groupedItems)
                        int j = 0;
                        int k = 0;
                        foreach (SessionContentItem sessionItem in newGroup)
                        {
                            //a check should be done of files, meshmesh
                            if (lastIteminPrevList != null && lastIteminPrevList.AttendantID == sessionItem.AttendantID &&
                                lastIteminPrevList.AgendaItemID == sessionItem.AgendaItemID &&
                                lastIteminPrevList.AgendaSubItemID == sessionItem.AgendaSubItemID)
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
                                pageNum = doc.CountPagesUsingOpenXML(doc, docPath, xmlFilesPaths, ServerMapPath, out doc);
                                AgendaItem updatedAgenda = contentItem.AgendaItem;
                                if (updatedAgenda.Name != "غير معرف")
                                {
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
                                            /*   if (!string.IsNullOrEmpty(sessionItem.CommentOnAttendant))
                                                   doc.AddParagraph(MabatahCreatorFacade.GetAttendantTitle(att, sessionID) + ": " + "(" + sessionItem.CommentOnAttendant + ")", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                               else
                                                   doc.AddParagraph(MabatahCreatorFacade.GetAttendantTitle(att, sessionID) + ":", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                               */
                                            WriteAttendantInWord(sessionItem, att, doc);

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

                                if (!string.IsNullOrEmpty(contentItem.PageFooter))
                                {
                                    doc.AddFootnote(contentItem.PageFooter);
                                }

                                // text += " " + TextHelper.StripHTML(contentItem.Text.ToLower()).Trim();
                                text += " " + contentItem.Text.ToLower().Trim();
                                contentItemAsText = text.ToLower().Replace("<br/>", "#!#!#!").Replace("<br>", "#!#!#!");
                                k++;
                                if (contentItem.AttachementID != null)
                                {
                                    Attachement attach = AttachmentHelper.GetAttachementByID(int.Parse(contentItem.AttachementID.ToString()));
                                    System.IO.File.WriteAllBytes(SessionWorkingDir + attach.Name, attach.FileContent);
                                    FileInfo fInfo = new FileInfo(SessionWorkingDir + attach.Name);

                                    if (fInfo.Extension.ToLower().Equals(".pdf"))
                                    {
                                        String pdfFilePath = SessionWorkingDir + attach.Name;
                                        pdf2ImageConvert.convertPdfFile(pdfFilePath);
                                        string wordAttFilePath = SessionWorkingDir + attach.Name.ToLower().Replace(".pdf", ".pdf.docx");
                                        string[] files = Directory.GetFiles(SessionWorkingDir + fInfo.Name.Replace(fInfo.Extension, "")).OrderBy(p => new FileInfo(p).CreationTime).ToArray(); ;

                                        // doc.AddParagraph(contentItemAsText.Replace("&nbsp;", " "), ParagraphStyle.NormalArabic, ParagrapJustification.Both, false, "");
                                        //contentItemAsText = "";
                                        /*string[] sep = new string[1] { "#!#!#!" };
                                        string[] paragraphs = contentItemAsText.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                                        string parag = "";
                                        foreach (string strP in paragraphs)
                                        {
                                            if (strP.IndexOf("procedure-id") != -1)
                                            {
                                                parag = TextHelper.StripHTML(strP.ToLower()).Trim();
                                                if (parag != "")
                                                {
                                                    doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                                    doc.AddParagraph(parag.Replace("&nbsp;", " "), ParagraphStyle.NormalArabic, ParagrapJustification.Center, false, "");
                                                    doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                                }
                                            }
                                            else
                                            {
                                                parag = TextHelper.StripHTML(strP.ToLower()).Trim();
                                                if (parag != "")
                                                    doc.AddParagraph(parag.Replace("&nbsp;", " "), ParagraphStyle.NormalArabic, ParagrapJustification.Both, false, "");
                                            }
                                        }*/

                                        WriteParagraphInWord(sessionItem, contentItemAsText, doc);
                                        text = "";
                                        foreach (string f in files)
                                        {
                                            ImageWriter.AddImage(doc.DocMainPart.Document.Body, doc.DocMainPart, f, "rId" + ii);
                                            ii++;
                                        }
                                    }
                                }
                                if (speakerGroup[j].Count == k)
                                {
                                    WriteParagraphInWord(sessionItem, contentItemAsText, doc);
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


                            /*if (!sessionItem.MergedWithPrevious.Value)
                                doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");*/

                           
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

                        }
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
                        if (att.Type != 3)
                            attFullPresentationName = attFullPresentationName + ")";
                        doc.AddParagraph(attFullPresentationName, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                        if (att.Type == 3)
                            doc.AddParagraph("    " + att.JobTitle + ")", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    }
                    else
                    {
                        if (att.AttendantTitle == null)
                            attFullPresentationName = "السيد " + att.ShortName.Trim() + " : ";
                        else attFullPresentationName = att.AttendantTitle.Trim() + " " + att.ShortName.Trim() + " : ";
                        doc.AddParagraph(attFullPresentationName, ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                        if (att.Type == 3)
                            doc.AddParagraph("    (" + att.JobTitle + ")", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    }
                }
            }
        }

        public static void WriteParagraphInWord(SessionContentItem sessionItem, string contentItemAsText, WordprocessingWorker doc)
        {
            contentItemAsText = contentItemAsText.Replace("#!#!#!", " ");
           
            string[] p = new string[] { };
            string[] paragraphs = GetParagraphsArr(contentItemAsText, out p);
            string parag = "";
            for (int pp = 0; pp < paragraphs.Length; pp++)
            {
                parag = TextHelper.StripHTML(paragraphs[pp].ToLower()).Trim();
                if (parag != "")
                {
                    if (pp != 0)
                        WriteAttendantInWord(sessionItem, sessionItem.Attendant, doc);
                    doc.AddParagraph(parag.Replace("&nbsp;", " "), ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                }
               // else doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                if (pp < p.Length)
                {
                    parag = TextHelper.StripHTML(p[pp].ToString()).Trim();
                    if (parag != "")
                    {
                        if (p[pp].ToString().IndexOf("center") > 0)
                            doc.AddParagraph(parag.Replace("&nbsp;", " "), ParagraphStyle.ParagraphTitle, ParagrapJustification.Center, false, "");
                        else doc.AddParagraph(parag.Replace("&nbsp;", " "), ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                        doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                     //   if (pp < paragraphs.Length)
                         //   WriteAttendantInWord(sessionItem, sessionItem.Attendant, doc);
                    }
                }
            }
        }

        public static string[] GetParagraphsArr(string parag, out string [] p)
        {
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

        public static int CreateMadbatahIndex(List<MadbatahIndexItem> index,string folderPath, int indexSize, int bodySize, string outIndexPath, string ServerMapPath,Model.SessionDetails details)
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
                  + " </tr><tr><table border='1' style='width:100%; border:2px solid #000; border-collapse:collapse; text-align:center; direction: rtl; font-family: AdvertisingBold; font-size: 14pt;' align='center' cellpadding='5' cellspacing='0'>"
                  + " <tr>"
                  + "   <th style='border:2px solid #000;width:50px'>م</th>"
                  + "   <th style='border:2px solid #000'>الموضوع</th>"
                  + "   <th style='border:2px solid #000'>الصفحات</th>"
                  + " </tr>";


                int i = 1, j = 1;
                string indx = "";
                string emptyRowBold = "<tr style=\"font-family: UsedFont;font-size: 14pt;\"><td style='border:2px solid #000;width:50px'>ItemNum</td><td style='text-align:right;border:2px solid #000'>ItemName</td><td style='border:2px solid #000'>PageNum</td></tr>";
                StringBuilder sb = new StringBuilder();
                sb.Append(indexHeader);

                string toBeReplaced = emptyRowBold.Replace("ItemNum", "1").Replace("ItemName", "<strong> - أسماء السادة الأعضاء</strong>").Replace("PageNum", "1").Replace("UsedFont", "AdvertisingBold");
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
                    pageNum = pageNums[0];
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
                    toBeReplaced = emptyRowBold.Replace("ItemNum", indx).Replace("ItemName", name).Replace("PageNum", pageNum).Replace("UsedFont", font);
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
                    <table border='1' style='width:100%; border:2px solid #000; border-collapse:collapse; text-align:center; direction: rtl; font-family: AdvertisingBold; font-size: 14pt;' align='center' cellpadding='3' cellspacing='0'>
                        <tr>
                            <th style='border:2px solid #000'>اسم المتحدث</th>
                            <th style='border:2px solid #000'>رقم الصفحة</th>
                        </tr>";
            

                int i = 1;
                string emptyRowBold = "<tr><td style='text-align:right;border:2px solid #000'>ItemName</td><td style='border:2px solid #000'>PageNum</td></tr>";
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
                    if (pagesStr.Length > 2)
                        pagesStr = pagesStr.Remove(pagesStr.Length - 2);
                       sb.Append(emptyRowBold.Replace("ItemName", item.Name.Trim()).Replace("PageNum", pagesStr));
                    i++;
                }
                sb.Append("</table>");

                int stats = 0;
                HTMLtoDOCX hd = new HTMLtoDOCX();
                hd.CreateFileFromHTML(sb.ToString(), outPath);
                return stats;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahIndex(" + ")");
                return -1;
            }
        }

        public static List<SessionContentItem> GroupSpeakerSimilarArticles(List<SessionContentItem> groupedItems, out List<List<SessionContentItem>> speakerContentItem)
        {
            List<SessionContentItem> newGroup = new List<SessionContentItem>();
            List<SessionContentItem> newSpeaker = new List<SessionContentItem>();
            List<List<SessionContentItem>> speakerSessionContentItem = new List<List<SessionContentItem>>();
            speakerContentItem = new List<List<SessionContentItem>>();
            int i = 0;
            foreach (SessionContentItem item in groupedItems)
            {
                if (newGroup.Count == 0)
                {
                    item.PageFooter = item.PageFooter == null ? "" : item.PageFooter;
                    newGroup.Add(item);
                    newSpeaker.Add(item);
                    speakerContentItem.Add(newSpeaker);
                }
                else
                    if (item.MergedWithPrevious == true)
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
                        i++;
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

        public static void CreateMadbatahStart(Model.SessionDetails details, WordprocessingWorker doc)
        {
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

            string docStartOnTime = SessionStartFacade.madbatahHeader.Replace("%sessionNum%", sessionNum).Replace("%GeorgianDate%", gDate).Replace("%sessionTime%", timeInHour).Replace("%hijriDate%", hijriDate).Replace("%President%", details.Presidnt);


            doc.AddParagraph(docStartOnTime, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
            doc.AddParagraph(" ", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

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
                    writeAttendantNFile("* و بحضور السادة الأعضاء:", attendants, doc);
                    writeAttendantNFile("* و فى أثناء الجلسة حضر كل من السادة الأعضاء:", attendantsWithinSession, doc);
                    writeAttendantNFile("* الغائبون بعذر:", abologAttendants, doc);
                    writeAttendantNFile("*الغائبون بدون اعتذار:", absenceAttendants, doc);
                }
            }
            //Session Started After Time
            else
            {
                allAttendants = SessionStartFacade.GetSessionAttendantOrderedByStatus(details.SessionID, (int)SessionOpenStatus.OnTime);
                if (allAttendants.Count > 0)
                    writeAttendantNFile("* و بحضور السادة الأعضاء:", allAttendants[0], doc);

                doc.AddParagraph("السيد الرئيس:", ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                doc.AddParagraph("بسم الله الرحمن الرحيم و الصلاة و السلام على رسول الله ، نتيجة لعدم اكتمال النصاب تأخر الجلسة لمدة نصف ساعة.", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                string docStartNotOnTime = SessionStartFacade.madbatahStartNotOnTime.Replace("%sessionNum%", sessionNum).Replace("%GeorgianDate%", gDate).Replace("%sessionTime%", timeInHour).Replace("%hijriDate%", hijriDate).Replace("%President%", details.Presidnt);
                doc.AddParagraph(docStartNotOnTime, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                doc.AddParagraph("السيد الرئيس:", ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");

                if (details.Date.DayOfWeek == DayOfWeek.Tuesday)
                    doc.AddParagraph(SessionStartFacade.madbatahTuesdayIntro, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                else
                    doc.AddParagraph(SessionStartFacade.madbatahWednesdayIntro, ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

                allAttendants = SessionStartFacade.GetSessionAttendantOrderedByStatus(details.SessionID, (int)SessionOpenStatus.NotOnTime);
                if (allAttendants.Count > 0)
                {
                    attendants = allAttendants[0];
                    attendantsWithinSession = allAttendants[1];
                    abologAttendants = allAttendants[2];
                    absenceAttendants = allAttendants[3];
                    inMissionAttendants = allAttendants[4];
                    writeAttendantNFile("* تليت بعد افتتاح الجلسة أسماء الأعضاء الحاضرين:", attendants, doc);
                    writeAttendantNFile("* و فى أثناء الجلسة حضر كل من السادة الأعضاء:", attendantsWithinSession, doc);
                    writeAttendantNFile("* الغائبون بعذر:", abologAttendants, doc);
                    writeAttendantNFile("*الغائبون بدون اعتذار:", absenceAttendants, doc);

                }
            }

            //Committee Attendance
            bool isCommittee = false;
            List<Committee> committeeLst = CommitteeHelper.GetAllCommittee();
            foreach (Committee comm in committeeLst)
            {
                List<List<DefaultAttendant>> allAtt = CommitteeHelper.GetSessionCommiteeAttendance(comm.ID, details.SessionID);
                if (allAtt[0].Count > 0 || allAtt[1].Count > 0)
                {
                    if (!isCommittee)
                    {
                        doc.AddParagraph("* أسماء السادة الأعضاء الغائبين بعذر أو من دون عذر عن حضور اجتماعات المجلس:", ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                        isCommittee = true;
                    }
                    doc.AddParagraph(comm.CommitteeName, ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                    writeAttendantNFile("* الغائبون بعذر:", allAtt[0], doc);
                    writeAttendantNFile("*الغائبون بدون اعتذار:", allAtt[1], doc);
                }
            }
        }

        public static bool writeAttendantNFile(string head,List<Attendant> attendants, WordprocessingWorker doc)
        {
            if (attendants.Count > 0)
            {
                doc.AddParagraph(head, ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                foreach (Attendant att in attendants)
                {
                    if (att.Name != "غير معرف")
                    {
                        doc.AddParagraph(" - " + att.Name.Trim(), ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                        if (att.Type == (int)Model.AttendantType.GovernmentRepresentative)
                            doc.AddParagraph("        (" + att.JobTitle.Trim() + ")", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    }
                }
                doc.AddParagraph(" ", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
            }
            return true;
        }

        public static bool writeAttendantNFile(string head, List<DefaultAttendant> attendants, WordprocessingWorker doc)
        {
            if (attendants.Count > 0)
            {
                doc.AddParagraph(head, ParagraphStyle.UnderLineParagraphTitle, ParagrapJustification.RTL, false, "");
                foreach (DefaultAttendant att in attendants)
                {
                    if (att.Name != "غير معرف")
                    {
                        doc.AddParagraph(" - " + att.Name.Trim(), ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                        if (att.Type == (int)Model.AttendantType.GovernmentRepresentative)
                            doc.AddParagraph("        (" + att.JobTitle.Trim() + ")", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                    }
                }
                doc.AddParagraph(" ", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
            }
            return true;
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

        public static void test(string outFilePath, string SessionWorkingDir, string ServerMapPath)
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
                WordprocessingWorker doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.CreateNew);
                {
                    List<MembersVote> mems = MembersVoteHelper.GetMembersVoteNonSecretVoteID(155);
                    List<SessionMembersVote> membersVoteLst = new List<SessionMembersVote>();
                    foreach (MembersVote mem in mems)
                    {
                        SessionMembersVote s = new SessionMembersVote(mem.AutoID, mem.NonSecretVoteSubjectID, mem.PersonID, (int)mem.MemberVoteID, mem.MemberFullName);
                        membersVoteLst.Add(s);
                    }
                    doc.AddCustomTable(membersVoteLst);
                }
                doc.Save();
                doc.Dispose();
            }
            catch (Exception ex)
            {
            }
            /*

            TableCellMarginDefault tableCellMarginDefault1 = new TableCellMarginDefault();

            TopMargin topMargin1 = new TopMargin() { Width = "500", Type = TableWidthUnitValues.Pct };

            TableCellLeftMargin tableCellLeftMargin1 = new TableCellLeftMargin() { Width = 108, Type = TableWidthValues.Dxa };

            BottomMargin bottomMargin1 = new BottomMargin() { Width = "500", Type = TableWidthUnitValues.Pct };

            TableCellRightMargin tableCellRightMargin1 = new TableCellRightMargin() { Width = 108, Type = TableWidthValues.Dxa };*/
        }


        public static void CreateMadbatahStart2(Model.SessionDetails details, string outStartPath, string ServerMapPath)
        {

            File.Copy(ServerMapPath + "\\docs\\templates\\madbatahStart-template.docx", outStartPath, true);
            //for dates 
            //Model.SessionDetails details = SessionStartFacade.GetSessionDetails(sessionID);

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

            NumberingFormatter fomratterFemale = new NumberingFormatter(false);
            NumberingFormatter fomratterMale = new NumberingFormatter(true);
            string sessionName = EMadbatahFacade.GetSessionName(details.Season, details.Stage, details.Serial);


            StringBuilder sb = new StringBuilder();
            sb.Append("<root>");
            sb.Append("<SessionNum>").Append(sessionNum).Append("</SessionNum>");
            sb.Append("<HijriDate>").Append(hijriDate).Append("</HijriDate>");
            sb.Append("<GeorgianDate>").Append(gDate).Append("</GeorgianDate>");
            sb.Append("<sessionTime>").Append(timeInHour).Append("</sessionTime>");
            sb.Append("<SessionNum>").Append(sessionNum).Append("</SessionNum>");
            sb.Append("<SessionNum>").Append(sessionNum).Append("</SessionNum>");
            sb.Append("<SessionNum>").Append(sessionNum).Append("</SessionNum>");
            sb.Append("<SessionNum>").Append(sessionNum).Append("</SessionNum>");

            sb.Append("<SessionNum>").Append(sessionName).Append("</SessionNum>");
            sb.Append("<Season>").Append(fomratterMale.getResultEnhanced((int)details.Season)).Append("</Season>");
            sb.Append("<StageType>").Append(details.StageType).Append("</StageType>");
            sb.Append("<Stage>").Append(fomratterMale.getResultEnhanced((int)details.Stage)).Append("</Stage>");
            sb.Append("<Subject>").Append(details.Subject).Append("</Subject>");
            sb.Append("<DateHijri>").Append(hijriDate).Append("</DateHijri>");
            sb.Append("<DateMilady>").Append(gDate).Append("</DateMilady>");
            sb.Append("</root>");


            string xmlToReplace = @" <root>
                      <SessionNum>" + sessionName + @"</SessionNum>
                      <HijriDate>" + hijriDate + @"</HijriDate>
                      <GeorgianDate>" + gDate + @"</GeorgianDate>
                      <sessionTime>" + timeInHour + @"</sessionTime>
                      <Table Name='AgendaItem'>
                        <Field Name='AgendaItem' />
                      </Table>
                    <Header>
                    <SessionNum>SessionNum</SessionNum>
                    <sessionType>sessionType</sessionType>
                    <sessionStage>sessionStage</sessionStage>
                    <sessionTime>sessionTime</sessionTime>
                      <HijriDate>HijriDate</HijriDate>
                      <GeorgianDate>GeorgianDate</GeorgianDate>
                    <sessionPresident>sessionPresident</sessionPresident>
                    </Header>
                      <Table Name='ApologiesAtt'>
                        <Field Name='ApologiesAtt1' />
                        <Field Name='ApologiesAtt2' />
                      </Table>
                      <Table Name='AbsentAtt'>
                        <Field Name='AbsentAtt1' />
                        <Field Name='AbsentAtt2' />
                      </Table>

                      <Table Name='attFromGov'>
                        <Field Name='attFromGov' />
                        <Field Name='attFromGovTitle' />
                      </Table>
                    <attendingOutOfMajles>attendingOutOfMajles</attendingOutOfMajles>
                    <secertairs>secertairs</secertairs>
                </root>";

            XElement contentControlValues = XElement.Parse(xmlToReplace);
            WordTemplateHandler.replaceCustomXML(outStartPath, xmlToReplace);
            WordTemplateHandler.replaceTableXmlContents(outStartPath, contentControlValues);
        }

        public static int CreateMadbatahIndexWithAttachment(List<MadbatahIndexItem> index, List<Attachement> attachments,
            string folderPath, int indexSize, int sessionStartSize, int bodySize, string outIndexPath, string ServerMapPath,
            Model.SessionDetails details, bool upDateAttach, out int allAttachSizes)
        {
            try
            {
                allAttachSizes = 0;
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

                string sessionName = details.Subject + "رقم (" + details.Type + ")";


                string indexHeader =
                    "<p align=\"center\"><font size=\"14\" face=\"AdvertisingBold\">"
                    + "<table width=\"100%\" border=\"0\" align=\"right\" cellpadding=\"3\" cellspacing=\"3\" style=\"writing-mode: tb-rl; text-align:right; direction: rtl; font-family: AdvertisingBold; font-size: 14pt;\">"
                    + "<tr style=\";\"><th colspan=\"3\" align=\"center\" scope=\"col\">بسم الله الرحمن الرحيم</th></tr>"
                    + "<tr style=\";\"><th colspan=\"3\" align=\"center\" scope=\"col\">مجلس الأمة</th></tr>"
                    + "<tr style=\";\"><th colspan=\"3\" align=\"center\" scope=\"col\">الأمانة العامة</th></tr>"
                    + "<tr style=\";\"><th colspan=\"3\" align=\"center\" scope=\"col\">ادارة شؤون المضابط</th></tr>"
                    + "<tr style=\";\"><th colspan=\"3\" align=\"center\" scope=\"col\"></th></tr>"
                    + "<tr style=text-decoration:underline; \";\"><th colspan=\"3\" align=\"center\" scope=\"col\">ملخص الموضوعات التى نظرت فى  " + sessionName + " ت</th></tr>"
                    + "<tr style=text-decoration:underline; \";\"><th colspan=\"3\" align=\"center\" scope=\"col\">المعقودة يوم " + hijriDate + "</th></tr>"
                    + "<tr style=text-decoration:underline; \";\"><th colspan=\"3\" align=\"center\" scope=\"col\">الموافق " + gDate + "</th></tr>"
                    + "<tr style=\";\"><th colspan=\"3\" align=\"center\" scope=\"col\">فهرس الموضوعات</th></tr>"
                    + "<tr style=\";\"><th scope=\"col\">م</th><th scope=\"col\">الموضوع</th><th scope=\"col\">رقم الصفحة</th></tr>";

                //doc.insertText("فهرس المحتويات", 14, format, FontColor.Black, TextFont.AdvertisingBold);
                //doc.insertBreakLine(1);
                //doc.insertText("البند                     الموضوع                  رقم الصفحة"
                //    , 14, format, FontColor.Black, TextFont.AdvertisingBold);
                //doc.insertBreakLine(1);
                //format.align = Alignment.Right;
                int i = 1;
                int coverSize = 1;
                //int charsBold = 94;
                //int charsNorm = 99;
                string emptyRowBold = "<strong><tr align=\"right\" style=\"font-family: AdvertisingBold;font-size: 14pt;\"><td>ItemNum</td><td>ItemName</td><td>PageNum</td></tr></strong>";
                string emptyRow = "<tr align=\"right\" style=\"font-family: AdvertisingBold;font-size: 14pt;\"><td>ItemNum</td><td>ItemName</td><td>PageNum</td></tr>";
                StringBuilder sb = new StringBuilder();
                sb.Append(indexHeader);


                /*  foreach (MadbatahIndexItem item in index)
                  {
                      string itemNum = "";
                      string name = "";
                      string pageNum = "";
                      string nextPageNum = "";
                      MadbatahIndexItem nextItem = item;
                      if (item.IsMainItem)
                      {
                          try { nextItem = index[i]; }
                          catch { }
                          itemNum = LocalHelper.GetLocalizedString("str" + (i));
                          i++;
                          name = item.Name;
                          pageNum = item.PageNum.ToString();
                          nextPageNum = nextItem.PageNum.ToString();

                          string[] pageNums = pageNum.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                          pageNum = pageNums[0].Trim().ToString();
                          string[] nextPageNums = nextPageNum.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                          nextPageNum = nextPageNums[0].Trim().ToString();
                          if (pageNum != nextPageNum)
                              pageNum = nextPageNum + "-" + pageNum;

                          string toBeReplaced = emptyRowBold.Replace("ItemNum", i.ToString()).Replace("ItemName", name).Replace("PageNum", pageNum);


                          sb.Append(toBeReplaced);
                      }
                  }
                  */

                foreach (MadbatahIndexItem item in index)
                {
                    string itemNum = "";
                    string name = "";
                    string pageNum = "";
                    //item.PageNum = item.PageNum + coverSize + sessionStartSize + indexSize;
                    if (item.IsMainItem)
                    {
                        itemNum = LocalHelper.GetLocalizedString("str" + (i));
                        i++;
                        name = item.Name;
                        //str += item.Subject.PadLeft(charsBold - ((item.Subject.Length % 90) + item.PageNum.ToString().Length - 1), ' ');// +"     ";
                        pageNum = item.PageNum.ToString();
                        //added 7-6-2012
                        string[] pageNums = pageNum.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        StringBuilder sbpagesNumber = new StringBuilder();
                        foreach (string curpageNum in pageNums)
                        {
                            sbpagesNumber.Append(curpageNum.Trim()).Append(", ");
                        }
                        pageNum = sbpagesNumber.ToString();
                        //format.textStyle = TextStyle.Bold;
                        string toBeReplaced = emptyRowBold.Replace("ItemNum", itemNum).Replace("ItemName", name).Replace("PageNum", pageNum);
                        //if (name.Contains("افتتاحية الجلسة ، وكلمة معالي رئيس المجلس"))
                        if (item.IsCustom != null && item.IsCustom == true) //for handling eftta7eyet el magles
                        {
                            toBeReplaced = emptyRowBold.Replace("ItemNum", i.ToString()).Replace("ItemName", name).Replace("PageNum", pageNum);
                            i--;
                        }

                        sb.Append(toBeReplaced);
                    }
                    else
                    {
                        //str += "- " + item.Subject.PadLeft(charsNorm - ((item.Subject.Length%90) + item.PageNum.ToString().Length + "- ".Length - 1), ' ');// +"     ";
                        //str += item.PageNum;
                        //format.textStyle = TextStyle.Normal;
                        if (!item.IsGroupSubAgendaItems)//25-03-2012
                        {
                            name = item.Name;
                            pageNum = item.PageNum.ToString();

                            //added 7-6-2012
                            string[] pageNums = pageNum.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            StringBuilder sbpagesNumber = new StringBuilder();
                            foreach (string curpageNum in pageNums)
                            {
                                sbpagesNumber.Append(curpageNum.Trim()).Append(", ");
                            }
                            pageNum = sbpagesNumber.ToString();


                            if (!string.IsNullOrEmpty(item.QFrom) && !string.IsNullOrEmpty(item.QTo))
                                name = new StringBuilder().Append("سؤال موجه إلى معالي / ") + item.QTo + "من سعادةالعضو /" + item.QFrom + "حول \"" + item.Name + "\".";
                            sb.Append(emptyRow.Replace("ItemNum", "").Replace("ItemName", "- " + name).Replace("PageNum", pageNum));
                        }
                    }
                    //doc.insertText(str, 14, format, FontColor.Black, TextFont.AdvertisingBold);
                    //doc.insertBreakLine(1);
                }

                int startPageNum = coverSize + sessionStartSize + indexSize + bodySize + 1; // 1 for attachments cover

                DocXmlParts xmlFilesPaths = WordprocessingWorker.GetDocParts(ServerMapPath + "\\resources\\");

                sb.Append("</table></font></p>");

                int stats = 0;
                //HtmlToOpenXml.SaveHtmlToWord(sb.ToString(), @outIndexPath, ServerMapPath + "\\resources\\", out stats);
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

                allAttachSizes = 0;

                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahIndexWithAttachment");

                return -1;
            }
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
        /*
        public static void CreateMadbatah(long sessionID, string outFilePath)
        {
            Model.SessionDetails details = SessionStartFacade.GetSessionDetails(sessionID);
            List<SessionContentItem> items = ReviewerFacade.GetSessionContentItems(sessionID);

            WordDocument doc = new WordDocument();
            doc.CreateNew();
            BasicFormat format = new BasicFormat();
            format.align = Alignment.Right;
            format.textStyle = TextStyle.Bold;
            doc.InsertPageNumInFooterWithSessionNum(details.Subject + "/" + details.Stage + "/" + details.Season);//"5/5/14");

            List<MadbatahIndexItem> index = new List<MadbatahIndexItem>();
            List<SpeakersIndexItem> speakersIndex = new List<SpeakersIndexItem>();

            int pageNum = 0;
            foreach (SessionAgendaItem item in details.AgendaItems.Values)
            {
                //if (item.IsCustom.Value)
                //{
                //    continue;
                //}
                if (item.Order == 0 || item.IsCustom.Value)
                {
                    var list = from it in items
                               orderby it.FragOrderInXml
                               where it.AgendaItemID == item.ID 
                               select it;

                    pageNum = doc.GetCurrentPageNumber();
                    index.Add(new MadbatahIndexItem("", item.Text, pageNum, true));
                    foreach (SessionContentItem sessionItem in list)
                    {
                        pageNum = doc.GetCurrentPageNumber();
                        if (!sessionItem.MergedWithPrevious.Value)
                        {
                            Attendant att = sessionItem.Attendant;
                            format.textStyle = TextStyle.Bold;
                            format.align = Alignment.Right;
                            doc.insertText(att.Name, 14, format, FontColor.Black, TextFont.AdvertisingBold);
                            speakersIndex.Add(new SpeakersIndexItem(att.Name, pageNum));
                            if (!string.IsNullOrEmpty(sessionItem.CommentOnAttendant))
                            {
                                doc.insertText("(" + sessionItem.CommentOnAttendant + ")", 14, format, FontColor.Black, TextFont.AdvertisingBold);
                            }

                            doc.insertBreakLine(1);
                        }
                        format.textStyle = TextStyle.Normal;
                        format.align = Alignment.Right;
                        doc.insertText(sessionItem.Text, 14, format, FontColor.Black, TextFont.AdvertisingBold);
                        if (string.IsNullOrEmpty(sessionItem.PageFooter))
                            doc.AddFooteNote(sessionItem.PageFooter);

                        //if(sessionItem.PageFooter

                        doc.insertBreakLine(1);

                        if (!string.IsNullOrEmpty(sessionItem.CommentOnText))
                        {
                            format.align = Alignment.Center;
                            format.textStyle = TextStyle.Normal;
                            doc.insertText("(" + sessionItem.CommentOnText + ")", 14, format, FontColor.Black, TextFont.AdvertisingBold);
                            doc.insertBreakLine(1);
                        }
                        
                    }
                    
                    continue;

                }
                format.textStyle = TextStyle.Bold;
                format.align = Alignment.Right;
                pageNum = doc.GetCurrentPageNumber();
                doc.insertText("* "+item.Text+":", 14, format, FontColor.Black, TextFont.AdvertisingBold);
                index.Add(new MadbatahIndexItem("",item.Text,pageNum,true));
                doc.insertBreakLine(1);

                
                if (item.SubAgendaItems.Count == 0)
                {
                    var list = from it in items
                               orderby it.FragOrderInXml
                               where it.AgendaItemID == item.ID
                               select it;

                    foreach (SessionContentItem sessionItem in list)
                    {
                        pageNum = doc.GetCurrentPageNumber();
                        if (!sessionItem.MergedWithPrevious.Value)
                        {
                            Attendant att = sessionItem.Attendant;
                            format.textStyle = TextStyle.Bold;
                            format.align = Alignment.Right;
                            doc.insertText(att.Name, 14, format, FontColor.Black, TextFont.AdvertisingBold);
                            speakersIndex.Add(new SpeakersIndexItem(att.Name, pageNum));
                            if (!string.IsNullOrEmpty(sessionItem.CommentOnAttendant))
                            {
                                doc.insertText("(" + sessionItem.CommentOnAttendant + ")", 14, format, FontColor.Black, TextFont.AdvertisingBold);
                            }

                            doc.insertBreakLine(1);
                        }
                        format.textStyle = TextStyle.Normal;
                        format.align = Alignment.Right;
                        doc.insertText(sessionItem.Text, 14, format, FontColor.Black, TextFont.AdvertisingBold);
                        if (string.IsNullOrEmpty(sessionItem.PageFooter))
                            doc.AddFooteNote(sessionItem.PageFooter);

                        //if(sessionItem.PageFooter

                        doc.insertBreakLine(1);

                        if (!string.IsNullOrEmpty(sessionItem.CommentOnText))
                        {
                            format.align = Alignment.Center;
                            format.textStyle = TextStyle.Normal;
                            doc.insertText("(" + sessionItem.CommentOnText + ")", 14, format, FontColor.Black, TextFont.AdvertisingMedium);
                            doc.insertBreakLine(1);
                        }
                    }
                }
                else
                {
                    foreach (SessionAgendaItem subItem in item.SubAgendaItems)
                    {
                        format.textStyle = TextStyle.Normal;
                        format.align = Alignment.Right;
                        pageNum = doc.GetCurrentPageNumber();
                        doc.insertText("- " + subItem.Text + ":", 14, format, FontColor.Black, TextFont.AdvertisingBold);
                        index.Add(new MadbatahIndexItem("",subItem.Text ,pageNum,false));
                        doc.insertBreakLine(1);

                        var list = from it in items
                                   orderby it.FragOrderInXml
                                   where it.AgendaItemID == item.ID && it.AgendaSubItemID == subItem.ID
                                   select it;

                        foreach (SessionContentItem sessionItem in list)
                        {
                            pageNum = doc.GetCurrentPageNumber();
                            if (!sessionItem.MergedWithPrevious.Value)
                            {
                                Attendant att = sessionItem.Attendant;
                                format.textStyle = TextStyle.Bold;
                                format.align = Alignment.Right;
                                doc.insertText(att.Name, 14, format, FontColor.Black, TextFont.AdvertisingBold);
                                speakersIndex.Add(new SpeakersIndexItem(att.Name, pageNum));
                                if (!string.IsNullOrEmpty(sessionItem.CommentOnAttendant))
                                {
                                    doc.insertText("(" + sessionItem.CommentOnAttendant + ")", 14, format, FontColor.Black, TextFont.AdvertisingBold);
                                }

                                doc.insertBreakLine(1);
                            }
                            format.textStyle = TextStyle.Normal;
                            format.align = Alignment.Right;
                            doc.insertText(sessionItem.Text, 14, format, FontColor.Black, TextFont.AdvertisingBold);
                            if (string.IsNullOrEmpty(sessionItem.PageFooter))
                                doc.AddFooteNote(sessionItem.PageFooter);

                            //if(sessionItem.PageFooter

                            doc.insertBreakLine(1);

                            if (!string.IsNullOrEmpty(sessionItem.CommentOnText))
                            {
                                format.align = Alignment.Center;
                                format.textStyle = TextStyle.Normal;
                                doc.insertText("(" + sessionItem.CommentOnText + ")", 14, format, FontColor.Black, TextFont.AdvertisingBold);
                                doc.insertBreakLine(1);
                            }
                        }
                    }
                }
            }
            doc.SaveDocument(outFilePath);
            doc.Quit();
        }
        */
        //public static List<SessionContentItem> MergeSessionContentItems(List<SessionContentItem> list)
        //{
        //    List<SessionContentItem> newItems = new List<SessionContentItem>();

        //    foreach (SessionContentItem item in list)
        //    {
        //        if (item.MergedWithID == null)
        //            newItems.Add(item);
        //        else
        //        {
        //            newItems[newItems.Count - 1] = MergeTwoItems(newItems[newItems.Count - 1], item);
        //        }
        //    }
        //    return newItems;
        //}

        //public static SessionContentItem MergeTwoItems(SessionContentItem first, SessionContentItem second)
        //{

        //    if (!string.IsNullOrEmpty(first.CommentOnText))
        //    {
        //        first.Text += "\r\n" + first.CommentOnText;
        //    }

        //    first.CommentOnText = second.CommentOnText;
        //    first.Text += second.Text;

        //    first.PageFooter += second.PageFooter;

        //    return first;
        //}
    }
}
