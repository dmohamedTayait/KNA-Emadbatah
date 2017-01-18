using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.DAL;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Config;
using TayaIT.Enterprise.EMadbatah.Model;
using System.IO;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Localization;
using TayaIT.Enterprise.EMadbatah.Util;


namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class PreReview : BasePage
    {
        public Hashtable tblStats = null;
        int nRejected = -1;
        int nModefiedAfterApprove = -1;
        int nFixed = -1;
        public int bChecked = 0;
        public SessionDetails sd;
        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (!Page.IsPostBack)
            {
                //now the dataentry-reviewr role is using filereviewer privilages instead od session reviewer
                isCurrentUserFileRev.Value = (CurrentUser.Role == UserRole.FileReviewer || CurrentUser.Role == UserRole.ReviewrDataEntry || CurrentUser.Role == UserRole.Reviewer).ToString().ToLower();
                currentUserID.Value = CurrentUser.ID.ToString();
                long sessionFileID;
                StringBuilder sb = new StringBuilder();

                if (SessionFileID != null && long.TryParse(SessionFileID, out sessionFileID))
                {
                    
                    //SessionIDHidden.Value = sessionFileID.ToString();
                   
                    SessionFile sf = SessionFileHelper.GetSessionFileByID(sessionFileID);
                    SessionIDHidden.Value = sf.SessionID.ToString();

                    if (CurrentUser.ID != sf.UserID)
                    {
                        Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);
                        return;
                    }

                    sessionSerial.InnerText = System.IO.Path.GetFileName(sf.Name);

                    List<long> WrittenAgendaItemWithGroupedSubItems = new List<long>();
                    List<string> writtenAgendaItems = new List<string>();

                    List<SessionContentItem> lsCntItems = SessionContentItemHelper.GetSessionContentItemsBySessionFileID(sessionFileID);

                    foreach (SessionContentItem item in lsCntItems)
                    {
                        if (!item.MergedWithPrevious.Value)
                        {
                            Attendant att = item.Attendant;

                            if (att.Type != (int)Model.AttendantType.UnAssigned)
                            {
                                string attFullPresentationName = "";
                                string name = "";
                                string job = "";
                                string job2 = "";
                                if ((Model.AttendantType)att.Type == Model.AttendantType.President)
                                {
                                    string speaker = Application[Constants.HTMLTemplateFileNames.ReviewItemSpeaker].ToString()
                                                   .Replace("<%itemText%>", "السيد الرئيـــــــــــــــــــــــــــس :")
                                                   .Replace("<%speakerJob%>", "")
                                                   .Replace("<%speakerJob2%>", "");
                                    sb.Append(speaker);
                                }
                                else
                                {
                                    if (item.IsSessionPresident == 1)
                                    {
                                        name = "السيد رئيـس الجلســـــــــــــــــــــــة :";

                                        if (att.AttendantTitle == null)
                                            attFullPresentationName = "السيد " + att.Name.Trim();
                                        else attFullPresentationName = att.AttendantTitle.Trim() + " " + att.Name.Trim();
                                        attFullPresentationName = "( " + attFullPresentationName;

                                        if (att.Type != 3)
                                            attFullPresentationName = "    " + item.CommentOnAttendant + ")";
                                        if (att.Type == 3)
                                            job2 = "    " + att.JobTitle + ")";
                                        /* }
                                         else
                                         {
                                             job2 = "    " + item.CommentOnAttendant + ")";
                                         }
                                         */
                                        string speaker = Application[Constants.HTMLTemplateFileNames.ReviewItemSpeaker].ToString()
                                                  .Replace("<%itemText%>", name)
                                                  .Replace("<%speakerJob%>", attFullPresentationName)
                                                  .Replace("<%speakerJob2%>", job2);
                                        sb.Append(speaker);
                                    }
                                    else
                                    {
                                        if (att.AttendantTitle == null)
                                            attFullPresentationName = "السيد " + att.Name.Trim();
                                        else attFullPresentationName = att.AttendantTitle.Trim() + " " + att.Name.Trim();
                                        if (string.IsNullOrEmpty(item.CommentOnAttendant))
                                        {
                                            if (att.Type == 3)
                                                job = att.JobTitle;
                                        }
                                        else
                                        {
                                            job = "    " + item.CommentOnAttendant;
                                        }

                                        string speaker = Application[Constants.HTMLTemplateFileNames.ReviewItemSpeaker].ToString()
                                                 .Replace("<%itemText%>", attFullPresentationName)
                                                 .Replace("<%speakerJob%>", job)
                                                 .Replace("<%speakerJob2%>", "");
                                        sb.Append(speaker);
                                    }
                                }
                            }
                        }

                        string reviewItem = Application[Constants.HTMLTemplateFileNames.ReviewItem].ToString()
                                                .Replace("<%SessionContentItemID%>", item.ID.ToString())
                                                .Replace("<%itemText%>", item.Text);

                        reviewItem = reviewItem.Replace("<%isLocked%>", "openeditem")
                                .Replace("<%title%>", "");



                        if (item.SessionFile.FileReviewerID != null)
                            reviewItem = reviewItem.Replace("<%FileRevID%>", item.SessionFile.FileReviewer.ID.ToString());
                        else
                            reviewItem = reviewItem.Replace("<%FileRevID%>", "");

                        reviewItem = reviewItem.Replace("<%SessionFileID%>", item.SessionFileID.ToString()).Replace("<%RevNote%>", item.ReviewerNote);

                        string mp3FilePath = string.Format("{0}://{1}:{2}/", Request.Url.Scheme, Request.Url.Host, Request.Url.Port) + item.SessionFile.Name.Substring(1).Replace(@"\", "/");
                        reviewItem = reviewItem.Replace("<%MP3FilePath%>", mp3FilePath);
                        reviewItem = reviewItem.Replace("<%MP3FileStartTime%>", item.StartTime.ToString());
                        reviewItem = reviewItem.Replace("<%MP3FileEndTime%>", item.EndTime.ToString());
                        reviewItem = reviewItem.Replace("<%IsSessionStart%>", "0");


                        sb.Append(reviewItem);
                    }
                    madbatahContents.InnerHtml += sb.ToString();
                }
                else
                {
                    ShowMainError(GetLocalizedString("strNoQueryStr"));
                }
            }
        }
    }
}