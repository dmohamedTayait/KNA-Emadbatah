using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Util;
using System.Data.Objects.DataClasses;
//using TayaIT.Enterprise.EMadbatah.Model;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public static class StageHelper
    {
        #region Stage

        public static List<Stage> GetStages()
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Stage> allStages = new List<Stage>();
                    if (context.Stages.Count<Stage>() > 0)
                    {
                        allStages = context.Stages.Where(a => a.Status == 1).ToList<Stage>();
                    }
                    return allStages;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.GetStages()");
                return null;
            }
        }

        public static int UpdateStage(long StageID, string StageName)
        {
            try
            {
                Stage StageForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    StageForUpdate = context.Stages.FirstOrDefault(a => a.ID == StageID);
                    if (StageForUpdate != null)
                    {
                        StageForUpdate.StageName = StageName;
                    }
                    
                    int res = context.SaveChanges();
                    return res;
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.StageHelper.UpdateStage(" + StageID + "," + StageName + ")");
                return -1;
            }
        }

        public static int DeleteStageById(long StageID)
        {
            try
            {
                Stage StageForDelete = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    StageForDelete = context.Stages.FirstOrDefault(a => a.ID == StageID);
                    if (StageForDelete != null)
                    {
                        StageForDelete.Status = 2;
                    }

                    int res = context.SaveChanges();
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.StageHelper.DeleteStageById(" + StageID + ")");
                return -1;
            }
        }

        public static string GetStageByID(long ID)
        {
            string StageTitle = "";
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Stage> allStages = new List<Stage>();
                    if (context.Stages.Count<Stage>() > 0)
                    {
                        StageTitle = context.Stages.Where(c => c.ID == ID).First().StageName;
                    }
                    return StageTitle;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.GetStageByID(" + ID + ")");
                return null;
            }
        }

        public static long AddNewStage(Stage stageObj)
        {
            try
            {

                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    context.Stages.AddObject(stageObj);
                    int result = context.SaveChanges();
                    return stageObj.ID;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.AddNewStage(stageObj)");
                return -1;
            }
        }

        #endregion
    }
}
