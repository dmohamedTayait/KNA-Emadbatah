<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageSessionCommitteeAttendance.aspx.cs"
    Title="المضبطة الإلكترونية - إدارة لجان الجلسات" Inherits="TayaIT.Enterprise.EMadbatah.Web.ManageSessionCommitteeAttendance"
    MasterPageFile="~/Site.master" %>

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
    <script type="text/javascript" src="scripts/Admin.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //  AjaxEndMethod();
            if (!Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(AjaxEndMethod);
            }
        });

    </script>
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
        .commraw
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
        .sessioncomm
        {
            padding-right: 20px;
            border: 1px solid #dedede;
            margin: 10px;
            background-color: #ececec;
        }
        .sessioncomms
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
            top:25%;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            mp3player($(".MP3FilePath").val());
        });

        function mp3player(mp3path) {     //alert($(".MP3FilePath").val());
            var AudioPlayer = $("#jquery_jplayer_1");
            var playertime;
            AudioPlayer.jPlayer({
                swfPath: "/scripts/jPlayer/",
                wmode: "window",
                solution: 'html, flash',
                supplied: "mp3",
                preload: 'metadata',
                volume: 1,
                cssSelectorAncestor: '#jp_container_1',
                errorAlerts: false,
                warningAlerts: false,
                ready: function () {
                    // play the jplayer
                    $(this).jPlayer("setMedia", {
                        mp3: mp3path// $(".MP3FilePath").val() // mp3 file path//'http://localhost:12000/SessionFiles/1345/session_10-05-2016_1.mp3'//$(".MP3FilePath").val() // mp3 file path
                    }).jPlayer("play", 0);
                    $('.jp-audio .next-jp-xseconds').click(function (e) {
                        AudioPlayer.jPlayer("play", playertime + 5)
                    });
                    $('.jp-audio .prev-jp-xseconds').click(function (e) {
                        AudioPlayer.jPlayer("play", playertime - 5)
                    });
                },
                timeupdate: function (event) {
                    if (!$(this).data("jPlayer").status.paused) {
                        playertime = event.jPlayer.status.currentTime;
                    }
                }
            });
        }
        $(document).ready(function () {
            if (!Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(AjaxEndMethod);
            }
        });
    </script>
    <form id="form1" runat="server">
    <div class="grid_22">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <input id="MP3FolderPath" class="MP3FolderPath" type="hidden" runat="server" value="" />
                <input id="MP3FilePath" class="MP3FilePath" type="hidden" runat="server" value="http://localhost:12000/SessionFiles/1345/session_10-05-2016_1.mp3"
                    name="MP3FilePath" />
                <div>
                    <asp:Label runat="server" ID="lblInfo1" Visible="false" CssClass="lInfo"></asp:Label>
                </div>
                <div class="grid_24 xxlargerow">
                    <div class="Ntitle">
                        إدارة لجان الجلسات:</div>
                </div>
                <div class="clear">
                </div>
                <div class="largerow">
                    <div class="grid_2 h2">
                        <asp:Label ID="Label1" runat="server" Text="رقم الجلسة:"></asp:Label>
                    </div>
                    <div class="grid_8">
                        <asp:DropDownList ID="ddlSessions" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSessions_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                    <div class="clear">
                    </div>
                </div>
                <div id="jquery_jplayer_1" class="jp-jplayer">
                </div>
                <div id="jp_container_1" class="jp-audio">
                    <div class="jp-type-single">
                        <div id="jp_interface_1" class="jp-interface">
                            <ul class="jp-controls">
                                <li><a href="#" class="jp-play" tabindex="1" title="play"></a></li>
                                <li><a href="#" class="jp-pause" tabindex="1" title="pause"></a></li>
                            </ul>
                            <div class="jp-progress">
                                <div class="jp-seek-bar">
                                    <div class="jp-play-bar">
                                    </div>
                                </div>
                            </div>
                            <div class="jp-current-time">
                            </div>
                            <div class="jp-duration">
                            </div>
                            <div class="next-jp-xseconds" title="تقديم 5 ثوانى">
                            </div>
                            <div class="prev-jp-xseconds" title="تاخير 5 ثوانى">
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="grid_22">
                    <% List<Committee> Committees = CommitteeHelper.GetAllCommittee(); %>
                    <section class="" border="0" cellspacing="0" cellpadding="0">
                        <%foreach (Committee comm in Committees)
                          { %>
                        <div id="divComm<%=comm.ID%>">
                            <div class="row commraw">
                             <div class="hoverArrow down" commid=" <%=comm.ID%>"></div>
                                <div class="grid_12 h2"><%=comm.CommitteeName%></div>
                                   <% if (ddlSessions.SelectedValue != "0")
                                      {%>
                                <div class="grid_6 h2">
                                    <a href="javascript:void(0)" class="aPopupAddSessionComm" commid="<%=comm.ID%>" commname="<%=comm.CommitteeName%>" sid="<%=ddlSessions.SelectedValue%>">أضف جلسة جديدة</a>
                                </div>
                                <%} %>
                                <div class="clear"></div>
                            </div>                         
                            <div id="divContent<%=comm.ID%>" class="sessioncomms">
                            <% if (ddlSessions.SelectedValue != "0")
                               { %>

                                <% List<SessionCommittee> sessionComms = SessionCommitteeHelper.GetSessionCommitteeBySessionIDAndCommitteeID(long.Parse(ddlSessions.SelectedValue), comm.ID); %>
                                <% if (sessionComms.Count > 0)
                                   {
                                       foreach (SessionCommittee sessionComm in sessionComms)
                                       { %>
                                   <div id="div_<%=sessionComm.ID%>" class="grid_20 sessioncomm">
                                       <div style="margin-top:10px"></div>
                                       <div class="row">
                                            <div class="grid_3 h2" style="color:#0134cb"><span>اسم الجلسة :</span></div>
                                            <div class="grid_6 h2"><%=sessionComm.CommitteeName%></div>
                                             <div class="grid_4 h2"><a href="javascript:void(0)" class="aPopupDelSessionComm" scommid="<%=sessionComm.ID%>">حذف</a></div>
                                             <div class="grid_4 h2"><a href="javascript:void(0)" class="aPopupTakeSessionCommAtt" scommid="<%=sessionComm.ID%>" commid="<%=comm.ID%>" sid="<%=ddlSessions.SelectedValue%>">أخذ الغياب</a></div>
                                             <div class="clear"></div>
                                        </div>
                                        <div class="row">
                                            <div class="grid_3 h2" style="color:#0134cb"><span>تاريخ الانشاء :</span></div>
                                            <div class="grid_6 h2"> <%=sessionComm.CreatedAt%></div>
                                             <div class="clear"></div>
                                        </div>
                                        <div class="row">
                                            <div class="grid_3 h2" style="color:#0134cb"><span>التفاصيل :</span></div>
                                            <div class="grid_6 h2"><%=sessionComm.AddedDetails%></div>
                                             <div class="clear"></div>
                                        </div>
                                     </div>
                                     <div class="clear"></div>
                                    <% }
                                   }
                           } %>  </div>
                         </div>
                        <% } %>
                    </section>
                </div>
                <div class="clear">
                </div>
                <br />
                <div>
                    <asp:Label runat="server" ID="lblInfo2" Visible="false" CssClass="lInfo"></asp:Label>
                </div>
                <br />
                <br />
                <div class="popupoverlay">
                </div>
                <div class="reviewpopup_cont popupAddSessionComm graybg">
                    <div class="close_btn">
                    </div>
                    <div class="clear">
                    </div>
                    <div class="borderBD">
                        <h2>
                            <span class="red">*</span> أضف جلسة جديدة : <span class="red">(</span> <span class="red lblCommName">
                            </span><span class="red">)</span>
                        </h2>
                    </div>
                    <div class="datacontainer inputcont datacontainer3">
                        <div class="row">
                            <div class="grid_3 h2">
                                <span class="red">*</span>
                                <label title="اسم اللجنة">
                                    اسم الجلسة
                                </label>
                            </div>
                            <div class="grid_8 ">
                                <input type="text" name="txtCommName" class="textfield inputBlock txtCommName" /></div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="row">
                            <div class="grid_3 h2">
                                <span class="red">*</span>
                                <label title="تاريخ">
                                    تاريخ
                                </label>
                            </div>
                            <div class="grid_8 ">
                                <input type="text" name="txtCreatedAt" class="textfield inputBlock txtCreatedAt Calender" /></div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="row">
                            <div class="grid_3 h2">
                                <span class="red">*</span>
                                <label title="التفاصيل">
                                    التفاصيل
                                </label>
                            </div>
                            <div class="grid_8 ">
                                <textarea name="txtAddDetails" class="textfield inputBlock txtAddDetails" rows="5"
                                    cols="20" style="width: 100%"></textarea></div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="row">
                            <div class="grid_3 h2">
                                <label title="">
                                </label>
                            </div>
                            <div class="grid_8 ">
                                <input type="hidden" name="txtSID" class="txtSID" />
                                <input type="hidden" name="txtCommID" class="txtCommID" /></div>
                            <div class="clear">
                            </div>
                        </div>
                    </div>
                    <div class="poppbtnscont fl">
                        <div class="fl" style="margin: 10px">
                            <input type="button" id="btnAddSessionComm" class="btnAddSessionComm btn" value="أضف" />
                            <div class="clear">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                </div>
                <div class="reviewpopup_cont popupAttendant graybg">
                    <div class="close_btn">
                    </div>
                    <div class="clear">
                    </div>
                    <div class="borderBD">
                        <h2>
                            <span class="red">*</span> الغياب :
                        </h2>
                    </div>
                    <div class="datacontainer inputcont datacontainer3 attendantCont">
                        <table id="tbl_Att_Status" class="table h1">
                            <tr>
                                <th style="padding-right: 10px;">
                                    اسماء الأعضاء
                                </th>
                                <th style="color: black">
                                    حاضر
                                </th>
                                <th style="color: Red">
                                    غائب
                                </th>
                                <th style="color: green">
                                    غائب بعذر
                                </th>
                                <th>
                                    غائب بعذر(مهمة رسمية)
                                </th>
                            </tr>
                        </table>
                    </div>
                    <div class="poppbtnscont fl">
                        <div class="fl" style="margin: 10px">
                            <input type="hidden" name="txtSCommID" class="txtSCommID" />
                            <input type="button" id="Button1" class="btnAddSessionCommAtt btn" value="حفظ" />
                            <div class="clear">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</asp:Content>
