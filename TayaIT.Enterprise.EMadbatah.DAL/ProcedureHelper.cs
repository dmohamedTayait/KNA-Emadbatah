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
    public static class ProcedureHelper
    {
        #region Procedure

       public static List<ProcedureType> GetProcedureTypes()
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<ProcedureType> allProcedureTypes = new List<ProcedureType>();
                    if (context.ProcedureTypes.Count<ProcedureType>() > 0)
                    {
                        allProcedureTypes = context.ProcedureTypes.Include("Procedures").ToList<ProcedureType>();
                    }
                    return allProcedureTypes;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.GetProcedureTypes()");
                return null;
            }
        }
        
        
        public static List<Procedure> GetProcedures(int procedureTypeID)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<Procedure> allProcedures = new List<Procedure>();
                    if (context.Procedures.Count<Procedure>() > 0)
                    {
                        if (procedureTypeID != 0)
                        {
                            allProcedures = context.Procedures.Include("ProcedureType").Where(c => c.ProcedureTypeID == procedureTypeID).ToList<Procedure>();
                           
                        }
                        else
                        {
                            allProcedures = context.Procedures.ToList<Procedure>();

                        }
                    }
                    return allProcedures;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.SessionHelper.GetProcedures(" + procedureTypeID + ")");
                return null;
            }
        }
        #endregion
    }
}
