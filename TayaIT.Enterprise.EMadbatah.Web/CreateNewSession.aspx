<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateNewSession.aspx.cs" Title="المضبطة الإلكترونية - إضافة مضبطة جديدة"
    Inherits="TayaIT.Enterprise.EMadbatah.Web.CreateNewSession" MasterPageFile="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
  <script src="scripts/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="scripts/jquery.dynDateTime.min.js" type="text/javascript"></script>
  <script src="scripts/calendar-en.min.js" type="text/javascript"></script>
  <link href="styles/calendar-blue.css" rel="stylesheet" type="text/css" />
  <script type="text/javascript">
      $(document).ready(function () {
          AjaxEndMethod();
          if (!Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
              Sys.WebForms.PageRequestManager.getInstance().add_endRequest(AjaxEndMethod);
          }
      });
      function AjaxEndMethod() {
          $(".Calender").dynDateTime({
              showsTime: true,
              timeFormat: 12,
              ifFormat: "%m/%d/%Y %H:%M",
              daFormat: "%l;%M %p, %e %m,  %Y",
              align: "BR",
              electric: false,
              singleClick: true,
              button: ".next()"
          });
      }
    </script>
    <style type="text/css">
        .calendar
        {
            direction: rtl;
        }
        select
        {
            font-size:15px;
        }
        .chk label
        {
            padding:12px;
        }
    </style>
  
  <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"> </asp:ToolkitScriptManager>
    <div class="grid_24 xxlargerow">
      <div class="Ntitle">اضافة مضبطة جديدة:</div>
    </div>
    <div class="clear"></div>
    <div class="grid_22">
      <div class="largerow">
        <div class="grid_5 h2">
          <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtDate" ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
          <asp:Label ID="lblDate" runat="server" Text="الميعاد المقرر لبدء الجلسة"></asp:Label>
        </div>
        <div class="grid_8">
          <asp:TextBox ID="txtDate" runat="server" class="textfield inputBlock Calender" onfocus="jsFunction();" />
        </div>
        <div class="clear"> </div>
      </div>
      <div class="largerow">
        <div class="grid_5 h2">
        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtStartTime" ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
          <asp:Label ID="lblStartTime" runat="server" Text="الميعاد الفعلى لبدء الجلسة"></asp:Label>
        </div>
        <div class="grid_8">
          <asp:TextBox ID="txtStartTime" runat="server" class="textfield inputBlock Calender" onfocus="jsFunction();" />
        </div>
        <div class="clear"> </div>
      </div>
      <div class="largerow">
        <div class="grid_5 h2">
          <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" ForeColor="Red" ControlToValidate="txtEParliamentID" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
          <asp:Label ID="lblEParliamentID" runat="server" Text="رقم الجلسة منذ بدء الحياة النيابية"></asp:Label>
        </div>
        <div class="grid_8">
          <asp:TextBox ID="txtEParliamentID" runat="server" CssClass="textfield inputBlock"></asp:TextBox>
          <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEParliamentID"
    ErrorMessage="يمكنك ادخال أرقام فقط" ForeColor="Red" ValidationExpression="^[0-9]*$"
    ValidationGroup="VGSession"> </asp:RegularExpressionValidator>
        </div>
        <div class="grid_3">
          <asp:DropDownList ID="ddlType" runat="server"  CssClass="">
            <asp:ListItem Value="0">-- اختر --</asp:ListItem>
            <asp:ListItem>أ</asp:ListItem>
            <asp:ListItem>ب</asp:ListItem>
            <asp:ListItem>ج</asp:ListItem>
            <asp:ListItem>د</asp:ListItem>
            <asp:ListItem>خاصة</asp:ListItem>
            <asp:ListItem>افتتاحية</asp:ListItem>
            <asp:ListItem>ختامية</asp:ListItem>
          </asp:DropDownList>
          <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="ddlType"
 InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
        </div>
        <div class="clear"></div>
      </div>
      <div class="largerow">
        <div class="grid_5 h2">
          <asp:RequiredFieldValidator ID="rfv1" runat="server" ControlToValidate="ddlSeason" InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
          <asp:Label ID="lblSeason" runat="server" Text="الفصل التشريعى"></asp:Label>
        </div>
        <div class="grid_8">
          <asp:DropDownList ID="ddlSeason" runat="server" CssClass="inputBlock">
            <asp:ListItem Value="0">-- اختر --</asp:ListItem>
            <asp:ListItem Value="14">الرابع عشر</asp:ListItem>
            <asp:ListItem Value="15">الخامس عشر</asp:ListItem>
            <asp:ListItem Value="16">السادس عشر</asp:ListItem>
            <asp:ListItem Value="17">السابع عشر</asp:ListItem>
            <asp:ListItem Value="18">الثامن عشر</asp:ListItem>
            <asp:ListItem Value="19">التاسع عشر</asp:ListItem>
            <asp:ListItem Value="20">العشرون</asp:ListItem>
          </asp:DropDownList>
        </div>
        <div class="clear"> </div>
      </div>
      <div class="largerow">
        <div class="grid_5 h2">
          <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddlStage" InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
          <asp:Label ID="lblStage" runat="server" Text="دور الانعقاد"></asp:Label>
        </div>
        <div class="grid_8">
          <asp:DropDownList ID="ddlStage" runat="server" CssClass="inputBlock">
            <asp:ListItem Value="0">-- اختر --</asp:ListItem>
            <asp:ListItem Value="1">الاول</asp:ListItem>
            <asp:ListItem Value="2">الثاني</asp:ListItem>
            <asp:ListItem Value="3">الثالث</asp:ListItem>
            <asp:ListItem Value="4">الرابع</asp:ListItem>
            <asp:ListItem Value="5">الخامس التكميلي</asp:ListItem>
          </asp:DropDownList>
        </div>
        <div class="grid_3">
          <asp:DropDownList ID="ddlStagetype" runat="server" CssClass="">
            <asp:ListItem Value="0">-- اختر --</asp:ListItem>
            <asp:ListItem>عادي</asp:ListItem>
            <asp:ListItem>غير عادي</asp:ListItem>
          </asp:DropDownList>
          <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlStagetype" InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
        </div>
        <div class="clear"> </div>
      </div>
           <div class="largerow">
        <div class="grid_5 h2">
          <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="ddlPresident" InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
          <asp:Label ID="Label1" runat="server" Text="رئيس الجلسة"></asp:Label>
        </div>
        <div class="grid_8">
          <asp:DropDownList ID="ddlPresident" runat="server" CssClass="inputBlock">
          </asp:DropDownList>
        </div>
        <div class="clear"> </div>
      </div>
      <div class="largerow">
        <div class="grid_5 h2">
        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="*" ControlToValidate="txtSubject" ValidationGroup="VGSession" ForeColor="Red"></asp:RequiredFieldValidator>
          <asp:Label ID="lblSubject" runat="server" Text="الجلسة"></asp:Label>
        </div>
        <div class="grid_8">
          <asp:TextBox ID="txtSubject" runat="server" CssClass="textfield inputBlock"></asp:TextBox>
            
        </div>
       <div class="clear"> </div>
      </div>
       <div class="largerow">
       <div class="grid_5 h2">
       &nbsp;
       </div>
         <div class="grid_8">
          <asp:CheckBox ID="CBSessionStart" runat="server" Text="اكتمال النصاب القانونى فى الموعد الأول" Checked style="font-size:15px;" class="chk"/>
        </div>
        <div class="clear"> </div>
      </div>
      <div class="prefix_5 addnewusercont">
        <asp:Button ID="btnCreateNewSession" runat="server" Text="انشاء مضبطة جديدة" OnClick="btnCreateNewSession_Click" ValidationGroup="VGSession" CssClass="btn" />
      </div>
    </div>
    <div class="clear"></div>
  </form>
</asp:Content>
