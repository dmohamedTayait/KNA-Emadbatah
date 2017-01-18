<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SpeakersAttendance.aspx.cs" Inherits="TayaIT.Enterprise.EMadbatah.Web.SpeakersAttendance" MasterPageFile="~/Site.master" %>


<asp:Content  ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
<script src="../Scripts/jquery.dynDateTime.min.js" type="text/javascript"></script>
<script src="../Scripts/calendar-en.min.js" type="text/javascript"></script>
<link href="../Styles/calendar-blue.css" rel="stylesheet" type="text/css" />
<link href="../Styles/calendar-blue.css" rel="stylesheet" type="text/css" />

    <style>

        .calendar{
                left: 428px !Important;
        }
    </style>
<script type = "text/javascript">
   
    $(document).ready(function () {
        AjaxEndMethod();
        if (!Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(AjaxEndMethod);
        }
    });

    function AjaxEndMethod()
    {        
        $(".Calender").dynDateTime({
            showsTime: true,
            timeFormat: 12,
            ifFormat: "%m/%d/%Y %H:%M",
            daFormat: "%l;%M %p, %e %m,  %Y",
            align: "BR",
            electric: false,
            singleClick: false,
            displayArea: ".siblings('.dtcDisplayArea')",
            button: ".next()"
        });
    }

</script>

    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                
        <asp:Label ID="Label1" runat="server" Text="رقم الجلسة:"></asp:Label>
        <asp:DropDownList ID="ddlSessions" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSessions_SelectedIndexChanged">
        </asp:DropDownList>

        <br />
        <br />
        <br />
        <asp:DropDownList ID="ddlTimes" runat="server" AutoPostBack="True" style="display:none"
            OnSelectedIndexChanged="ddlTimes_SelectedIndexChanged" ></asp:DropDownList>


        <br />
        <br />
        
         <asp:GridView ID="GVAttendants" runat="server" CssClass="grid" BackColor="LightGoldenrodYellow"
                            BorderColor="Tan" BorderWidth="1px" CellPadding="2" AutoGenerateColumns="false"
                            ForeColor="Black" GridLines="None" OnRowDataBound="GVAttendants_RowDataBound"
             OnRowCreated="testHide">
                            <AlternatingRowStyle BackColor="PaleGoldenrod" />
                            <FooterStyle BackColor="Tan" />
                            <HeaderStyle BackColor="Tan" Font-Bold="True" />
                            <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                            <SortedAscendingCellStyle BackColor="#FAFAE7" />
                            <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                            <SortedDescendingCellStyle BackColor="#E1DB9C" />
                            <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                            <Columns>
                                <asp:TemplateField >
                                    <ItemTemplate>
                                        <asp:HiddenField runat="server" ID="HFID" Value='<%# Bind("ID") %>'></asp:HiddenField></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="FirstName">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblFName" Text='<%# Bind("Name") %>'></asp:Label></ItemTemplate>
                                </asp:TemplateField>                                
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <asp:RadioButtonList ID="RBLAttendantStates" runat="server" RepeatDirection="Horizontal" DataSourceID="AttendantStateDS" DataTextField="Name" DataValueField="ID" > 
                                        </asp:RadioButtonList>                                        
                                        <asp:SqlDataSource ID="AttendantStateDS" runat="server" ConnectionString="<%$ ConnectionStrings:EMadbatahConn %>" SelectCommand="SELECT * FROM [AttendantState]"></asp:SqlDataSource>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                 
        <asp:TemplateField HeaderText="Attendant Date">
            <ItemTemplate>
                
                <asp:TextBox ID="txtDOB" runat="server"  class = "Calender" />                
                
            </ItemTemplate>
        </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

        <br />
        <br />
        <br />

        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" style="display:none" />

        <br />
        <br />
        <br />
        
           </ContentTemplate>
                 </asp:UpdatePanel>
        <asp:Button ID="btnSaveToFile" runat="server" Text="Save in File" OnClick="btnSaveToFile_Click" Visible="false"/>
    </div>
    </form>

</asp:Content>
