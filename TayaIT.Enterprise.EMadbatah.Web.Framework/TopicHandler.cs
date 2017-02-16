using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Script.Serialization;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.BLL;
using TayaIT.Enterprise.EMadbatah.Config;
using TayaIT.Enterprise.EMadbatah.Util.Web;
using System.IO;
using System.Security.Principal;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Localization;
using System.Web.Security;

namespace TayaIT.Enterprise.EMadbatah.Web.Framework
{
    public class TopicHandler : BaseHandler
    {

        protected override void HandleRequest()
        {
            WebFunctions.TopicFunctions function;

            if (AjaxFunctionName != null && Enum.TryParse<WebFunctions.TopicFunctions>(AjaxFunctionName, true, out function))
            {
                string jsonStringOut = null;
                long sId = 0;
                long tpcId = 0;
                long res = 0;
                switch (function)
                {
                    case WebFunctions.TopicFunctions.GetSessionTopics:
                        List<Topic> topics = new List<Topic>();
                        List<Topic> retTopics = new List<Topic>();
                        if (SessionID != null && long.TryParse(SessionID, out sId))
                        {
                            topics = TopicHelper.GetAllTopicsBySessionID(sId);
                            Topic tpcObj = new Topic();
                            foreach (Topic tt in topics)
                            {
                                tpcObj = new Topic();
                                tpcObj.ID = tt.ID;
                                tpcObj.Title = tt.Title;
                                retTopics.Add(tpcObj);
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(retTopics);
                        break;
                    case WebFunctions.TopicFunctions.AddTopic:
                        if (TopicTitle != null && long.TryParse(SessionID, out sId))
                        {
                            Topic tpc = new Topic();
                            tpc.Title = TopicTitle;
                            tpc.UserID = CurrentUser.ID;
                            tpc.SessionID = sId;
                            tpc.CreatedAt = DateTime.Now;

                            res = TopicHelper.AddTopic(tpc);
                        }
                        jsonStringOut = SerializeObjectInJSON(res);
                        break;
                    case WebFunctions.TopicFunctions.EditTopic:
                        if (TopicTitle != null && long.TryParse(TopicID, out tpcId))
                        {
                            res = TopicHelper.UpdateTopicById(tpcId,TopicTitle);
                        }
                        jsonStringOut = SerializeObjectInJSON(res);
                        break;
                    case WebFunctions.TopicFunctions.DeleteTopic:
                        if (TopicID != null)
                        {
                            res = TopicHelper.DeleteTopicByTopicID(long.Parse(TopicID));
                        }
                        jsonStringOut = SerializeObjectInJSON(res);
                        break;
                    case WebFunctions.TopicFunctions.GetAllAtt:
                        Attendant att = new Attendant();
                        List<Attendant> attLst = new List<Attendant>();
                        if (SessionID != null)
                        {
                            Session current_session = EditorFacade.GetSessionByID(long.Parse(SessionID));
                            List<TopicAttendant> tpcAttLst = TopicHelper.GetTopicAttsByTopicID(long.Parse(TopicID));
                            foreach (Attendant item in current_session.Attendants.Where(c => c.SessionAttendantType == current_session.SessionStartFlag && (c.Type != (int)Model.AttendantType.UnAssigned)).OrderBy(s => s.CreatedAt).ThenBy(s => s.LongName))
                            {
                                att = new Attendant();
                                att.FirstName = "0";
                                TopicAttendant tmpobj = tpcAttLst.FirstOrDefault(c => c.AttendantID == item.ID);
                                if(tmpobj != null)
                                    att.FirstName = "1";
                                att.ID = item.ID;
                                att.Name = item.Name;
                                att.JobTitle = item.JobTitle;
                                att.Type = item.Type;
                                att.ShortName = item.ShortName;
                                att.LongName = item.LongName;
                                att.AttendantTitle = item.AttendantTitle;
                              
                                attLst.Add(att);
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(attLst);
                        break;
                    case WebFunctions.TopicFunctions.AddTopicAtt:
                        if (TopicID != null)
                        {
                            List<TopicAttendant> tpcAttLst = TopicHelper.GetTopicAttsByTopicID(long.Parse(TopicID));
                            foreach (TopicAttendant tpcAttObj in tpcAttLst)
                            {
                                res = TopicHelper.DeleteTopicAttendant(long.Parse(TopicID), (long)tpcAttObj.AttendantID);
                            }
                            List<string> attIDS = serializer.Deserialize<List<string>>(JsonStr);
                            foreach (string attIDStr in attIDS)
                            {
                                res = TopicHelper.AddTopicAttendant(long.Parse(TopicID), long.Parse(attIDStr));
                            }
                        }
                        jsonStringOut = SerializeObjectInJSON(res);
                        break;

                    case WebFunctions.TopicFunctions.AddTopicParag:
                        if (TopicID != null && long.TryParse(TopicID, out tpcId) && TopicParag != null)
                        {
                            TopicParagraph tpcParag = new TopicParagraph();
                            tpcParag.ParagraphText = TopicParag;
                            tpcParag.ParagraphAlignment = TopicParagAlignment == null ? 1 : int.Parse(TopicParagAlignment);
                            tpcParag.TopicID = tpcId;

                            res = TopicHelper.AddTopicParagraph(tpcParag);
                        }
                        jsonStringOut = SerializeObjectInJSON(res);
                        break;
                    case WebFunctions.TopicFunctions.EditTopicParag:
                        if (TopicParagID != null && TopicParag != null)
                        {
                            res = TopicHelper.UpdateTopicParagraphById(long.Parse(TopicParagID), TopicParag, TopicParagAlignment == null ? 1 : int.Parse(TopicParagAlignment));
                        }
                        jsonStringOut = SerializeObjectInJSON(res);
                        break;
                    case WebFunctions.TopicFunctions.DeleteTopicParag:
                        if (TopicParagID != null)
                        {
                            res = TopicHelper.DeleteTopicParagByTopicParagraphID(long.Parse(TopicParagID));
                        }
                        jsonStringOut = SerializeObjectInJSON(res);
                        break;
                }

                if (jsonStringOut != null)
                {
                    _context.Response.AddHeader("Encoding", "UTF-8");
                    _context.Response.Write(jsonStringOut);
                }
            }
            else
                return;
        }

        public string TopicID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.TOPIC_ID, _context);                
            }
        }

        public string TopicTitle
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.TOPIC_TITLE, _context);
            }
        }

        public string TopicParagID
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.TOPIC_PARAG_ID, _context);
            }
        }

        public string TopicParag
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.TOPIC_PARAG, _context);
            }
        }

        public string TopicParagAlignment
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.TOPIC_PARAG_ALIGN, _context);
            }
        }

        public string JsonStr
        {
            get
            {
                return WebHelper.GetQSValue(Constants.QSKeyNames.JSON_STR, _context);
            }
        }

    }//end class
}//end namespace
