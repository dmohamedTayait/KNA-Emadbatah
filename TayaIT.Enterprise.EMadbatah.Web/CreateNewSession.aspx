<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateNewSession.aspx.cs" Inherits="TayaIT.Enterprise.EMadbatah.Web.CreateNewSession" MasterPageFile="~/Site.master"%>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<script src="scripts/jquery-1.4.2.min.js" type="text/javascript"></script>
<script src="scripts/jquery.dynDateTime.min.js" type="text/javascript"></script>
<script src="scripts/calendar-en.min.js" type="text/javascript"></script>
<link href="styles/calendar-blue.css" rel="stylesheet" type="text/css" />

<script type = "text/javascript">

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
            singleClick: false,
            displayArea: ".siblings('.dtcDisplayArea')",
            button: ".next()"
        });

    }

</script>
 <style type="text/css">
        .auto-style1 {
           
        }
    
        .calendar{
                right: 162px !Important;
                direction:rtl;
        }
    </style>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager runat="server"></asp:ToolkitScriptManager>    

   <table style="height: 493px; width: 779px">
            <tr>
                <td>
                    <asp:Label ID="lblDate" runat="server" Text="التاريخ"></asp:Label>
                </td>
                <td class="auto-style1">
                   <asp:TextBox ID="txtDate" runat="server"  class = "Calender" onfocus="jsFunction();" />
                   </td>
                   <td>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                           ControlToValidate="txtDate" ErrorMessage="*" ForeColor="Red" 
                           ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                        </td>
                   <td></td>
                <%--<td>
                    <asp:TextBox ID="txtDate" runat="server" contentEditable="false"  autocomplete="off"></asp:TextBox>
                    <asp:CalendarExtender ID="txtBOD_CalendarExtender" Format="yyyy/MM/dd"
                            runat="server" Enabled="True" CssClass="cal_Theme1" TargetControlID="txtDate">
                        </asp:CalendarExtender>
                </td>--%>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="lblStartTime" runat="server" Text="ميعاد بدء الجلسة"></asp:Label>
                </td>
               <td class="auto-style1">
                    <asp:TextBox ID="txtStartTime" runat="server"  class= "Calender" onfocus="jsFunction();" />
                 </td>
                    <td></td>
                   <td></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblEParliamentID" runat="server" Text="رقم الجلسة منذ بدء الحياة النيابية"></asp:Label>
                </td>
                <td class="auto-style1">
                    <asp:TextBox ID="txtEParliamentID" runat="server"></asp:TextBox>
                    </td>
                    <td class="auto-style1">
                    <asp:DropDownList ID="ddlType" runat="server">
                        <asp:ListItem Value="0">-- اختر --</asp:ListItem>
                        <asp:ListItem>أ</asp:ListItem>
                        <asp:ListItem>ب</asp:ListItem>
                        <asp:ListItem>ج</asp:ListItem>
                        <asp:ListItem>د</asp:ListItem>
                        <asp:ListItem>خاصة</asp:ListItem>
                        <asp:ListItem>افتتاحية</asp:ListItem>
                        <asp:ListItem>ختامية</asp:ListItem>
                    </asp:DropDownList>                    
                </td>
                <td>
                 

                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                     ErrorMessage="*" ForeColor="Red" ControlToValidate="txtEParliamentID" ValidationGroup="VGSession"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                         ControlToValidate="txtEParliamentID" ErrorMessage="يمكنك ادخال أرقام فقط" ForeColor="Red" ValidationExpression="^[0-9]*$" ValidationGroup="VGSession">
                    </asp:RegularExpressionValidator>
                </td>
                
           <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                        ControlToValidate="ddlType" 
                        InitialValue="0" ErrorMessage="*" 
                        ValidationGroup="VGSession" ForeColor="Red"/>
                   </td>              
            </tr>

            <tr>
                <td>
                    <asp:Label ID="lblSeason" runat="server" Text="الفصل التشريعى"></asp:Label>
                </td>

                <td class="auto-style1">
                    <asp:DropDownList ID="ddlSeason" runat="server">
                        <asp:ListItem Value="0">-- اختر --</asp:ListItem>
                        <asp:ListItem>14</asp:ListItem>
                        <asp:ListItem>15</asp:ListItem>
                        <asp:ListItem>16</asp:ListItem>
                        <asp:ListItem>17</asp:ListItem>
                        <asp:ListItem>18</asp:ListItem>
                        <asp:ListItem>19</asp:ListItem>
                        <asp:ListItem>20</asp:ListItem>
                    </asp:DropDownList> 
                   </td>
                    
                    <td>
                    <asp:RequiredFieldValidator ID="rfv1" runat="server" 
                        ControlToValidate="ddlSeason" 
                        InitialValue="0" ErrorMessage="*" 
                        ValidationGroup="VGSession" ForeColor="Red"/>
                   </td>                
            </tr>

            <tr>
                <td>
                    <asp:Label ID="lblStage" runat="server" Text="دور الانعقاد"></asp:Label>
                </td>

                <td class="auto-style1">
                    <asp:DropDownList ID="ddlStage" runat="server">
                        <asp:ListItem Value="0">-- اختر --</asp:ListItem>
                        <asp:ListItem Value="1">الاول</asp:ListItem>
                        <asp:ListItem Value="2">الثاني</asp:ListItem>
                        <asp:ListItem Value="3">الثالث</asp:ListItem>
                        <asp:ListItem Value="4">الرابع</asp:ListItem>
                        <asp:ListItem Value="5">الخامس التكميلي</asp:ListItem>
                    </asp:DropDownList>                    
                </td>
                <td class="auto-style1">
                    <asp:DropDownList ID="ddlStagetype" runat="server">
                        <asp:ListItem Value="0">-- اختر --</asp:ListItem>
                        <asp:ListItem>عادي</asp:ListItem>
                        <asp:ListItem>غير عادي</asp:ListItem>
                       
                    </asp:DropDownList>                    
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ControlToValidate="ddlStage" 
                        InitialValue="0" ErrorMessage="*" 
                        ValidationGroup="VGSession" ForeColor="Red"/>
                   </td>  
                    
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                        ControlToValidate="ddlStagetype" 
                        InitialValue="0" ErrorMessage="*" 
                        ValidationGroup="VGSession" ForeColor="Red"/>
                </td> 
  
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblSubject" runat="server" Text="عنوان الجلسة"></asp:Label>
                </td>

                <td class="auto-style1">
                    <asp:TextBox ID="txtSubject" runat="server"></asp:TextBox>
                </td>
            </tr>

             <tr>
                <td>
                    <asp:CheckBox ID="CBSessionStart" runat="server" Text="افتتحت الجلسسة في وقتها" Checked/>                    
                </td>                
            </tr>  
       <tr>

       <td>
       <asp:Button ID="btnCreateNewSession" runat="server" Text="انشاء مضبطة جديدة" OnClick="btnCreateNewSession_Click" 
           ValidationGroup="VGSession"/>
           </td>
           </tr>
        </table>

    </form>
</asp:Content>
