using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Util;

namespace TayaIT.Enterprise.EMadbatah.DAL
{
    public class DefaultAttendantHelper
    {
        public static List<DefaultAttendant> GetAllDefaultAttendants(bool unassigned)
        {
            try
            {
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    List<DefaultAttendant> DefaultAttendants = new List<DefaultAttendant>();
                    if (unassigned)
                    {
                        DefaultAttendants = context.DefaultAttendants.Select(c => c).Where(c => c.Type != 9).OrderBy(s => s.LongName).ToList();
                    }
                    else
                    {
                        DefaultAttendants = context.DefaultAttendants.Select(c => c).Where(c => c.Type != 8 && c.Type != 9).OrderBy(s => s.LongName).ToList();
                    }
                   
                    return DefaultAttendants;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.DefaultAttendantHelper.GetAllDefaultAttendants()");
                return null;
            }
        }

        public static DefaultAttendant GetAttendantById(long attendant_id)
        {
            try
            {
                DefaultAttendant attendant = null;
                using (EMadbatahEntities context = new EMadbatahEntities())
                {
                    attendant = context.DefaultAttendants.FirstOrDefault(c => c.ID == attendant_id);
                }
                return attendant;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "TayaIT.Enterprise.EMadbatah.DAL.AttendantHelper.GetAttendantById(" + attendant_id + ")");
                return null;
            }
        }


    }
}
