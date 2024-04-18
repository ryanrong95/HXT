<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Plat.Administrators.Edit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管理员编辑</title>
    <uc:EasyUI runat="server" />
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '';
        gvSettings.menu = '管理员管理';
        gvSettings.summary = '';

    </script>
    <script src="http://fixed2.b1b.com/My/Scripts/jquery.md5.js"></script>
    <script>
        $(function () {



            $('form').submit(function () {


                var password = $.trim($('#txtPassword').val());

                if (password != "") {
                    $('#hPassword').val($.md5(password));
                    $('#txtPassword').val('');
                }
                else {
                    password = $.trim($('#txtNewPassword').val());
                    if (password != "") {
                        $('#hPassword').val($.md5(password));
                    }
                    $('#txtNewPassword').val('');
                }



            });
        });
    </script>
</head>
<body>
    <div id="p" class="easyui-panel" data-options="border:false,fit:true,iconCls:'icon-edit',closable:true,onClose:function(){$.myWindow.close();}" title="管理员编辑页" style="width: 100%;">
        <form id="form" runat="server">
            <%
                var model = this.Model as NtErp.Services.Models.Admin;
            %>
            <table class="liebiao">
                <tr>
                    <td style="width: 100px;">登录名</td>
                    <td>
                        <input id="txtUserName" type="text" class="easyui-validatebox easyui-textbox" name="UserName"
                            data-options="required:true,prompt:'请输入登录名'" value="<%=model?.UserName %>" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px;">真实姓名</td>
                    <td>
                        <input id="txtRealName" type="text" class="easyui-validatebox easyui-textbox" name="RealName"
                            data-options="required:true,prompt:'请输入真实姓名'" value="<%=model?.RealName %>" />
                    </td>
                </tr>
                <tr id="trPassword" runat="server">
                    <td style="width: 100px;">密码</td>
                    <td>
                        <input id="txtPassword" type="text" class="easyui-validatebox easyui-textbox" name="Password"
                            data-options="required:true,prompt:'请输入密码'" value="" />
                    </td>
                </tr>
                <tr id="trNewPassword" runat="server">
                    <td style="width: 100px;">新密码</td>
                    <td>
                        <input id="txtNewPassword" type="text" class="easyui-validatebox easyui-textbox" name="Password"
                            data-options="prompt:'新密码为空时，将保持密码不变'" value="" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px;">摘要</td>
                    <td>
                        <input id="txtSummary" type="text" class="easyui-validatebox easyui-textbox" name="Summary"
                            data-options="required:true,prompt:'请输入摘要',multiline:true" value="<%=model?.Summary %>" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnSubmit" runat="server" class="easyui-linkbutton" OnClick="btnSubmit_Click" Text="提交" />
                        <%--<a href="javascript:void(0)" style="width: 100px;"  id="btnsubmit">提交</a>--%>
                    </td>
                </tr>
            </table>
            <input id="hPassword" name="hPassword" type="hidden" value="" runat="server" />
        </form>
    </div>
    <input type="hidden" id="EnterSuccess" value="提交成功！" runat="server" />
    <input type="hidden" id="EnterError" value="提交失败！" runat="server" />
    <input type="hidden" id="AccountRepeat" value="账户重复！" runat="server" />
</body>
</html>
