<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageTopics.aspx.cs" Title="المضبطة الإلكترونية - إدارة لجان الجلسات"
    Inherits="TayaIT.Enterprise.EMadbatah.Web.ManageTopics" MasterPageFile="~/Site.master" %>

<%@ Import Namespace="TayaIT.Enterprise.EMadbatah.DAL" %>
<%@ Import Namespace="TayaIT.Enterprise.EMadbatah.Model" %>
<%@ Import Namespace="TayaIT.Enterprise.EMadbatah.BLL" %>
<%@ Import Namespace="TayaIT.Enterprise.EMadbatah.Util" %>
<%@ Import Namespace="TayaIT.Enterprise.EMadbatah.Web" %>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script src="scripts/jquery-3.0.0.min.js" type="text/javascript"></script>
    <script src="scripts/jquery.datetimepicker.full.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="scripts/jPlayer/jquery.jplayer.min.js"></script>
    <link href="styles/jquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="scripts/Topics.js"></script>
    <link href="styles/jplayer.blue.monday.css" rel="stylesheet" type="text/css" />
    <style>
        select
        {
            width: 100%;
            font-size: 16px;
        }
        table.radio_list td label
        {
            display: none;
            width: 50%;
        }
        .table th
        {
            text-align: right;
            height: 35px;
        }
        .table, .table tr
        {
            border: 1px solid #dedede;
        }
        .radio_list, .radio_list tr
        {
            border: none;
        }
        .space-st1
        {
            margin: 0 2px;
            display: inline-block;
            width: 70px;
        }
        .radio_list
        {
        }
        .tpcraw
        {
            border: 1px solid #dedede;
            background-color: White;
            font-size: 10pt;
            font-weight: bold;
            padding: 7px;
        }
        .hoverArrow
        {
            background-repeat: no-repeat;
            width: 22px;
            height: 22px;
            margin: 0px auto;
            cursor: pointer;
            background-image: url(../images/arrow-all.png);
            float: right;
        }
        .hoverArrow.up
        {
            background-position: 5px 8px;
        }
        .hoverArrow.down
        {
            background-position: 5px -10px;
        }
        .TpcParag
        {
            padding-right: 20px;
            border: 1px solid #dedede;
            margin: 10px;
            background-color: #ececec;
        }
        .TpcParags
        {
            padding-right: 20px;
            border: 1px solid #dedede;
            margin-bottom: 10px;
            display: none;
        }
        .popupoverlay
        {
            z-index: 600;
        }
        .reviewpopup_cont
        {
            z-index: 1000;
            height: auto;
            top: 450px;
        }
        .txtTpcParag,.txtTpcTitle
        {
            font-size: 14pt ! important;
            font-weight: bold;
        }
    </style>
    <form id="form1" runat="server">
    <div class="grid_22">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div>
                    <asp:Label runat="server" ID="lblInfo1" Visible="false" CssClass="lInfo"></asp:Label>
                </div>
                <div class="grid_24 xlargerow">
                    <div class="Ntitle">
                        إدارة المقترحات و التوصيات:</div>
                </div>
                <% if (SessionID != "0")
                   {%>
                <div class="grid_6 h2">
                    <a href="javascript:void(0)" class="aPopupAddTopic" sid="<%=SessionID%>">أضف مقترح /
                        توصية</a>
                </div>
                <div class="clear">
                </div>
                <br />
                <div class="grid_24 divTpcs" id="divTpcs">
                    <% List<Topic> topics = TopicHelper.GetAllTopicsBySessionID(long.Parse(SessionID)); %>
                    <%foreach (Topic TpcObj in topics)
                      { %>
                    <div id="divTpc<%=TpcObj.ID%>" tpcid="<%=TpcObj.ID%>">
                        <div class="row tpcraw">
                            <div class="hoverArrow down" tpcid="<%=TpcObj.ID%>">
                            </div>
                            <div class="grid_12 h2 dicTpcTitle" id="dicTpcTitle<%=TpcObj.ID%>">
                                <%=TpcObj.Title%></div>
                            <div class="grid_4 h2">
                                <a href="javascript:void(0)" class="aPopupAddParag" sid="<%=SessionID%>" tpcid="<%=TpcObj.ID%>">
                                    اضافة نص الطلب</a></div>
                            <div class="grid_2 h2">
                                <a href="javascript:void(0)" class="aPopupEditTopic" sid="<%=SessionID%>" tpcid="<%=TpcObj.ID%>">
                                    تعديل</a></div>
                            <div class="grid_2 h2">
                                <a href="javascript:void(0)" class="aPopupDeleteTopic" sid="<%=SessionID%>" tpcid="<%=TpcObj.ID%>">
                                    حذف</a></div>
                            <div class="grid_2 h2">
                                <a href="javascript:void(0)" class="aPopupGetAttTopic" sid="<%=SessionID%>" tpcid="<%=TpcObj.ID%>">
                                    مقدموا الطلب</a></div>
                            <div class="clear">
                            </div>
                        </div>
                        <div id="divTpcParags<%=TpcObj.ID%>" class="TpcParags">
                            <% if (SessionID != "0")
                               { %>
                            <% List<TopicParagraph> tpcparags = TopicHelper.GetTopicParagsByTopicID(TpcObj.ID); %>
                            <% if (tpcparags.Count > 0)
                               {
                                   foreach (TopicParagraph tpcparagObj in tpcparags)
                                   { %>
                            <div id="divTpcParag_<%=tpcparagObj.ID%>" class="grid_22 TpcParag">
                                <div style="margin-top: 10px">
                                </div>
                                <div class="row">
                                    <div class="grid_6 h2 tpcparag" id="divTpcParagtxt_<%=tpcparagObj.ID%>">
                                        <%=tpcparagObj.ParagraphText%></div>
                                    <div class="grid_4 h2">
                                        <a href="javascript:void(0)" class="aPopupEditTpcParag" tpcparagid="<%=tpcparagObj.ID%>"
                                            tcpid="<%=TpcObj.ID%>" sid="<%=SessionID%>">تعديل</a></div>
                                    <div class="grid_4 h2">
                                        <a href="javascript:void(0)" class="aPopupDelTpcParag" tpcparagid="<%=tpcparagObj.ID%>">
                                            حذف</a></div>
                                    <div class="clear">
                                    </div>
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                            <% }
                               }
                               } %>
                        </div>
                    </div>
                    <% } %>
                </div>
                <%} %>
                <div class="clear">
                </div>
                <br />
                <div>
                    <asp:Label runat="server" ID="lblInfo2" Visible="false" CssClass="lInfo"></asp:Label>
                      <input type="hidden" name="txtSID" class="txtSID" />
                </div>
                <br />
                <br />
                <div class="popupoverlay">
                </div>
                <div class="reviewpopup_cont popupAddTopic graybg">
                    <div class="close_btn">
                    </div>
                    <div class="clear">
                    </div>
                    <div class="borderBD">
                        <h2>
                            <span class="red">*</span> اضافة / تعديل (مقترح / توصية ) :
                        </h2>
                    </div>
                    <div class="datacontainer inputcont datacontainer3">
                        <div class="row">
                            <div class="grid_5 h2">
                                <span class="red">*</span>
                                <label title="اسم مقترح / توصية">
                                    عنوان المقترح / التوصية
                                </label>
                            </div>
                             </div>
                             <div class="row">
                            <div class="grid_14 ">
                                <input type="text" name="txtTpcTitle" class="textfield inputBlock txtTpcTitle" /></div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="row">
                            <div class="grid_3 h2">
                                <label title="">
                                </label>
                            </div>
                            <div class="grid_8 ">
                                <input type="hidden" name="txtTpcIDPopupEdit" class="txtTpcIDPopupEdit" />
                                 <div class="clear">
                                </div>
                            </div>
                        </div>
                        <div class="poppbtnscont fl">
                            <div class="fl" style="margin: 10px">
                                <input type="button" id="btnAddTopic" class="btnAddTopic btn" value="حفظ" />
                                <div class="clear">
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                </div>
                <div class="reviewpopup_cont popupAttendant graybg" style="width: 1260px !important;
                    left: 31% !important;">
                    <div class="close_btn">
                    </div>
                    <div class="clear">
                    </div>
                    <div class="borderBD">
                        <h2>
                            <span class="red">*</span> مقدموا الطلب :<span> (</span><span class="spnTpcTitlePopup"></span><span>
                                )</span>
                        </h2>
                    </div>
                    <div class="datacontainer inputcont datacontainer3 attendantCont">
                        <div id="AttCont" class="AttCont">
                        </div>
                    </div>
                    <div class="poppbtnscont fl">
                        <div class="fl" style="margin: 10px">
                            <input type="hidden" name="txtTpcID" class="txtTpcID" />
                            <input type="button" id="Button1" class="btnAddAttToTopic btn" value="حفظ" />
                            <div class="clear">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                </div>
                <div class="reviewpopup_cont popupAddTopicParag graybg">
                    <div class="close_btn">
                    </div>
                    <div class="clear">
                    </div>
                    <div class="borderBD">
                        <h2>
                            <span class="red">*</span> اضافة النص :
                        </h2>
                    </div>
                    <div class="datacontainer inputcont datacontainer3">
                        <div class="row">
                            <div class="grid_15">
                                <textarea rows="8" cols="150" name="txtTpcParag" class="textfield inputBlock txtTpcParag"></textarea></div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="row">
                            <div class="grid_3 h2">
                                <label title="">
                                </label>
                            </div>
                            <div class="grid_8 ">

                                <input type="hidden" name="txtTpcIDPopupParag" class="txtTpcIDPopupParag" />
                                 <input type="hidden" name="txtTpcParagIDPopup" class="txtTpcParagIDPopup" />
                             
                                <div class="clear">
                                </div>
                            </div>
                        </div>
                        <div class="poppbtnscont fl">
                            <div class="fl" style="margin: 10px">
                                <input type="button" id="btnAddTopicParag" class="btnAddTopicParag btn" value="حفظ" />
                                <div class="clear">
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</asp:Content>
