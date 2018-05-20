﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TayaIT.Enterprise.EMadbatah.Config;
using System.Xml;
using System.Xml.XPath;
using System.Web.UI;
using System.Security.Principal;
using Microsoft.Practices.EnterpriseLibrary.Common.Properties;
using TayaIT.Enterprise.EMadbatah.Model;
using TayaIT.Enterprise.EMadbatah.BLL;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Localization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for BasePage
/// </summary>
/// 
namespace TayaIT.Enterprise.EMadbatah.Web
{
    public class BasePage : Page
    {
        public Preferences _preferences = null;

        //public AppConfig _appConfig = AppConfig.GetInstance();

        public BasePage()
        {

            //
            // TODO: Add constructor logic here
            //
        }



        //private string[] GetUserRoles(string currentUser)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    string XMLFilePath = Server.MapPath("files/Users.xml"); ;
        //    doc.Load(XMLFilePath);
        //    XPathNavigator nav;
        //    XPathExpression expr;
        //    string[] userRoles = { };
        //    nav = doc.CreateNavigator();
        //    expr = nav.Compile("/Root/Users/User[@id='" + currentUser + "']");
        //    nav = nav.SelectSingleNode(expr);
        //    if (nav != null)
        //    {
        //        string uName = nav.GetAttribute("id", "");
        //        string Permissions = nav.InnerXml;
        //        userRoles = Permissions.Split(',');
        //    }

        //    return userRoles;
        //}

        public static string GetLocalizedString(string locStringKeyName)
        {
            return LocalHelper.GetLocalizedString(locStringKeyName);// return rmEn.GetString(locStringKeyName);
        }

        //public string RegisterStartupScript(string script)
        //{
        //    //Master.FindControl("litStartupScript")
        //    MasterPage master = this.Master;
        //    //master.RemoveHTMLTags(
        //}


        public EMadbatahUser CurrentUser
        {
            get
            {
                return (EMadbatahUser)Session[Constants.SessionObjectsNames.CURRENT_USER];
            }
            set
            {
                Session[Constants.SessionObjectsNames.CURRENT_USER] = value;
            }
        }

        public EMadbatahUser PreviousUser
        {
            get
            {
                return (EMadbatahUser)Session[Constants.SessionObjectsNames.CURRENT_USER];
            }
            set
            {
                Session[Constants.SessionObjectsNames.CURRENT_USER] = value;
            }
        }

        public string CurrentDomain
        {
            get
            {
                return (string)Session[Constants.SessionObjectsNames.CURRENT_DOMAIN];
            }
            set
            {
                Session[Constants.SessionObjectsNames.CURRENT_DOMAIN] = value;
            }
        }

