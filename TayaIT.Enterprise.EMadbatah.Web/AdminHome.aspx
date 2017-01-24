<%@ Page Title="المضبطة الإلكترونية - ادارة النظام" Language="C#" AutoEventWireup="true"
    CodeFile="AdminHome.aspx.cs" Inherits="TayaIT.Enterprise.EMadbatah.Web.AdminHome"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="grid_24" style="padding-right: 20px;">
        <div class="xxlargerow Ntitle">
            إختر الصفحة المراد إدارتها:</div>
        <div class="toolsbox">
            <div class="adminhomebutton create_ico">
                <a href="CreateNewSession.aspx">اضافة مضبطة جديدة</a></div>
            <div class="adminhomebutton session_icon">
                <a href="AdminSessions.aspx">إدارة الجلسات</a></div>
            <div class="adminhomebutton user_icon">
                <a href="ManageDefaultAttendance.aspx">إدارة الأعضاء</a></div>
            <div class="adminhomebutton setting_icon">
                <a href="ManageProcedureTypes.aspx">إدارة الاجراءات</a></div>
            <div class="clear">
            </div>
            <div class="adminhomebutton setting_icon">
                <a href="ManageSessionAttributes.aspx">إدارة بيانات المضبطة</a></div>
            <div class="adminhomebutton setting_icon">
                <a href="ManageCommittees.aspx">إدارة اللجان</a></div>
            <div class="adminhomebutton setting_icon">
                <a href="ManageSessionCommitteeAttendance.aspx">إدارة لجان الجلسات</a></div>
            <div class="adminhomebutton user_icon">
                <a href="AdminSecurity.aspx">إدارة المستخدمين</a></div>
            <div class="clear">
            </div>
            <div class="adminhomebutton setting_icon">
                <a href="AdminAPPConfig.aspx">إدارة الإعدادات</a></div>
            <div class="clear">
            </div>
        </div>
    </div>
    <div class="clear">
    </div>
</asp:Content>
