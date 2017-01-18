<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditSessionFile.aspx.cs"
   Inherits="TayaIT.Enterprise.EMadbatah.Web.EditSessionFile" MasterPageFile="~/Site.master"
   Title="المضبطة الإلكترونية - تعديل الملف الصوتي" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
   <script type="text/javascript" src="scripts/jquery.hotkeys.js"></script>
   <script type="text/javascript" src="scripts/tiny_mce/jquery.tinymce.js"></script>
   <script type="text/javascript" src="scripts/jPlayer/jquery.jplayer.min.js"></script>
   <script type="text/javascript" src="scripts/EditSessionScript.js"></script>
   <link href="styles/jplayer.blue.monday.css" rel="stylesheet" type="text/css" />
   <script language="javascript" type="text/javascript">
      // <![CDATA[
      function Button1_onclick() {
          $.ajax({url: "FileError.aspx?sid=" + <%=SessionFileID %>});
          if(document.getElementById("Button1").value == "خطأ" )
            document.getElementById("Button1").value = "خطأ هنا";
          else
            document.getElementById("Button1").value = "خطأ";
          // you can add some processing to the AJAX call
          return false;
      }
      // ]]>
                
   </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
   <% 
      if (CurrentUser == null || file == null)
      {
      Context.Response.Redirect("~/Error.aspx");
      }
      //if (CurrentUser.ID == file.UserID || file.UserID == null
      //            || ((CurrentUser.Role != UserRole.DataEntry) && (current_session.ReviewerID == CurrentUser.ID)))
      %>
   <form id="editSessionFileForm" runat="server">
      <input id="MP3FilePath" class="MP3FilePath" type="hidden" runat="server" value="" />
      <input id="sessionID" type="hidden" value="" runat="server" class="sessionID" />
      <input id="eparId" type="hidden" value="" runat="server" class="eparId" />
      <input id="XmlFilePath" type="hidden" value="" runat="server" class="hdxmlFilePath" />
      <input type="hidden" id="hdPageMode" runat="server" value="1" class="hdPageMode" />
      <input type="hidden" id="hdSessionContentItemID" runat="server" value="1" class="hdSessionContentItemID" />
      <input type="hidden" id="startTime" runat="server" value="" class="hdstartTime" />
      <input type="hidden" id="endTime" runat="server" value="" class="hdendTime" />
      <input type="hidden" id="currentOrder" runat="server" value="" class="hdcurrentOrder" />
      <input type="hidden" name="agendaItemId" id="agendaItemId" value="" runat="server"
         class="agendaItemId" />
      <input type="hidden" name="attachId" id="attachId" value="0" runat="server" class="attachId" />
      <input type="hidden" name="voteId" id="voteId" value="0" runat="server" class="voteId" />
      <div id="editSessionFile" class="grid_22 prefix_1 suffix_1">
      <div class="borderBD row h2">
         <div class="fr">
            الملف: 
            <strong>
               <asp:Label ID="lblMP3FileName" runat="server"></asp:Label>
            </strong>
         </div>
         <div class="fl spaceR">
            بتاريخ: 
            <strong>
               <asp:Label ID="lblSessionDate" runat="server"></asp:Label>
            </strong>
         </div>
         <div class="fl">
            الجلسة: 
            <strong>
               (
               <asp:Label ID="lblSessionName" runat="server"></asp:Label>
               )
            </strong>
         </div>
         <div class="clear"></div>
      </div>
      <div class="row">
         <div class="row">
            <div class="grid_22">
               <div id="divAgenda" name="divAgenda" runat="server" class="divAgenda">
                  <h2>
                     البند:
                  </h2>
                  <p class="agendaItemTxt">
                     <%= agendaItemTxt%>
                  </p>
                  <br />
               </div>
            </div>
            <div class="clear"></div>
         </div>
         <!--<div class="row">
            <div class="grid_3">
            <span class="red">*</span> البند:
            </div>
            <div class="grid_7">
            <div class="marginBS">
            <asp:DropDownList ID="ddlAgendaItems" AutoPostBack="false" runat="server">
            </asp:DropDownList>
            </div>
            <div>
            <input type="checkbox" class="chkGroupSubAgendaItems" id="chkGroupSubAgendaItems"
            runat="server" />
            <label>
            دمج البنود الفرعية</label>
            </div>
            </div>
            <div class="grid_3 prefix_1">
            البند الفرعى:</div>
            <div class="grid_7">
            <div class="marginBS">
            <asp:DropDownList ID="ddlAgendaSubItems" AutoPostBack="false" runat="server">
            </asp:DropDownList>
            </div>
            <div>
            <input name="" type="checkbox" id="specialBranch" value="" />
            <label for="specialBranch">
            بند استثنائى</label>
            <div id="smallwindow" class="smallwindow">
            <div class="smallwindowCont">
            <div class="smallwindowArrow">
            </div>
            <div class="marginBS title">
            ادخل اسم البند الأستثنائى</div>
            <div class="inputs">
            <input name="txtAgendaItem" type="text" id="txtAgendaItem" class="textfield">
            <input name="" type="button" id="btnAddCustomAgendaItem" class="smallbtn" value="إضافة">
            <div class="errorCont">
            </div>
            </div>
            </div>
            </div>
            </div>
            </div>
            <div class="clear">
            </div>
            </div>-->
         <div class="row h2">
            <div class="grid_3">
               <div>
                  <span class="red">*</span> المتحدث:
               </div>
                 <div style=" padding-top: 50px;">
                <span class="red">*</span> لقب أخر:
                 </div>
            </div>
            <div class="grid_10">
               <div class="marginBS">
                  <asp:DropDownList ID="ddlSpeakers" AutoPostBack="false" runat="server"></asp:DropDownList>
               </div>
               <div style="margin-top:10px;margin-bottom:10px">
                  <div id="newjobtitle" class="edit">
                     <div class="editmode">
                        <label name="addnewjobtext" id="txtSpeakerJob" runat="server" class="textfield" style="font-weight: bold;
                           font-size: 16px; border: 0 !important; box-shadow: 0 0 !important" size="26">
                        </label>
                     </div>
                     <div class="donemode">
                        <strong></strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id="editnewjobbutton" href="#">[تعديل]</a>
                     </div>
                  </div>
               </div>
               <div class="marginBS">
                  <asp:DropDownList ID="ddlOtherTitles" AutoPostBack="false" runat="server" CssClass="ddlOtherTitles">
                   <asp:ListItem Text="-- أختر --" Value="0"></asp:ListItem>
                  <asp:ListItem Text="رئيس الجلسة" Value="1"></asp:ListItem>
                  <asp:ListItem Text="المقرر" Value="2"></asp:ListItem>
                  <asp:ListItem Text="عن المقرر" Value="3"></asp:ListItem>
                  <asp:ListItem Text="-------------------" Value="4"></asp:ListItem>
                  <asp:ListItem Text="رئيس لجنة" Value="5"></asp:ListItem>
                  <asp:ListItem Text="عضو لجنة" Value="6"></asp:ListItem>
                  <asp:ListItem Text="-------------------" Value="7"></asp:ListItem>
                  <asp:ListItem Text="أخرى" Value="8"></asp:ListItem>
                  </asp:DropDownList>
               </div>
               <div class="marginBS displaynone divddlCommittee" style="margin-top:20px;margin-bottom:10px" >
                  <asp:DropDownList ID="ddlCommittee" AutoPostBack="false" runat="server" CssClass="ddlCommittee">
                  </asp:DropDownList>
               </div>
                 <div class="marginBS" style="margin-top:20px;margin-bottom:10px" >
                 <input type="text" name="txtSpeakerOtherJob" id="txtSpeakerOtherJob" runat="server" class="textfield txtSpeakerOtherJob" style="font-weight: bold;font-size: 16px; width: 95%;" size="26" />
                
               </div>
                <div class="clear"></div>

               <div style="margin-top:30px;margin-bottom:10px">
                  <input name="" id="sameAsPrevSpeaker" runat="server" class="sameAsPrevSpeaker" type="checkbox"
                     value="" />
                  <label>
                  نفس المتحدث السابق</label>
                  &nbsp;&nbsp;&nbsp;&nbsp;
                  <input name="isSessionPresident" id="isSessionPresident" runat="server" class="isSessionPresident"
                     type="checkbox" value="" />
                  <label>
                  رئيس الجلسة</label>
               </div>
            </div>
            &nbsp;&nbsp;&nbsp;&nbsp;
             <div class="grid_5 fl">
               <img style="width:200px ;height:150px" src="/images/unknown.jpg" id="imgSpeakerAvatar" name="imgSpeakerAvatar" runat="server" alt=""/>
            </div>
            <div class="clear"></div>
         </div>
      </div>
      <div id="jquery_jplayer_1" class="jp-jplayer"></div>
      <div id="jp_container_1" class="jp-audio">
         <div class="jp-type-single">
            <div id="jp_interface_1" class="jp-interface">
               <ul class="jp-controls">
                  <li><a href="#" class="jp-play" tabindex="1" title="play"></a></li>
                  <li><a href="#" class="jp-pause" tabindex="1" title="pause"></a></li>
               </ul>
               <div class="jp-progress">
                  <div class="jp-seek-bar">
                     <div class="jp-play-bar"></div>
                  </div>
               </div>
               <div class="jp-current-time"></div>
               <div class="jp-duration"></div>
               <div class="next-jp-xseconds" title="تقديم 5 ثوانى"></div>
               <div class="prev-jp-xseconds" title="تاخير 5 ثوانى"></div>
            </div>
         </div>
      </div>
      <div class="graybg">
         <div class="row">
            <textarea id="elm1" runat="server" name="elm1" rows="3" style="width: 100%" class="tinymce"></textarea>
         </div>
         <div class="row divAttach" id="divAttach" runat="server" name="divAttach">
            <span style="color:Red">*</span>
            <span style="color:green">اسم المرفق: </span>
            <span id="spanAttachTitle" runat="server" name="spanAttachTitle" class="spanAttachTitle"></span>&nbsp;&nbsp;&nbsp;&nbsp;
            <a href="javascript:void(0)" class="removeAttach"> حذف المرفق</a>
         </div>
         <div class="row divVote" id="divVote" runat="server" name="divVote">
            <span style="color:Red">*</span>
            <span style="color:green;">اسم التصويت: </span>
            <span id="spanVoteSubject" runat="server" name="spanVoteSubject" class="spanVoteSubject"></span>&nbsp;&nbsp;&nbsp;&nbsp;
            <a href="javascript:void(0)" class="removeVote">حذف التصويت</a>
         </div>
         <br/>
         <div class="row h2">
            <div class="grid_11 alpha">
               <div class="row">
                  تعليق على الفقرة
               </div>
               <div>
                  <textarea rows="3" id="txtComments" runat="server" class="textfield fitAll"></textarea>
               </div>
            </div>
            <div class="grid_10 prefix_1 omega">
               <div class="row">
                  تذيل الصفحة
               </div>
               <div>
                  <textarea rows="3" id="txtFooter" runat="server" class="textfield fitAll"></textarea>
               </div>
            </div>
            <div class="clear"></div>
         </div>
         <br/>
         <div class="nav row">
            <div class="fr">
               <input name="" runat="server" disabled="disabled" id="btnNext" type="button" class="btn next"
                  value="التالى" />
               &nbsp;&nbsp;
               <input name="" runat="server" disabled="disabled" id="btnPrev" type="button" class="btn prev"
                  value="السابق" />
               &nbsp;&nbsp;
               <%--                        <input name="" runat="server" disabled="disabled" id="Button1" type="button" class="btn"
                  value="تجاهل" />--%>
               <input name="chkIgnoredSegment" id="chkIgnoredSegment" runat="server" class="chkIgnoredSegment"
                  type="checkbox" value="" />
               <label for="chkIgnoredSegment">
               تجاهل هذا المقطع</label>  &nbsp;&nbsp;
            </div>
            <div class="fr" style="padding-left: 30px;">
               <input name="" runat="server" disabled="disabled" id="btnFinish" type="button" class="btn finish"
                  value="انهاء" />
            </div>
            <div class="fl">
               <input name="" runat="server" id="btn_addNewAgendaItem" type="button" class="btn addingNewAgendaItem" value="اضافة فهرس" /> 
               &nbsp;&nbsp;
               <input name="" runat="server" id="btnAddProcuder" type="button" class="btn btnAddProcuder" value="اضافة اجراء" />
               &nbsp;&nbsp;
               <input name="" runat="server" id="btnAssignAttachToContentItem" type="button" class="btn btnAssignAttachToContentItem" value="اضافة مرفق" />
               &nbsp;&nbsp;
               <input name="" runat="server" id="btnAddNewVote" type="button" class="btn btnAddNewVote" value="اضافة تصويت" />
               &nbsp;&nbsp;
               <input name="" id="btnSplit" runat="server" type="button" class="btn split" value="اقطع" data-clipboard-action="cut" data-clipboard-target="#MainContent_txtFooter" />
               &nbsp;&nbsp;
               <input name="" type="button" id="btnSaveAndExit" class="btn btnSaveAndExit" value="حفظ و خروج" />
               &nbsp;&nbsp;
               <input name="" type="button" id="various1" data-div="#inline1" class="btn" value="عودة للنص الأصلى" />
               &nbsp;&nbsp;
               <input id="Button1" class="btn " type="button" value="خطأ" onclick="return Button1_onclick()" />
            </div>
            <div class="clear"></div>
         </div>
         <br/>
         <br/>
      </div>
      <!--Begin popup container div-->
      <div class="main_popup_container displaynone">
         <div id="inline1" class="popup_container_content tex_align_center">
            <h5 class="row">
               هل أنت موافق على العودة للنص الأصلي؟
            </h5>
            <div class="row">
               سوف يتم إستبدال كل التعديلات التي قمت بها بالنص الأصلي الموجود، يرجي العلم بأنه
               في حال موافقتك علي إستبدال التعديلات بالنص الأصلي، فإنه لا يمكن العودة لتعديلاتك
               مرة أخري.
            </div>
            <div class="actions_btns">
               <input type="button" class="smallbtn" id="yes" value="نعم">
               <input type="button" class="smallbtn" id="no" value="لا">
            </div>
         </div>
      </div>
      <div class="popupoverlay"></div>
      <div class="reviewpopup_cont-st1 reviewpopup_cont1 graybg">
         <div class="close_btn"></div>
         <div class="clear"></div>
         <div class="datacontainer inputcont">
            <textarea id="Textarea1" runat="server" name="elm1" rows="3" style="width: 100%"
               class="splittinymce"></textarea>
         </div>
         <div class="poppbtnscont fl">
            <div class="fl">
               <input type="button" id="approve" class="approve1" value="موافق" />
               <div class="clear"></div>
            </div>
            <div class="clear"></div>
         </div>
         <div class="clear"></div>
      </div>
      <div class="reviewpopup_cont-st1 reviewpopup_cont2 graybg">
         <div class="close_btn"></div>
         <div class="clear"></div>
         <div class="row">
            <div class="grid_3 h2">
               الاجراءات:
            </div>
            <div class="grid_7">
               <asp:DropDownList ID="DropDownList1" AutoPostBack="false" runat="server">
                  <asp:ListItem Text="-------- اختر الاجراء --------" Value="0"></asp:ListItem>
               </asp:DropDownList>
            </div>
            <div class="clear"></div>
         </div>
         <div class="row">
            <div class="grid_6">
                <ul id="listData" class="listData-st1"></ul>
            </div>
            <div class="grid_14">
                <div class="datacontainer inputcont">
                    <textarea id="Textarea2" runat="server" name="elm1" rows="3" style="width: 100%" class="splittinymce"></textarea>
                </div>
             </div>
         </div>
         <div class="poppbtnscont fl">
            <div class="fl">
               <input type="button" id="Button2" class="approve2" value="موافق" />
               <div class="clear"></div>
            </div>
            <div class="clear"></div>
         </div>
         <div class="clear"></div>
      </div>
      <div class="reviewpopup_cont reviewpopup_cont3 graybg">
         <div class="close_btn"></div>
         <div class="clear"></div>
         <div>
            <h2 class="borderBD">
               <span class="red">*</span> البند :
            </h2>
         </div>
         <div class="datacontainer inputcont datacontainer1">
            <textarea id="Textarea3" runat="server" name="elm1" rows="3" style="width: 100%"
               class="splittinymce"></textarea>
         </div>
         <div>
            <input type="checkbox" class="isAgendaItemIndexed" id="isAgendaItemIndexed" runat="server"
               value="1" />
            <label for="MainContent_isAgendaItemIndexed">
            مسلسل</label>
         </div>
         <div class="poppbtnscont fl">
            <div class="fl">
               <input type="button" id="Button3" class="approve3 btn " value="أضف" />
               <div class="clear"></div>
            </div>
            <div class="clear"></div>
         </div>
         <div class="clear"></div>
      </div>
      <div class="reviewpopup_cont reviewpopup_cont4 graybg">
         <div class="close_btn"></div>
         <div class="clear"></div>
         <div class="borderBD">
            <h2>
               <span class="red">*</span> المرفقات :
            </h2>
         </div>
         <div class="datacontainer inputcont datacontainer2">
            <asp:RadioButtonList runat="server" ID="rdlattachments" AutoPostBack="false" CssClass="rdlattachments"></asp:RadioButtonList>
         </div>
         <div class="poppbtnscont fl">
            <div class="fl" style="margin: 10px;">
               <input type="button" id="btnAddAttach" class="btnAddAttach btn" value="أضف" />
               <div class="clear"></div>
            </div>
            <div class="clear"></div>
         </div>
         <div class="clear"></div>
      </div>
      <div class="reviewpopup_cont reviewpopup_cont5 graybg">
         <div class="close_btn"></div>
         <div class="clear"></div>
         <div class="borderBD">
            <h2>
               <span class="red">*</span> التصويتات :
            </h2>
         </div>
         <div class="datacontainer inputcont datacontainer3">
            <div class="rdlvotes"></div>
         </div>
         <div class="poppbtnscont fl">
            <div class="fl" style="margin: 10px">
               <input type="button" id="btnAddVote" class="btnAddVote btn" value="أضف" />
               <div class="clear"></div>
            </div>
            <div class="clear"></div>
         </div>
         <div class="clear"></div>
      </div>
      <div class="clear"></div>
   </form>
</asp:Content>