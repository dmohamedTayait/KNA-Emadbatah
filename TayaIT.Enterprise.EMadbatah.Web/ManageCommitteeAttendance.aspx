<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageCommitteeAttendance.aspx.cs"
    Title="المضبطة الإلكترونية - إدارة اللجان" Inherits="TayaIT.Enterprise.EMadbatah.Web.ManageCommitteeAttendance"
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
    <style type="text/css">
        .chbDefAttlst
        {
            border: 1px solid #DEDEDE;
            color: black;
        }
        .chbDefAttlst td
        {
            border: 1px solid #DEDEDE;
            color: black;
            background-color: white;
            padding: 20px;
        }
        td label
        {
            padding-right: 10px;
            font-size: 140%;
            font-weight: bold;
        }
    </style>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="grid_24 xxlargerow">
                <div>
                    <asp:Label runat="server" ID="lblInfo1" Visible="false" CssClass="lInfo"></asp:Label>
                </div>
                <div class="Ntitle">
                    <span>من فضلك قم باختيار اسماء أعضاء
                        <%= CommitteeName%>:</span><a href="ManageCommittees.aspx" style="float: left">عودة
                            لقائمة اللجان</a></div>
            </div>
            </div>
            <div class="clear">
            </div>
            <div class="grid_22">
                <asp:CheckBoxList ID="chbDefAttlst" runat="server" RepeatColumns="5" CssClass="chbDefAttlst">
                </asp:CheckBoxList>
            </div>
            <div class="clear">
            </div>
            <br />
            <div class="largerow addnewusercont" style="float: right;">
                <asp:Button ID="btnSave" runat="server" Text="حفظ" OnClick="btnSave_Click" CssClass="btn" />
            </div>
            <br />
            <br />
            <div>
                <asp:Label runat="server" ID="lblInfo2" Visible="false" CssClass="lInfo"></asp:Label>
            </div>
            <br />
            <br />
            <div class="clear">
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</asp:Content>
