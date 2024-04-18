<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApp.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    远大协同管理系统-登录
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Styles/login.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery.cookie.js"></script>
    <script>
        $(function () {
            //修改 easyui-checkbox 中 svg 绝对位置属性
            $('#chkIsMultLogin').next().find('svg').css({ position: 'absolute' });
            $('#psd').textbox('textbox').prop('type', 'password').prop('autocomplete', 'off');
            //$('#psd').textbox('textbox').focus(function () {
            //    $(this).attr('type', "password");
            //}).blur(function () {
            //    $(this).attr('type', 'password');
            //});
        });
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphForm" runat="server">
    <%-- <iframe src="Seeks.aspx" style="width: 0px; height: 0px;"></iframe>--%>
    <div class="login">
        <div class="login-content">
            <h1>用户登录<i></i></h1>
            <div id="login-form" class="easyui-form" method="post" data-options="novalidate:true">
                <div style="margin-bottom: 22px">
                    <input class="easyui-textbox" name="UserName"
                        data-options="iconCls:'icon-jl-user',iconAlign:'left',iconWidth:'28',prompt:'用户名/手机号/邮箱',required:true"
                        style="width: 280px; height: 36px;">
                </div>
                <div style="margin-bottom: 22px">
                    <%--  <input class="easyui-passwordbox" id="psd" name="password"--%>
                    <input class="easyui-textbox" id="psd" name="password"
                        <%--  prompt="密码"--%>
                        data-options="iconCls:'icon-jl-psd',iconAlign:'left',iconWidth:'28',required:true,showEye: false"
                        style="width: 280px; height: 36px; padding: 10px">
                </div>
                <div style="margin-bottom: 15px">
                    <span style="font-size: 14.3px; color: black">多用户登录：</span><input id="chkIsMultLogin" class="easyui-checkbox" name="isMultLogin" value="1">
                </div>
                <div class="login-submit">
                    <button onclick="$('#btnSubmit').click();return false;">登录</button>
                    <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" ClientIDMode="Static" Style="display: none;" />
                </div>
            </div>
            <br />
            <ul>
                <li class="checkbox1">
                    <span class="login_weixin">
                        <a href="/Outsets/wxlogon.aspx">&nbsp;&nbsp;&nbsp;&nbsp;微信登录</a></span>
                </li>
            </ul>
        </div>
    </div>
    <input type="hidden" runat="server" id="hRoleError_url" value="/Errors/Roles.aspx" />

</asp:Content>