        public string SessionID
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.SESSION_ID];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string SessionIDEParliment
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.SESSION_ID_EParliment];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string AttendantID
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.ATTENDANT_ID];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string CommitteeID
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.COMMITTEE_ID];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string AttendantIDEParliment
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.ATTENDANT_ID_EParliment];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string AgendaItemID
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.AGENDA_ITEM_ID];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string AgendaItemIDEparliment
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.AGENDA_ITEM_ID_EParliment];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        //public string UserID
        //{
        //    get
        //    {
        //        string val = Context.Request.QueryString[Constants.QSKeyNames.USER_ID];
        //        if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
        //        {
        //            return val;
        //        }
        //        else
        //            return null;
        //    }
        //}

        public string SessionFileID
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.SESSION_FILE_ID];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string SessionStartID
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.SESSION_START_ID];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string SessionContentItemID
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.SESSION_CONTENT_ITEM_ID];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string AttachmentID
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.ATTACHMENT_ID];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string AjaxFunctionName
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.AJAX_FUNCTION_NAME];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string ItemsPerPage
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.ITEMS_PER_PAGE];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string CurrentPageNo
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.PAGE_NO];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string ProcedureID
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.SESSION_ID];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public string ProcedureTypeID
        {
            get
            {
                string val = Context.Request.QueryString[Constants.QSKeyNames.PROCEDURE_TYPE_ID];
                if (!string.IsNullOrEmpty(val) && val.Trim() != string.Empty)
                {
                    return val;
                }
                else
                    return null;
            }
        }

        public Label ErrorMessagePlace
        {
            get
            {
                return (Label)this.Master.FindControl("lblErrorMsg");
            }
        }

        public Label WarnMessagePlace
        {
            get
            {
                return (Label)this.Master.FindControl("lblWarnMsg");
            }
        }

        public Label InfoMessagePlace
        {
            get
            {
                return (Label)this.Master.FindControl("lblInfo");
            }
        }

        public void HideMainPageContent()
        {
            ((ContentPlaceHolder)this.Master.FindControl("MainContent")).Visible = false;
        }

        public void ShowMainError(string errorMessage)
        {
            ErrorMessagePlace.Visible = true;
            ErrorMessagePlace.Text = errorMessage;
            HideMainPageContent();
        }

        public void ShowWarn(string warnMessage)
        {
            WarnMessagePlace.Visible = true;
            WarnMessagePlace.Text = warnMessage;
            //HideMainPageContent();
        }

        public void ShowInfo(string infoMsg)
        {
            InfoMessagePlace.Visible = true;
            InfoMessagePlace.Text = infoMsg;
            //HideMainPageContent();
        }

        /// <summary>
        /// Make sure the browser does not cache this page
        /// </summary>
        public void DisablePageCaching()
        {
            Response.Expires = 0;
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
        }

        protected override void InitializeCulture()
        {
            _preferences = new Preferences();

            if (Request.Cookies[AppConfig.GetInstance().PrefCookieName] == null)
            {
                _preferences.SavePreferencesToCookie();
            }

            WindowsIdentity id = (WindowsIdentity)Page.User.Identity;
            //string userDomainName = id.Name.Split('\\')[1];

            WindowsIdentity id2 = (WindowsIdentity)this.Request.LogonUserIdentity;


            string testUser = "Develop\\dmohamed";//id.Name.ToLower();\\"Develop\\noha";
            CurrentUser = EMadbatahFacade.GetUserByUserID(138); //EMadbatahFacade.GetUserByDomainUserName(testUser);138// EMadbatahFacade.GetUserByUserID(92);// 146//id.Name.ToLower());/174
/*
          //  CurrentUser = EMadbatahFacade.GetUserByUserID(17);

            CurrentDomain = id.Name.Split('\\')[0].ToLower();

            if (CurrentUser == null && AppConfig.GetInstance().MainAdmin != null)
            {
                EMadbatahUser mainAdmin = AppConfig.GetInstance().MainAdmin;
                if (!string.IsNullOrEmpty(mainAdmin.DomainUserName)
                    && !string.IsNullOrEmpty(mainAdmin.Email)
                    && !string.IsNullOrEmpty(mainAdmin.Name))
                {
                    long newid = EMadbatahFacade.AddNewUser(new EMadbatahUser(UserRole.Admin, mainAdmin.Name, mainAdmin.DomainUserName, mainAdmin.Email, true));
                    CurrentUser = EMadbatahFacade.GetUserByUserID(newid);

                }
            }

            if (!Request.AppRelativeCurrentExecutionFilePath.EndsWith(Constants.PageNames.ERROR_PAGE)
                && !Request.AppRelativeCurrentExecutionFilePath.EndsWith(Constants.PageNames.SIGN_OUT))
            {
                if (CurrentUser == null || CurrentDomain == null)
                {
                    int nullval = 0;
                    if (CurrentUser == null)
                    {
                        nullval = 1;
                    }
                    else if (CurrentDomain == null)
                    {
                        nullval = 2;
                    }
                    Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.Unauthorized + "&nullis=" + nullval);
                }
                else if (CurrentDomain.ToLower() != AppConfig.GetInstance().AllowedDomainName.ToLower())
                    Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.InvalidDomain);
                else if (!CurrentUser.IsActive)
                    Response.Redirect(Constants.PageNames.ERROR_PAGE + "?" + Constants.QSKeyNames.ERROR_TYPE + "=" + (int)ErrorType.UserinActive);
            }
            */
            DisablePageCaching();



            // string scriptStartup = new StringBuilder("var ").Append(Constants.QSKeyNames.SESSION_ID_EParliment).Append(" = \"").Append(SessionIDEParliment).Append("\";").ToString();
            // this.ClientScript.RegisterStartupScript(typeof(string), "globalvars", scriptStartup, true);

            //switch (_preferences.UiLang)
            //{
            //    case Language.ar:
            //        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ar-eg");
            //        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ar-eg");
            //        break;

            //    case Language.en:
            //        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            //        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");
            //        break;

            //    default:
            //        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ar-eg");
            //        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ar-eg");
            //        break;
            //}

        }

        //protected void Page_PreRender(object sender, EventArgs e)
        //{

        //    string scriptStartup = new StringBuilder("var ").Append(Constants.QSKeyNames.SESSION_ID_EParliment).Append(" = \"").Append(SessionIDEParliment).Append("\";").ToString();
        //    ScriptManager.RegisterStartupScript(this, GetType(), "globalvars", scriptStartup, true);
        //}


        //public static string GetLocalizedString(string locStringKeyName)
        //{
        //    return Resources.CSearch.ResourceManager.GetString(locStringKeyName);// return rmEn.GetString(locStringKeyName);
        //}

        protected override void Render(HtmlTextWriter writer)
        {

            //string styleSheet = "_css/default_rtl.css";
            //if (_preferences.UiLang == Language.en)
            //    styleSheet = "_css/default_ltr.css";

            //Literal linkTag = new Literal();
            //linkTag.Text = string.Format(@"<link href=""{0}"" type=""text/css"" rel=""stylesheet"" />", styleSheet);

            //Page.Header.Controls.Add(linkTag);

            base.Render(writer);
        }


    }
}
