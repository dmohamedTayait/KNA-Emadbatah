﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Model;
namespace TayaIT.Enterprise.EMadbatah.BLL
{
    public class ReviewerFacade
    {
        
        public static List<SessionContentItem> GetSessionContentItems(long sessionID)
        {
            return SessionContentItemHelper.GetSessionContentItemsBySessionID(sessionID);
        }
        public static int ApproveSessionContentItem(long sessionContentItemId, string note, EMadbatahUser user)
        {
            return SessionContentItemHelper.UpdateSessionContentItemStatus(sessionContentItemId, (int)Model.SessionContentItemStatus.Approved, note, user.ID, (user.Role == UserRole.FileReviewer));
        }
        public static int RejectSessionContentItem(long sessionContentItemId, string note, EMadbatahUser user)
        {
            return SessionContentItemHelper.UpdateSessionContentItemStatus(sessionContentItemId, (int)Model.SessionContentItemStatus.Rejected, note, user.ID, (user.Role == UserRole.FileReviewer));
        }
        public static int UpdateSessionContentItemText(long sessionContentItemId, string text, long reviewerUserID, string reviewerNote,string userId)
        {
            if (userId == "")
                return SessionContentItemHelper.UpdateSessionContentItemForReviewer(sessionContentItemId, text, reviewerUserID, reviewerNote);
            else return SessionContentItemHelper.UpdateSessionContentItemText(sessionContentItemId, text);
        }
        public static bool ApproveSession(long sessionId)
        {
            List<SessionContentItem> items = SessionContentItemHelper.GetSessionContentItemsBySessionIDAndNotStatusID(sessionId, (int)Model.SessionContentItemStatus.Approved);
            if (items.Count == 0 && !SessionContentItemHelper.DoesSessionContainsUndefinedAttendants(sessionId))
            {
                SessionHelper.UpdateSessionStatus(sessionId, (int)Model.SessionStatus.Approved);
                return true;
            }
            else
                return false;
        }
        public static bool FinalApproveSession(long sessionId)
        {
            if (SessionHelper.GetSessionByID(sessionId).SessionStatusID != (int)Model.SessionStatus.Approved)
                return false;
            List<SessionContentItem> items = SessionContentItemHelper.GetSessionContentItemsBySessionIDAndNotStatusID(sessionId, (int)Model.SessionContentItemStatus.Approved);
            if (items.Count == 0 && !SessionContentItemHelper.DoesSessionContainsUndefinedAttendants(sessionId))
            {
                SessionHelper.FinalApproveSession(sessionId, (int)Model.SessionStatus.FinalApproved, (int)Model.MadbatahFilesStatus.FinalCreated);
                return true;
            }
            else
            {
                SessionHelper.UpdateSessionStatus(sessionId, (int)Model.SessionStatus.Approved);
                return false;
            }
        }
        public static List<SessionContentItem> GetUserReviewNotes(long sessionId, long userId)
        {
            return SessionContentItemHelper.GetSessionContentItemsBySessionIDAndStatusAndUserID(sessionId, (int)Model.SessionContentItemStatus.Rejected , userId);
        }
       
    }
}
