<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageCommittees.aspx.cs"
    Title="المضبطة الإلكترونية - إدارة اللجان" Inherits="TayaIT.Enterprise.EMadbatah.Web.ManageCommittees"
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
        .Gridview
        {
            border: 1px solid #DEDEDE;
            color: black;
        }
        .Gridview td
        {
            border: 1px solid #DEDEDE;
            color: black;
            background-color: white;
            padding: 10px;
        }
    </style>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="grid_24 xxlargerow">
                <div class="Ntitle">
             <span>  اللجان</span>
              
                  </div>
                 
                   
            </div>
            <div class="clear">
            </div>
            <div class="grid_12">
                <div>
                    <asp:GridView ID="gvCommittees" runat="server" AutoGenerateColumns="false" DataKeyNames="id"
                        CssClass="Gridview h2" ShowFooter="true" ShowHeader="false" OnRowEditing="gvCommittees_RowEditing"
                        OnRowUpdating="gvCommittees_RowUpdating" OnRowCancelingEdit="gvCommittees_RowCancelingEdit">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                     <asp:LinkButton ID="lblCommitteeName" runat="server" Text='<%# Eval("CommitteeName")%>'
                                        PostBackUrl='<%# String.Format("~/ManageCommitteeAttendance.aspx?commid={0}", Eval("ID"))%>'></asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="textCommitteeName" runat="server" Text='<%# Eval("CommitteeName")%>'></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="textCommitteeName" runat="server" Style="width: 100%;height:40px" ></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowEditButton="true" CancelText="الغاء" EditText="تعديل" UpdateText="حفظ" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkRemove" runat="server" CommandArgument='<%# Eval("ID")%>'
                                        OnClick="gvCommittees_RowDeleting" OnClientClick="return confirm('هل أنت متأكد أنك تريد الحذف ؟')"
                                        Text="حذف"></asp:LinkButton>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Button ID="btnAdd" runat="server" Text="اضافة" CssClass="btn" OnClick="AddNewCommittee" />
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                <div>
                    <asp:Label ID="lblresult" runat="server"></asp:Label>
                </div>
            </div>
             <div class="clear">
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</asp:Content>
