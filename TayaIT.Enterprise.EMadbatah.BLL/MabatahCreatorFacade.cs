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
            ///////////////Start////////////////////////////////////
            //WordDocument docStart = new WordDocument();
            //WordDocument AttachCover = new WordDocument();
            //WordDocument doc = new WordDocument();
            try
            {
                int coverSize = 1;
                List<MadbatahIndexItem> index = new List<MadbatahIndexItem>();
                List<SpeakersIndexItem> speakersIndex = new List<SpeakersIndexItem>();
                TayaIT.Enterprise.EMadbatah.Model.SessionDetails details = SessionStartFacade.GetSessionDetails(sessionID);
                //create madbatahCover
                CreateMadbatahCover(details, SessionWorkingDir + "coverDoc.docx", ServerMapPath);


                int sessionStartSize = 0;
                SessionFile start = SessionStartFacade.GetSessionStartBySessionID(sessionID);
                //HtmlToOpenXml.SaveHtmlToWord(start.SessionStartText, SessionWorkingDir + "sessionStartDoc.docx", ServerMapPath+ "\\resources\\", out sessionStartSize);
                HTMLtoDOCX hd = new HTMLtoDOCX();
                hd.CreateFileFromHTML(start.SessionStartText, SessionWorkingDir + "sessionStartDoc.docx");


                using (WordprocessingWorker doc = new WordprocessingWorker(SessionWorkingDir + "sessionStartDoc.docx", WordprocessingWorker.GetDocParts(ServerMapPath + "\\resources\\"), DocFileOperation.Open))
                {
                    WordprocessingWorker doctmp = doc;
                    sessionStartSize = doc.CountPagesUsingOpenXML(doc, SessionWorkingDir + "sessionStartDoc.docx", WordprocessingWorker.GetDocParts(ServerMapPath + "\\resources\\"), ServerMapPath, out doctmp);//CountPagesUsingOpenXML(DocumentType.DOCX, folderPath + att.Name);
                    doctmp.Dispose();
                }



                int bodySize = MabatahCreatorFacade.CreateMadbatahBody(sessionID, SessionWorkingDir + "body.docx", SessionWorkingDir, ServerMapPath, details, out index, out speakersIndex);
                //, out sessionName, out sessionNameForFooter);
                if (bodySize == -1)
                    throw new Exception("Madbatah Body Creation Failed.");


                List<string> mergeList = new List<string>();
                //mergeList.Add(SessionWorkingDir + "coverDoc.docx");//done
                mergeList.Add(SessionWorkingDir + "indexDoc.docx");

                
                ////attachments
                List<Attachement> attachments = AttachmentHelper.GetSessionAttachments(sessionID);
               /* foreach (Attachement att in attachments)
                {
                    System.IO.File.WriteAllBytes(SessionWorkingDir + att.Name, att.FileContent);
                    FileInfo fInfo = new FileInfo(SessionWorkingDir + att.Name);

                    if (!fInfo.Extension.ToLower().Equals(".pdf"))
                        mergeList.Add(SessionWorkingDir + att.Name);
                    else
                    {
                        String pdfFilePath = SessionWorkingDir + att.Name;
                        pdf2ImageConvert.convertPdfFile(pdfFilePath);
                        string wordAttFilePath = SessionWorkingDir + att.Name.ToLower().Replace(".pdf", ".pdf.docx");
                        string[] files = Directory.GetFiles(SessionWorkingDir + fInfo.Name.Replace(fInfo.Extension, ""));
                        //WordDocument pdfAttDoc = new WordDocument();
                        //pdfAttDoc.CreateNew();

                        // WordprocessingWorker.InsertAPicture(wordAttFilePath, files);

                        ImageWriter.CreateImageDocument(wordAttFilePath, ServerMapPath + "\\resources\\", files);


                        //foreach (string file in files)
                        //    pdfAttDoc.InsertImage(file);
                        ////int pagenos =  getNumberOfPdfPages(pdfFilePath);
                        ////convert(pdfFilePath, pagenos);

                        //pdfAttDoc.SaveDocument(SessionWorkingDir + att.Name.ToLower().Replace(".pdf", ".pdf.doc"));
                        //pdfAttDoc.Quit();
                        mergeList.Add(SessionWorkingDir + att.Name.ToLower().Replace(".pdf", ".pdf.docx"));
                        att.Name = att.Name.ToLower().Replace(".pdf", ".pdf.docx");
                        //use waleed function
                    }
                }
                */

                //int indexSize = MabatahCreatorFacade.CreateMadbatahIndex(index, folderPath + "coverDoc1.doc", folderPath + "indexDoc1.doc", details);//, sessionName );
                int allAttachSizes = 0;
                int indexSize = 0;
                indexSize = MabatahCreatorFacade.CreateMadbatahIndexWithAttachment(
                        index, attachments, SessionWorkingDir, indexSize, sessionStartSize, bodySize,
                        SessionWorkingDir + "indexDoc.docx", ServerMapPath,
                        details, false, out allAttachSizes);//, sessionName );

                if (indexSize == -1)
                    throw new Exception("Index with Attachment Creation Failed.");
                for (int i = 0; i < index.Count; i++)
                {
                    string[] parts = index[i].PageNum.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    string newPageNum = "";
                    List<int> pages = new List<int>();
                    foreach (string part in parts)
                    {
                        int p = int.Parse(part) + coverSize + sessionStartSize + indexSize - 1;//1 cover
                        if (pages.IndexOf(p) == -1)
                        {
                            pages.Add(p);
                            newPageNum += p + ",";
                        }
                    }
                    newPageNum = newPageNum.Remove(newPageNum.Length - 1);
                    index[i].PageNum = newPageNum;
                    //index[i].PageNum += coverSize + sessionStartSize + indexSize - 1;//1 cover
                }
                int attachCover = 1;

                int speakerSize = MabatahCreatorFacade.CreateSpeakersIndex(speakersIndex,
                    coverSize + sessionStartSize + indexSize - 1, //2 for att cover + speakers cover
                    ServerMapPath, SessionWorkingDir + "indexSpeakers.docx");

                //mergeList.Add(SessionWorkingDir + "indexSpeakers.doc");
                if (speakerSize == -1)
                    throw new Exception("Speakers Index Creation Failed.");


                indexSize = MabatahCreatorFacade.CreateMadbatahIndexWithAttachment(
                    index, attachments, SessionWorkingDir,
                    indexSize, sessionStartSize, bodySize,
                    SessionWorkingDir + "indexDoc.docx", ServerMapPath,
                    details, true, out allAttachSizes);//, sessionName );
                if (indexSize == -1)
                    throw new Exception("Index with Attachment Creation Failed.");


                mergeList.Add(SessionWorkingDir + "indexSpeakers.docx");//done
                mergeList.Add(ServerMapPath + "\\docs\\BodyCover.docx");//ready
                mergeList.Add(SessionWorkingDir + "sessionStartDoc.docx");//done
                mergeList.Add(SessionWorkingDir + "body.docx");
                mergeList.Add(ServerMapPath + "\\docs\\EndCover.docx");//ready

                //List<byte[]> docs = new List<byte[]>();
                //foreach (string file in mergeList)
                //    docs.Add(File.ReadAllBytes(file));

                //File.WriteAllBytes(SessionWorkingDir + "merged.docx", WordprocessingWorker.OpenAndCombine(docs));
                //WordMerger.MergeDocs(mergeList.ToArray(), SessionWorkingDir + sessionID + ".docx", true);
                File.Copy(SessionWorkingDir + "coverDoc.docx", SessionWorkingDir + sessionID + ".docx", true);
                WordprocessingWorker.MergeWithAltChunk(SessionWorkingDir + sessionID + ".docx", mergeList.ToArray());

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

        public static List<SessionContentItem> GroupSpeakerSimilarArticles(List<SessionContentItem> groupedItems)
        {
            List<SessionContentItem> newGroup = new List<SessionContentItem>();
            foreach (SessionContentItem item in groupedItems)
            {
                if(newGroup.Count == 0)//called once
                    newGroup.Add(item);
                else
                    if (item.MergedWithPrevious == true)
                    {
                        newGroup[newGroup.Count - 1].Text += " " + item.Text;
                        //if(!string.IsNullOrEmpty(newGroup[newGroup.Count - 1].CommentOnAttendant ) &&
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
                attName = ((att.FirstName == null ? "" : att.FirstName )+ " " + (att.SecondName== null ? "" : att.SecondName) + " " + (att.TribeName== null ? "" : att.TribeName)).Replace("   ", " ").Replace("  ", " ").Trim();
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

        public static string GetAttendantTitle(Object SomeObj,long session_id)
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

            int attendant_title_id = BLL.EMadbatahFacade.get_session_attendant_title_id(session_id, att.ID);
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

            return (title + attName).Trim();

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

        public static int CreateMadbatahBody(long sessionID, string outFilePath, string SessionWorkingDir, string ServerMapPath, Model.SessionDetails details, out List<MadbatahIndexItem> index, out List<SpeakersIndexItem> speakersIndex
            )//, out string sessionName, out string sessionNameForFooter)
        {

            string docPath = outFilePath;

            string resfolderpath =ServerMapPath + "\\resources\\";
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
                //using (WordprocessingWorker doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.CreateNew))
                WordprocessingWorker doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.CreateNew);
                {

                    index = new List<MadbatahIndexItem>();
                    speakersIndex = new List<SpeakersIndexItem>();
                    List<string> writtenAgendaItems = new List<string>();

                    List<List<SessionContentItem>> allItems = SessionContentItemHelper.GetItemsBySessionIDGrouped(sessionID);

                    //should merge items by same speaker
                    //should be merged with previous, if same speaker and same agenda item
                    //used when last item in a file is merged with first item in next file, meshmesh
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

                            ////attachments
                            if (ii == 1)
                            {
                                List<Attachement> attachments = AttachmentHelper.GetSessionAttachments(sessionID);
                                Attachement att = attachments[0];
                                System.IO.File.WriteAllBytes(SessionWorkingDir + att.Name, att.FileContent);
                                FileInfo fInfo = new FileInfo(SessionWorkingDir + att.Name);

                                if (fInfo.Extension.ToLower().Equals(".pdf"))
                                {
                                    String pdfFilePath = SessionWorkingDir + att.Name;
                                    pdf2ImageConvert.convertPdfFile(pdfFilePath);
                                    string wordAttFilePath = SessionWorkingDir + att.Name.ToLower().Replace(".pdf", ".pdf.docx");
                                    string[] files = Directory.GetFiles(SessionWorkingDir + fInfo.Name.Replace(fInfo.Extension, ""));

                                    foreach (string f in files)
                                    {
                                        ImageWriter.AddImage(doc.DocMainPart.Document.Body, doc.DocMainPart, f, "rId" + ii);
                                    }
                                }
                                //ImageWriter.CreateImageDocument(wordAttFilePath, ServerMapPath + "\\resources\\", files);

                            }
                            if (ii == 2)
                            {
                                List<Attachement> attachments = AttachmentHelper.GetSessionAttachments(sessionID);
                                Attachement att = attachments[1];
                                System.IO.File.WriteAllBytes(SessionWorkingDir + att.Name, att.FileContent);
                                FileInfo fInfo = new FileInfo(SessionWorkingDir + att.Name);

                                if (fInfo.Extension.ToLower().Equals(".pdf"))
                                {
                                    String pdfFilePath = SessionWorkingDir + att.Name;
                                    pdf2ImageConvert.convertPdfFile(pdfFilePath);
                                    string wordAttFilePath = SessionWorkingDir + att.Name.ToLower().Replace(".pdf", ".pdf.docx");
                                    string[] files = Directory.GetFiles(SessionWorkingDir + fInfo.Name.Replace(fInfo.Extension, ""));

                                    foreach (string f in files)
                                    {
                                        ImageWriter.AddImage(doc.DocMainPart.Document.Body, doc.DocMainPart, f, "rId" + ii);
                                    }
                                }
                                //ImageWriter.CreateImageDocument(wordAttFilePath, ServerMapPath + "\\resources\\", files);

                            }
                            //commeneted 12-04-2012
                            //index.Add(new MadbatahIndexItem(curAgendaItem.ID , originalName, pageNum, true, "", "", curAgendaItem.IsCustom, curAgendaItem.IsGroupSubAgendaItems));

                            //int ind = index.IndexOf(new MadbatahIndexItem(curAgendaItem.ID, originalName, pageNum+"", true, "", "", curAgendaItem.IsCustom, curAgendaItem.IsGroupSubAgendaItems));
                            int ind = index.FindIndex(curIndexItem => curIndexItem.ID == curAgendaItem.ID);


                            if (ind == -1)
                                index.Add(new MadbatahIndexItem(curAgendaItem.ID, originalName, pageNum+"", true, "", "", curAgendaItem.IsCustom, curAgendaItem.IsGroupSubAgendaItems));
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
                                    index.Add(new MadbatahIndexItem(curAgendaSubItem.ID, curAgendaSubItem.Name, pageNum+"", false, curAgendaSubItem.QFrom, curAgendaSubItem.QTo, curAgendaSubItem.IsCustom, curAgendaItem.IsGroupSubAgendaItems));
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
                                index.Add(new MadbatahIndexItem(curAgendaSubItem.ID, curAgendaSubItem.Name, pageNum+"", false, curAgendaSubItem.QFrom, curAgendaSubItem.QTo, curAgendaSubItem.IsCustom, curAgendaItem.IsGroupSubAgendaItems));
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
                                    //doc.insertText("(" + sessionItem.CommentOnAttendant + ")", 14, format, FontColor.Black, TextFont.Arial);
                                    doc.AddParagraph(MabatahCreatorFacade.GetAttendantTitle(att,sessionID) + ": " + "(" + sessionItem.CommentOnAttendant + ")", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
                                }
                                else
                                    doc.AddParagraph(MabatahCreatorFacade.GetAttendantTitle(att, sessionID) + ":", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");

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

                                string tt = TextHelper.StripHTML(part.ToLower()).Trim() + " ";

                                doc.AddParagraph(tt.Replace("&nbsp;", " "), ParagraphStyle.NormalArabic, ParagrapJustification.Both, false, "");
                                //doc.AddParagraph("", ParagraphStyle.ParagraphTitle, ParagrapJustification.RTL, false, "");
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
                                    string tempDir = ServerMapPath +"Corrupted files\\"+ System.IO.Path.GetFileName(sf.Name.Replace(".mp3", ""));
                                    System.IO.Directory.CreateDirectory(tempDir);
                                    fileName = sf.Name.Split('\\')[3].Replace("mp3", "txt");
                                    FileStream fs = System.IO.File.Create(tempDir + "\\" + fileName);
                                    fs.Close();
                                    System.IO.File.WriteAllText(tempDir + "\\" + fileName, fileText);
                                    System.IO.File.Copy(ServerMapPath + sf.Name, tempDir +"\\" +System.IO.Path.GetFileName(sf.Name), true);
                                    System.IO.File.Copy(ServerMapPath + sf.Name.Replace("mp3", "trans.xml"), tempDir + "\\" + System.IO.Path.GetFileName(sf.Name.Replace("mp3", "trans.xml")), true);
                                    }
                            }

                        }
                        
                    }
                    
                    //07-06-2012
                    doc.AddParagraph(""
                        , ParagraphStyle.NormalArabic, ParagrapJustification.Both, false, "");
                    doc.AddParagraph("د. محمد سالم المزروعي                                                                 محمد أحمد المر"
                        , ParagraphStyle.NormalArabic, ParagrapJustification.Both, false, "");
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

            //sessionName = EMadbatahFacade.GetSessionName(details.Season, details.Stage, details.Serial);
            //sessionNameForFooter = EMadbatahFacade.GetSessionName(details.Season, details.Stage, details.Serial);
        }
        public static void CreateMadbatahCover(Model.SessionDetails details, string outCoverPath, string ServerMapPath)
        {
            File.Copy(ServerMapPath + "\\docs\\templates\\madbatahCover-template.docx", outCoverPath, true);
              
                //calculate hijri date
                //details.Date = details.Date.Subtract(new TimeSpan(1, 0, 0));
                DateTimeFormatInfo dateFormat = Util.DateUtils.ConvertDateCalendar(details.Date, Util.CalendarTypes.Hijri, "en-us");
                //DateTime hijDate = details.Date.ToString("f", dateFormat);

                string dayNameAr = details.Date.ToString("dddd", dateFormat); // LocalHelper.GetLocalizedString("strDay" + hijDate.DayOfWeek);
                string monthNameAr = LocalHelper.GetLocalizedString("strMonth" + details.Date.Month);
                string monthNameHijAr = details.Date.ToString("MMMM", dateFormat); //LocalHelper.GetLocalizedString("strHijMonth"+hijDate.Month);
                string dayOfMonthNumHij = details.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("dd", dateFormat);//hijDate.Day;
                string yearHij = details.Date.ToString("yyyy", dateFormat);  //hijDate.Year;


                //for header
                string sessionNum = details.Subject; //"الخامسة عشره";
                string hijriDate = dayNameAr + " " + dayOfMonthNumHij + " " + monthNameHijAr + " سنة " + yearHij + " هـ";//" 10 رجب سنة 1431 ه";//"الثلاثاء 10 رجب سنة 1431 ه";
                string gDate = " "+details.Date.Day + " " + monthNameAr + " سنة " + details.Date.Year + " م "; //"22 يونيو سنة 2010 م";

                NumberingFormatter fomratterFemale = new NumberingFormatter(false);
                NumberingFormatter fomratterMale = new NumberingFormatter(true);
                string sessionName = details.EparlimentID.ToString() + "/" + details.Type;//EMadbatahFacade.GetSessionName(details.Season, details.Stage, details.Serial);

            string footer = details.Subject + "- الدور " + details.Stage + "- الفصل " + details.Season.ToString();//"5/5/14");
                ///for 3'olaf
                ///
            StringBuilder sb=new StringBuilder();
                    sb.Append("<root>");

            sb.Append("<Name>").Append(sessionName).Append("</Name>");
            sb.Append("<Season>").Append(fomratterMale.getResultEnhanced((int)details.Season)).Append("</Season>");
            sb.Append("<StageType>").Append("ال" + details.StageType).Append("</StageType>");
            sb.Append("<Stage>").Append(fomratterMale.getResultEnhanced((int)details.Stage)).Append("</Stage>");
            sb.Append("<Subject>").Append(details.Subject).Append("</Subject>");
            sb.Append("<DateHijri>").Append(hijriDate).Append("</DateHijri>");
            sb.Append("<DateMilady>").Append(gDate).Append("</DateMilady>");
            sb.Append("<StartTime>").Append(details.StartTime.ToLongTimeString()).Append("</StartTime>");
           // sb.Append("<Footer>").Append(footer).Append("</Footer>");
            sb.Append("</root>");
     
            WordTemplateHandler.replaceCustomXML(outCoverPath,sb.ToString());
        }

        public static void CreateMadbatahStart(Model.SessionDetails details, string outStartPath, string ServerMapPath)
        {

            File.Copy(ServerMapPath + "\\docs\\templates\\madbatahStart-template.docx", outStartPath,true);
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
                    "<p align=\"center\"><font size=\"14\" face=\"arial\">"
                    + "<table width=\"100%\" border=\"0\" align=\"right\" cellpadding=\"3\" cellspacing=\"3\" style=\"writing-mode: tb-rl; text-align:right; direction: rtl; font-family: arial; font-size: 14pt;\">"
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

                //doc.insertText("فهرس المحتويات", 14, format, FontColor.Black, TextFont.Arial);
                //doc.insertBreakLine(1);
                //doc.insertText("البند                     الموضوع                  رقم الصفحة"
                //    , 14, format, FontColor.Black, TextFont.Arial);
                //doc.insertBreakLine(1);
                //format.align = Alignment.Right;
                int i = 1;
                int coverSize = 1;
                //int charsBold = 94;
                //int charsNorm = 99;
                string emptyRowBold = "<strong><tr align=\"right\" style=\"font-family: arial;font-size: 14pt;\"><td>ItemNum</td><td>ItemName</td><td>PageNum</td></tr></strong>";
                string emptyRow = "<tr align=\"right\" style=\"font-family: arial;font-size: 14pt;\"><td>ItemNum</td><td>ItemName</td><td>PageNum</td></tr>";
                StringBuilder sb = new StringBuilder();
                sb.Append(indexHeader);
     

                foreach (MadbatahIndexItem item in index)
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
                            pageNum = nextPageNum + "-" +  pageNum ;
               
                        string toBeReplaced = emptyRowBold.Replace("ItemNum", i.ToString()).Replace("ItemName", name).Replace("PageNum", pageNum);
                        
                      
                        sb.Append(toBeReplaced);
                    }
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

        public static int CreateSpeakersIndex(List<SpeakersIndexItem> index, int increment,string ServerMapPath, string outPath)
        {
            //WordDocument doc = new WordDocument();
            //doc.CreateNew();
            //doc.initializeStyle();
            //BasicFormat format = new BasicFormat();
            //format.align = Alignment.Right;
            //format.textStyle = TextStyle.Bold;

            //format.align = Alignment.Center;
            //doc.insertText("فهرس المتحدثين", 14, format, FontColor.Black, TextFont.Arial);
            //doc.insertBreakLine(1);
            //doc.insertText("الاسم                               رقم الصفحة"
            //    , 14, format, FontColor.Black, TextFont.Arial);
            //doc.insertBreakLine(1);
            //format.align = Alignment.Right;
            //int charsNorm = 99;

                //<p align=\"center\"><font face=\"arial\" size=\"14pt\">فهرس المتحدثين</font></p>
           
            try
            {
                string indexHeader = "<font size=\"14\" face=\"arial\"><div style\"text-align:center;text-decoration:underline; font-family: arial; font-size: 14pt;\">فهرس المتحدثين</div><table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"5\" cellspacing=\"5\" style=\"writing-mode: tb-rl; text-align:right; direction: rtl; font-family: arial; font-size: 14pt;\">"
                + "<tr style=\";\"><th colspan=\"2\" align=\"center\" scope=\"col\">السادة الاعضاء المتحدثون على الموضوعات التى تم مناقشتها أثناء انعقاد الجلسة</th></tr>"
                + "<tr style=\";\"><th scope=\"col\">المتحدث</th><th scope=\"col\">ارقام الصفحات</th></tr>";
                int i = 1;
                string emptyRowBold = "<strong><tr align=\"right\" style=\"font-family: arial;font-size: 14pt;\"><td>ItemName</td><td>PageNum</td></tr></strong>";
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
                    //sb.Append(emptyRowBold.Replace("ItemNum", i.ToString()).Replace("ItemName", MabatahCreatorFacade.GetAttendantTitle(item.Name, item.attType) + item.Name.Trim()).Replace("PageNum", pagesStr));
                    sb.Append(emptyRowBold.Replace("ItemName", item.Name.Trim()).Replace("PageNum", pagesStr));
                    i++;
                    //string str = item.Name.PadLeft(charsNorm - ((item.Name.Length % 90) + pagesStr.ToString().Length  - 1), ' ');// +"     ";
                    ////str += item.PageNum;
                    //
                    //doc.insertText(str, 14, format, FontColor.Black, TextFont.Arial);
                    //doc.insertBreakLine(1);
                }
                sb.Append("</table></font>");

                //StreamWriter sw = new StreamWriter(outPath + ".htmlhtml" , false, Encoding.UTF8);
                //sw.Write(sb.ToString());
                //sw.WriteLine();
                //sw.Close();

                //docStart.OpenForHtml(outPath.Replace(".doc", ".htmlhtml"));

                ////docStart.ReplaceWithPageBreak("mcenoneditable");
                ////doc.InsertPageNumInFooterWithSessionNum("4/13/4");
                //docStart.SaveDocument(outPath);


                //int stats = docStart.GetCurrentPageNumber();
                /////doc.SaveDocument(outPath);
                //docStart.Quit();

                int stats = 0;
                //HtmlToOpenXml.SaveHtmlToWord(sb.ToString(), outPath, ServerMapPath + "\\resources\\", out stats);
                HTMLtoDOCX hd = new HTMLtoDOCX();
                hd.CreateFileFromHTML(sb.ToString(), outPath);

                //TayaIT.Enterprise.EMadbatah.Word.WordDocument doc = new TayaIT.Enterprise.EMadbatah.Word.WordDocument();
                //doc.initializeStyle();
                //doc.OpenForHtml(outPath + ".htmlhtml");
                //doc.SaveDocument(outPath);
                //doc.Quit();
                return stats;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.BLL.MabatahCreatorFacade.CreateMadbatahIndex(" + ")");
                return -1;
            }
            //int stats = doc.GetCurrentPageNumber();
            //doc.SaveDocument(outPath);
            //doc.Quit();
            //return stats;
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
                            doc.insertText(att.Name, 14, format, FontColor.Black, TextFont.Arial);
                            speakersIndex.Add(new SpeakersIndexItem(att.Name, pageNum));
                            if (!string.IsNullOrEmpty(sessionItem.CommentOnAttendant))
                            {
                                doc.insertText("(" + sessionItem.CommentOnAttendant + ")", 14, format, FontColor.Black, TextFont.Arial);
                            }

                            doc.insertBreakLine(1);
                        }
                        format.textStyle = TextStyle.Normal;
                        format.align = Alignment.Right;
                        doc.insertText(sessionItem.Text, 14, format, FontColor.Black, TextFont.Arial);
                        if (string.IsNullOrEmpty(sessionItem.PageFooter))
                            doc.AddFooteNote(sessionItem.PageFooter);

                        //if(sessionItem.PageFooter

                        doc.insertBreakLine(1);

                        if (!string.IsNullOrEmpty(sessionItem.CommentOnText))
                        {
                            format.align = Alignment.Center;
                            format.textStyle = TextStyle.Normal;
                            doc.insertText("(" + sessionItem.CommentOnText + ")", 14, format, FontColor.Black, TextFont.Arial);
                            doc.insertBreakLine(1);
                        }
                        
                    }
                    
                    continue;

                }
                format.textStyle = TextStyle.Bold;
                format.align = Alignment.Right;
                pageNum = doc.GetCurrentPageNumber();
                doc.insertText("* "+item.Text+":", 14, format, FontColor.Black, TextFont.Arial);
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
                            doc.insertText(att.Name, 14, format, FontColor.Black, TextFont.Arial);
                            speakersIndex.Add(new SpeakersIndexItem(att.Name, pageNum));
                            if (!string.IsNullOrEmpty(sessionItem.CommentOnAttendant))
                            {
                                doc.insertText("(" + sessionItem.CommentOnAttendant + ")", 14, format, FontColor.Black, TextFont.Arial);
                            }

                            doc.insertBreakLine(1);
                        }
                        format.textStyle = TextStyle.Normal;
                        format.align = Alignment.Right;
                        doc.insertText(sessionItem.Text, 14, format, FontColor.Black, TextFont.Arial);
                        if (string.IsNullOrEmpty(sessionItem.PageFooter))
                            doc.AddFooteNote(sessionItem.PageFooter);

                        //if(sessionItem.PageFooter

                        doc.insertBreakLine(1);

                        if (!string.IsNullOrEmpty(sessionItem.CommentOnText))
                        {
                            format.align = Alignment.Center;
                            format.textStyle = TextStyle.Normal;
                            doc.insertText("(" + sessionItem.CommentOnText + ")", 14, format, FontColor.Black, TextFont.Arial);
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
                        doc.insertText("- " + subItem.Text + ":", 14, format, FontColor.Black, TextFont.Arial);
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
                                doc.insertText(att.Name, 14, format, FontColor.Black, TextFont.Arial);
                                speakersIndex.Add(new SpeakersIndexItem(att.Name, pageNum));
                                if (!string.IsNullOrEmpty(sessionItem.CommentOnAttendant))
                                {
                                    doc.insertText("(" + sessionItem.CommentOnAttendant + ")", 14, format, FontColor.Black, TextFont.Arial);
                                }

                                doc.insertBreakLine(1);
                            }
                            format.textStyle = TextStyle.Normal;
                            format.align = Alignment.Right;
                            doc.insertText(sessionItem.Text, 14, format, FontColor.Black, TextFont.Arial);
                            if (string.IsNullOrEmpty(sessionItem.PageFooter))
                                doc.AddFooteNote(sessionItem.PageFooter);

                            //if(sessionItem.PageFooter

                            doc.insertBreakLine(1);

                            if (!string.IsNullOrEmpty(sessionItem.CommentOnText))
                            {
                                format.align = Alignment.Center;
                                format.textStyle = TextStyle.Normal;
                                doc.insertText("(" + sessionItem.CommentOnText + ")", 14, format, FontColor.Black, TextFont.Arial);
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
