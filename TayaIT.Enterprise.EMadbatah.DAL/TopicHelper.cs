using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Util;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public class TopicHelper
    {
        public static List<Topic> GetAllTopicsBySessionID(long sessionID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Topic> Topics = context.Topics.Where(c => c.SessionID == sessionID).Select(c => c).ToList();
                    return Topics;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.GetAllTopicsBySessionID(" + sessionID.ToString() + ")");
                return null;
            }
        }
        
        public static Topic GetTopicByID(long topicID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Topic Topic = context.Topics.Where(c => c.ID == topicID).Select(c => c).FirstOrDefault();
                    return Topic;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.GetTopicsByID()");
                return null;
            }
        }

        public static List<TopicParagraph> GetTopicParagsByTopicID(long topicID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<TopicParagraph> allTopics = context.TopicParagraphs.Where(c => c.TopicID == topicID).ToList();
                    return allTopics;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.GetTopicParagsByTopicID(" + topicID + ")");
                return null;
            }
        }

        public static List<TopicAttendant> GetTopicAttsByTopicID(long topicID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<TopicAttendant> allTopics = context.TopicAttendants.Where(c => c.TopicID == topicID).ToList();
                    return allTopics;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.GetTopicParagsByTopicID(" + topicID + ")");
                return null;
            }
        }

        public static long AddTopic(Topic topicObj)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                        context.Topics.AddObject(topicObj);
                        int result = context.SaveChanges();
                        return topicObj.ID;
                    }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.AddNewDefaultAttendant(defAttObj)");
                return 0;
            }
        }

        public static long AddTopicParagraph(TopicParagraph topicParagObj)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    context.TopicParagraphs.AddObject(topicParagObj);
                    int result = context.SaveChanges();
                    return topicParagObj.ID;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.AddTopicParagraph(defAttObj)");
                return 0;
            }
        }

        public static int UpdateTopicById(long topicID, string topicTitle)
        {
            try
            {
                Topic topicForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    topicForUpdate = context.Topics.FirstOrDefault(c => c.ID == topicID);
                    if (topicForUpdate != null)
                    {
                        topicForUpdate.Title = topicTitle;
                    }

                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.UpdateTopicById(" + topicID + "," + topicTitle + ")");
                return 0;
            }
        }

        public static int UpdateTopicParagraphById(long topicParagID, string paragText,int paragAlignment)
        {
            try
            {
                TopicParagraph topicForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    topicForUpdate = context.TopicParagraphs.FirstOrDefault(c => c.ID == topicParagID);
                    if (topicForUpdate != null)
                    {
                        topicForUpdate.ParagraphText = paragText;
                        topicForUpdate.ParagraphAlignment = paragAlignment;
                    }

                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.UpdateTopicById(" + topicParagID + "," + paragText + "," + paragAlignment + ")");
                return 0;
            }
        }

        public static long DeleteTopicByTopicID(long topicID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    Topic chkIftopicExist = context.Topics.FirstOrDefault(c => c.ID == topicID);
                    if (chkIftopicExist != null)
                    {
                        List<TopicAttendant> chkIftpcAttExist = context.TopicAttendants.Where(c => c.TopicID == topicID).ToList();
                        {
                            foreach (TopicAttendant tpcAttObj in chkIftpcAttExist)
                            {
                                context.DeleteObject(tpcAttObj);
                                context.SaveChanges();
                            }
                        }

                        List<TopicParagraph> chkIfParagExist = context.TopicParagraphs.Where(c => c.TopicID == topicID).ToList();
                        {
                            foreach (TopicParagraph tpcParagObj in chkIfParagExist)
                            {
                                context.DeleteObject(tpcParagObj);
                                context.SaveChanges();
                            }
                        }


                        context.DeleteObject(chkIftopicExist);
                        context.SaveChanges();
                        return 1;
                    }
                    else
                        return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.DeleteTopicByTopicID(" + topicID + ")");
                return 0;
            }
        }
      
        public static long DeleteTopicParagByTopicParagraphID(long topicParagID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    TopicParagraph chkIfAttExist = context.TopicParagraphs.FirstOrDefault(c => c.ID == topicParagID);
                    if (chkIfAttExist != null)
                    {
                        context.Attach(chkIfAttExist);
                        context.DeleteObject(chkIfAttExist);
                        context.SaveChanges();
                        return 1;
                    }
                    else
                        return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.DeleteTopicParagByTopicParagraphID(" + topicParagID + ")");
                return 0;
            }
        }
      
        public static long AddTopicAttendant(long topicID, long AttID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<TopicAttendant> chkIfAttExist = context.TopicAttendants.Where(c => c.TopicID == topicID && c.AttendantID == AttID).ToList();
                    if (chkIfAttExist.Count == 0)
                    {
                        TopicAttendant topicAtt = new TopicAttendant();
                        topicAtt.TopicID = topicID;
                        topicAtt.AttendantID = AttID;
                        context.TopicAttendants.AddObject(topicAtt);
                        int result = context.SaveChanges();
                        return topicAtt.ID;
                    }
                    else
                        return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.AddTopicAttendant("+ topicID +" , "+ AttID +")");
                return 0;
            }
        }

        public static long DeleteTopicAttendant(long topicID, long AttID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    TopicAttendant chkIfAttExist = context.TopicAttendants.FirstOrDefault(c => c.TopicID == topicID && c.AttendantID == AttID);
                    if (chkIfAttExist != null)
                    {
                        context.Attach(chkIfAttExist);
                        context.DeleteObject(chkIfAttExist);
                        context.SaveChanges();
                        return 1;
                    }
                    else
                        return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.TopicHelper.DeleteTopicAttendantStatus(" + topicID + " , " + AttID + ")");
                return 0;
            }
        }
    }
}