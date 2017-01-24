<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageSessionCommitteeAttendance.aspx.cs"
    Title="المضبطة الإلكترونية - إدارة لجان الجلسات" Inherits="TayaIT.Enterprise.EMadbatah.Web.ManageSessionCommitteeAttendance"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script src="scripts/jquery-3.0.0.min.js" type="text/javascript"></script>

    <script src="scripts/jquery.datetimepicker.full.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="scripts/jPlayer/jquery.jplayer.min.js"></script>
    <link href="styles/jquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="styles/jplayer.blue.monday.css" rel="stylesheet" type="text/css" />


    <script type="text/javascript">
        $(document).ready(function () {
            AjaxEndMethod();
            if (!Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(AjaxEndMethod);
            }
        });
        function AjaxEndMethod() {
            // change the date language
            $.datetimepicker.setLocale('ar');
            // date picker
            $(".Calender").datetimepicker({
                format: 'm/d/Y',
                defaultDate: new Date()
            });
        }
        $(document).ready(function () {
     // mp3player($(".MP3FilePath").val());
            // on change
            var MP3FilePath = $(".MP3FilePath");
            var MP3FilePathValue = MP3FilePath.val();
           // var AudioPlayer = $("#jquery_jplayer_1");
            // timer
            setInterval(function () {
                // vars
                MP3FilePath = $(".MP3FilePath");
                // cehck if the value changed
                if (MP3FilePath.val() != MP3FilePathValue) {
                    // save the last value
                    MP3FilePathValue = MP3FilePath.val();
                    // change the player file
                  /*  AudioPlayer.jPlayer("setMedia", {
                        mp3: MP3FilePathValue
                    }).jPlayer("play", 0);
                    $('.jp-audio .next-jp-xseconds').click(function (e) {
                        AudioPlayer.jPlayer("play", playertime + 5)
                    });
                    // prev x seconds button
                    $('.jp-audio .prev-jp-xseconds').click(function (e) {
                        AudioPlayer.jPlayer("play", playertime - 5)
                    });*/
                    mp3player(MP3FilePathValue);
                }
            }, 500);
        });

        function mp3player(mp3path) {     // alert($(".MP3FilePath").val());
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
                       mp3:mp3path// $(".MP3FilePath").val() // mp3 file path//'http://localhost:12000/SessionFiles/1345/session_10-05-2016_1.mp3'//$(".MP3FilePath").val() // mp3 file path
                   }).jPlayer("play",0);
                   // next x seconds button
                   $('.jp-audio .next-jp-xseconds').click(function (e) {
                       AudioPlayer.jPlayer("play", playertime + 5)
                   });
                   // prev x seconds button
                   $('.jp-audio .prev-jp-xseconds').click(function (e) {
                       AudioPlayer.jPlayer("play", playertime - 5)
                   });
               },
               timeupdate: function (event) {
                   if (!$(this).data("jPlayer").status.paused) {
                       // highlight the word by time
                       playertime = event.jPlayer.status.currentTime;
                   }
               }
           });
       }

        })
    </script>
    <style>
        select
        {
            width: 100%;
            font-size:16px;
        }
        table.radio_list td label
        {
            display: none;
            width: 50%;
        }
        .table th
        {
            text-align: right;
            height:35px;
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
            width: 250px;
        }
    </style>
    <form id="form1" runat="server">
    <div class="grid_22">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <input id="MP3FolderPath" class="MP3FolderPath" type="hidden" runat="server" value="" />
               <input id="MP3FilePath" class="MP3FilePath" type="hidden" runat="server" value="http://localhost:12000/SessionFiles/1345/session_10-05-2016_1.mp3" name="MP3FilePath"/>
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
                <div class="largerow">
                    <div class="grid_2 h2">
                        <asp:Label ID="Label2" runat="server" Text="اختر اللجان:"></asp:Label>
                    </div>
                    <div class="grid_8">
                        <asp:DropDownList ID="ddlCommittee" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCommittee_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                    <div class="clear">
                    </div>
                </div>

                  <div class="largerow">
                    <asp:Button ID="btnShow" runat="server" Text="عرض" OnClick="btnShow_Click" CssClass="btn" style="margin-right: 445px;"
                        ValidationGroup="VGSession" />
                    <div class="clear">
                    </div>
                </div>

                <div class="largerow">
                    <div class="grid_2 h2">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                            ControlToValidate="txtDate" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                        <asp:Label ID="lblDate" runat="server" Text="التاريخ"></asp:Label>
                    </div>
                    <div class="grid_8">
                        <asp:TextBox ID="txtDate" runat="server" class="textfield inputBlock Calender" onfocus="jsFunction();" />
                    </div>
                    <div class="clear">
                    </div>
                </div>
                <div class="grid_22">
                    <div class="largerow">
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
                    </div>
                    <div class="largerow">
                        <asp:GridView ID="GVDefaultAttendants" runat="server" CssClass="table h1" orderWidth="0"
                            CellPadding="0" AutoGenerateColumns="false" OnRowDataBound="GVDefaultAttendants_RowDataBound">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:HiddenField runat="server" ID="HFID" Value='<%# Bind("ID") %>'></asp:HiddenField>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="اسم العضو">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblFName" Text='<%# string.Format("{0} {1}", Eval("AttendantTitle ") ,Eval("LongName"))%>'></asp:Label></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<span class='space-st1' style='color:red'>غائب</span><span class='space-st1' style='color:green'>غائب بعذر</span><span class='space-st1' style='width:200px !important'>غائب بعذر (مهمة رسمية )</span>">
                                    <ItemTemplate>
                                        <asp:RadioButtonList ID="RBLAttendantStates" runat="server" RepeatDirection="Horizontal"
                                            CssClass="radio_list">
                                            <asp:ListItem Text="غائب بدون عذر" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="غائب  بعذر" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="غائب  بعذر ( مهمة رسمية)" Value="4"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div class="clear">
                </div>
                <div class="largerow">
                    <asp:Button ID="btnSave" runat="server" Text="حفــظ" OnClick="btnSave_Click" CssClass="btn"
                        ValidationGroup="VGSession" />
                    <div class="clear">
                    </div>
                </div>
                <br />
                  <div>
                        <asp:Label runat="server" ID="lblInfo2" Visible="false" CssClass="lInfo"></asp:Label>
                    </div>
                        <br />
                <br />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</asp:Content>
