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
    public static class SeasonHelper
    {
        #region Season

        public static List<Season> GetSeasons()
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Season> allSeasons = new List<Season>();
                    if (context.Seasons.Count<Season>() > 0)
                    {
                        allSeasons = context.Seasons.Where(a => a.Status == 1).ToList<Season>();
                    }
                    return allSeasons;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.GetSeasons()");
                return null;
            }
        }

        public static int UpdateSeason(long SeasonID, string SeasonName)
        {
            try
            {
                Season SeasonForUpdate = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    SeasonForUpdate = context.Seasons.FirstOrDefault(a => a.ID == SeasonID);
                    if (SeasonForUpdate != null)
                    {
                        SeasonForUpdate.SeasonName = SeasonName;
                    }
                    
                    int res = context.SaveChanges();
                    return res;
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SeasonHelper.UpdateSeason(" + SeasonID + "," + SeasonName + ")");
                return -1;
            }
        }

        public static int DeleteSeasonById(long SeasonID)
        {
            try
            {
                Season SeasonForDelete = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    SeasonForDelete = context.Seasons.FirstOrDefault(a => a.ID == SeasonID);
                    if (SeasonForDelete != null)
                    {
                        SeasonForDelete.Status = 2;
                    }

                    int res = context.SaveChanges();
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SeasonHelper.DeleteSeasonById(" + SeasonID + ")");
                return -1;
            }
        }

        public static string GetSeasonByID(long ID)
        {
            string SeasonTitle = "";
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Season> allSeasons = new List<Season>();
                    if (context.Seasons.Count<Season>() > 0)
                    {
                        SeasonTitle = context.Seasons.Where(c => c.ID == ID).First().SeasonName;
                    }
                    return SeasonTitle;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.GetSeasonByID(" + ID + ")");
                return null;
            }
        }

        public static long AddNewSeason(Season SeasonObj)
        {
            try
            {

                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    context.Seasons.AddObject(SeasonObj);
                    int result = context.SaveChanges();
                    return SeasonObj.ID;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.AddNewSeason(SeasonObj)");
                return -1;
            }
        }

        #endregion
    }
}
