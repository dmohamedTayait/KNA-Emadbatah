using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TayaIT.Enterprise.EMadbatah.DAL;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.Model.VecSys;
using TayaIT.Enterprise.EMadbatah.Vecsys;
using TayaIT.Enterprise.EMadbatah.BLL;
using System.Collections;
using TayaIT.Enterprise.EMadbatah.Util.Web;
using TayaIT.Enterprise.EMadbatah.Config;
using System.Text;
namespace TayaIT.Enterprise.EMadbatah.Web
{
    public partial class ManageSessionAttributes : BasePage
    {
        public string SeasonTitle = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.Role != Model.UserRole.Admin)
                Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized);
            if (!Page.IsPostBack)
            {
                gvSeasonbind();
                gvStagebind();
            }
        }

        protected void gvSeasonbind()
        {
            EMadbatahEntities context = new EMadbatahEntities();
            List<Season> SeasonLst = SeasonHelper.GetSeasons();//context.Seasons.Select(aa => aa).Where(cc => cc.SeasonTypeID == SeasonTypeID).ToList();
            if (SeasonLst.Count == 0)
            {
                SeasonLst = new List<Season>();
                Season procObj = new Season();
                procObj.SeasonName = "";
                SeasonLst.Add(procObj);
                gvSeasons.DataSource = SeasonLst;
                gvSeasons.DataBind();
                int columncount = gvSeasons.Rows[0].Cells.Count;
                gvSeasons.Rows[0].Cells.Clear();
                gvSeasons.Rows[0].Cells.Add(new TableCell());
                gvSeasons.Rows[0].Cells[0].ColumnSpan = columncount;
                gvSeasons.Rows[0].Cells[0].Text = "لا يوجد بنود تحت هذه القائمة";
            }
            else
            {
                gvSeasons.DataSource = SeasonLst;
                gvSeasons.DataBind();
            }
        }

        protected void gvSeasons_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvSeasons.EditIndex = e.NewEditIndex;
            gvSeasonbind();
        }

        protected void gvSeasons_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            long SeasonID = Convert.ToInt64(gvSeasons.DataKeys[e.RowIndex].Value.ToString());
            GridViewRow row = (GridViewRow)gvSeasons.Rows[e.RowIndex];

            string textSeasonName = ((TextBox)gvSeasons.Rows[e.RowIndex].FindControl("textSeasonName")).Text;
   
            gvSeasons.EditIndex = -1;
            SeasonHelper.UpdateSeason(SeasonID, textSeasonName);
            gvSeasonbind();
           
        }

        protected void gvSeasons_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvSeasons.EditIndex = -1;
            gvSeasonbind();
        }

        protected void gvSeasons_RowDeleting(object sender, EventArgs e)
        {
            LinkButton lnkRemove = (LinkButton)sender;
            long SeasonID = long.Parse(lnkRemove.CommandArgument);//Convert.ToInt64(gvSeasons.DataKeys[e.RowIndex].Value.ToString());
            SeasonHelper.DeleteSeasonById(SeasonID);
            gvSeasonbind();

        }

        protected void AddNewSeason(object sender, EventArgs e)
        {
            string textSeasonName = ((TextBox)gvSeasons.FooterRow.FindControl("textSeasonName")).Text;

            //Add new Season
            Season SeasonObj = new Season();
            SeasonObj.SeasonName = textSeasonName;
            SeasonObj.Status = 1;
            SeasonHelper.AddNewSeason(SeasonObj);

            gvSeasonbind();
        }




        protected void gvStagebind()
        {
            EMadbatahEntities context = new EMadbatahEntities();
            List<Stage> StageLst = StageHelper.GetStages();//context.Stages.Select(aa => aa).Where(cc => cc.StageTypeID == StageTypeID).ToList();
            if (StageLst.Count == 0)
            {
                StageLst = new List<Stage>();
                Stage procObj = new Stage();
                procObj.StageName = "";
                StageLst.Add(procObj);
                gvStages.DataSource = StageLst;
                gvStages.DataBind();
                int columncount = gvStages.Rows[0].Cells.Count;
                gvStages.Rows[0].Cells.Clear();
                gvStages.Rows[0].Cells.Add(new TableCell());
                gvStages.Rows[0].Cells[0].ColumnSpan = columncount;
                gvStages.Rows[0].Cells[0].Text = "لا يوجد بنود تحت هذه القائمة";
            }
            else
            {
                gvStages.DataSource = StageLst;
                gvStages.DataBind();
            }
        }

        protected void gvStages_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvStages.EditIndex = e.NewEditIndex;
            gvStagebind();
        }

        protected void gvStages_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            long StageID = Convert.ToInt64(gvStages.DataKeys[e.RowIndex].Value.ToString());
            GridViewRow row = (GridViewRow)gvStages.Rows[e.RowIndex];

            string textStageName = ((TextBox)gvStages.Rows[e.RowIndex].FindControl("textStageName")).Text;

            gvStages.EditIndex = -1;
            StageHelper.UpdateStage(StageID, textStageName);
            gvStagebind();

        }

        protected void gvStages_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvStages.EditIndex = -1;
            gvStagebind();
        }

        protected void gvStages_RowDeleting(object sender, EventArgs e)
        {
            LinkButton lnkRemove = (LinkButton)sender;
            long StageID = long.Parse(lnkRemove.CommandArgument);//Convert.ToInt64(gvStages.DataKeys[e.RowIndex].Value.ToString());
            StageHelper.DeleteStageById(StageID);
            gvStagebind();

        }

        protected void AddNewStage(object sender, EventArgs e)
        {
            string textStageName = ((TextBox)gvStages.FooterRow.FindControl("textStageName")).Text;

            //Add new Stage
            Stage stageObj = new Stage();
            stageObj.StageName = textStageName;
            stageObj.Status = 1;
            StageHelper.AddNewStage(stageObj);

            gvStagebind();
        }
    }
}