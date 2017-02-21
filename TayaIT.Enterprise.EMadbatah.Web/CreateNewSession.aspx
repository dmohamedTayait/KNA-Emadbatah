<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateNewSession.aspx.cs"
    Title="المضبطة الإلكترونية - إضافة مضبطة جديدة" Inherits="TayaIT.Enterprise.EMadbatah.Web.CreateNewSession"
    MasterPageFile="~/Site.master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script src="scripts/jquery-3.0.0.min.js" type="text/javascript"></script>
    <script src="scripts/jquery.datetimepicker.full.min.js" type="text/javascript"></script>
    <link href="styles/jquery.datetimepicker.css" rel="stylesheet" type="text/css" />
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
                timepicker: false,
                defaultDate: new Date(),
                format:'m/d/Y',
            });
            // time picker
            $(".timePicker").datetimepicker({
                datepicker: false,
                defaultDate: new Date(),
                format: 'H:i'
            });
        }
    </script>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div class="grid_24 xxlargerow">
        <div class="Ntitle" runat="server" id="divPageTitle">
            اضافة مضبطة جديدة:</div>
    </div>
    <div class="clear">
    </div>
    <div class="grid_22">
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtDate"
                    ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                <asp:Label ID="lblDate" runat="server" Text="الميعاد المقرر لبدء الجلسة ( التاريخ -الوقت )"></asp:Label>
            </div>
            <div class="grid_8">
                <asp:TextBox ID="txtDate" runat="server" class="textfield inputBlock Calender" />
            </div>
            <div class="grid_3">
              <asp:TextBox ID="txtTime" runat="server" class="textfield inputBlock timePicker"></asp:TextBox>
            </div>
               <div class="grid_3 h2">
                  <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="txtTime"
                    ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtStartDate"
                    ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                <asp:Label ID="lblStartTime" runat="server" Text="الميعاد الفعلى لبدء الجلسة ( التاريخ -الوقت )"></asp:Label>
            </div>
            <div class="grid_8"> 
                <asp:TextBox ID="txtStartDate" runat="server" class="textfield inputBlock Calender"></asp:TextBox>
            </div>
            <div class="grid_3">
                <asp:TextBox ID="txtStartTime" runat="server" class="textfield inputBlock timePicker" />
                 </div>
               <div class="grid_3 h2">
                  <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="txtStartTime"
                    ErrorMessage="*" ForeColor="Red" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ForeColor="Red" ControlToValidate="txtEParliamentID" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                <asp:Label ID="lblEParliamentID" runat="server" Text="رقم الجلسة منذ بدء الحياة النيابية"></asp:Label>
            </div>
            <div class="grid_8">
                <asp:TextBox ID="txtEParliamentID" runat="server" CssClass="textfield inputBlock"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEParliamentID"
                    ErrorMessage="يمكنك ادخال أرقام فقط" ForeColor="Red" ValidationExpression="^[0-9]*$"
                    ValidationGroup="VGSession"> </asp:RegularExpressionValidator>
            </div>
            <div class="grid_3">
                <asp:DropDownList ID="ddlType" runat="server" CssClass="inputBlock">
                    <asp:ListItem Value="0">-- اختر --</asp:ListItem>
                    <asp:ListItem>أ</asp:ListItem>
                    <asp:ListItem>ب</asp:ListItem>
                    <asp:ListItem>ج</asp:ListItem>
                    <asp:ListItem>د</asp:ListItem>
                    <asp:ListItem>خاصة</asp:ListItem>
                    <asp:ListItem>افتتاحية</asp:ListItem>
                    <asp:ListItem>ختامية</asp:ListItem>
                </asp:DropDownList>
                 </div>
               <div class="grid_3 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="ddlType"
                    InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="rfv1" runat="server" ControlToValidate="ddlSeason"
                    InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
                <asp:Label ID="lblSeason" runat="server" Text="الفصل التشريعى"></asp:Label>
            </div>
            <div class="grid_8">
                <asp:DropDownList ID="ddlSeason" runat="server" CssClass="inputBlock">
                </asp:DropDownList>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddlStage"
                    InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
                <asp:Label ID="lblStage" runat="server" Text="دور الانعقاد"></asp:Label>
            </div>
            <div class="grid_8">
                <asp:DropDownList ID="ddlStage" runat="server" CssClass="inputBlock">
                </asp:DropDownList>
            </div>
            <div class="grid_3">
                <asp:DropDownList ID="ddlStagetype" runat="server" CssClass="inputBlock">
                    <asp:ListItem Value="0">-- اختر --</asp:ListItem>
                    <asp:ListItem>عادي</asp:ListItem>
                    <asp:ListItem>غير عادي</asp:ListItem>
                </asp:DropDownList>
                 </div>
               <div class="grid_3 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlStagetype"
                    InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="ddlPresident"
                    InitialValue="0" ErrorMessage="*" ValidationGroup="VGSession" ForeColor="Red" />
                <asp:Label ID="Label1" runat="server" Text="رئيس الجلسة"></asp:Label>
            </div>
            <div class="grid_8">
                <asp:DropDownList ID="ddlPresident" runat="server" CssClass="inputBlock">
                </asp:DropDownList>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="*"
                    ControlToValidate="txtSubject" ValidationGroup="VGSession" ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:Label ID="lblSubject" runat="server" Text="الجلسة"></asp:Label>
            </div>
            <div class="grid_8">
                <asp:TextBox ID="txtSubject" runat="server" CssClass="textfield inputBlock"></asp:TextBox>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="largerow">
            <div class="grid_6 h2">
                &nbsp;
            </div>
            <div class="grid_8 h2">
                <asp:CheckBox ID="CBSessionStart" runat="server" Text="اكتمال النصاب القانونى فى الموعد الأول"
                    Checked Style="font-size: 15px;" class="chk" />
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="prefix_5 addnewusercont">
            <asp:Button ID="btnCreateNewSession" runat="server" Text="حفظ بيانات المضبطة" OnClick="btnCreateNewSession_Click"
                ValidationGroup="VGSession" CssClass="btn" />
        </div>
    </div>
    <div class="clear">
    </div>
    </form>
</asp:Content>
