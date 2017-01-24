<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageSessionAttributes.aspx.cs"
    Title="المضبطة الإلكترونية - إدارة بيانات بداية المضبطة" Inherits="TayaIT.Enterprise.EMadbatah.Web.ManageSessionAttributes"
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
             <span>  الفصول التشريعية</span>
              <span style="padding-right: 465px;">  دور الانعقاد</span>
                  </div>
                 
                   
            </div>
            <div class="clear">
            </div>
            <div class="grid_12">
                <div>
                    <asp:GridView ID="gvSeasons" runat="server" AutoGenerateColumns="false" DataKeyNames="id"
                        CssClass="Gridview h2" ShowFooter="true" ShowHeader="false" OnRowEditing="gvSeasons_RowEditing"
                        OnRowUpdating="gvSeasons_RowUpdating" OnRowCancelingEdit="gvSeasons_RowCancelingEdit">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="lblSeasonName" runat="server" Text='<%# Eval("SeasonName")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="textSeasonName" runat="server" Text='<%# Eval("SeasonName")%>'  Style="width: 100%;height: 40px;"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="textSeasonName" runat="server"  Style="width: 100%;height: 40px;"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowEditButton="true" CancelText="الغاء" EditText="تعديل" UpdateText="حفظ" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkRemove" runat="server" CommandArgument='<%# Eval("ID")%>'
                                        OnClick="gvSeasons_RowDeleting" OnClientClick="return confirm('هل أنت متأكد أنك تريد الحذف ؟')"
                                        Text="حذف"></asp:LinkButton>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Button ID="btnAdd" runat="server" Text="اضافة" CssClass="btn" OnClick="AddNewSeason" />
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                <div>
                    <asp:Label ID="lblresult" runat="server"></asp:Label>
                </div>
            </div>
         
            <div class="grid_12">
                <div>
                    <asp:GridView ID="gvStages" runat="server" AutoGenerateColumns="false" DataKeyNames="id"
                        CssClass="Gridview h2" ShowFooter="true" ShowHeader="false" OnRowEditing="gvStages_RowEditing"
                        OnRowUpdating="gvStages_RowUpdating" OnRowCancelingEdit="gvStages_RowCancelingEdit">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="lblStageName" runat="server" Text='<%# Eval("StageName")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="textStageName" runat="server" Text='<%# Eval("StageName")%>'  Style="width: 100%;height: 40px;"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="textStageName" runat="server" Style="width: 100%;height: 40px;" ></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowEditButton="true" CancelText="الغاء" EditText="تعديل" UpdateText="حفظ" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkRemove" runat="server" CommandArgument='<%# Eval("ID")%>'
                                        OnClick="gvStages_RowDeleting" OnClientClick="return confirm('هل أنت متأكد أنك تريد الحذف ؟')"
                                        Text="حذف"></asp:LinkButton>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Button ID="btnAdd" runat="server" Text="اضافة" CssClass="btn" OnClick="AddNewStage" />
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                <div>
                    <asp:Label ID="Label1" runat="server"></asp:Label>
                </div>
            </div>
            <div class="clear">
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</asp:Content>
