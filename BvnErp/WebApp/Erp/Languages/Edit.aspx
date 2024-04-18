<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Erp.Languages.Edit" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>语言编辑页</title>
    <uc:EasyUI runat="server"></uc:EasyUI>

    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '';
        gvSettings.menu = '语言管理';
        gvSettings.summary = '';

    </script>
</head>
<body>
    <div id="p" class="easyui-panel" data-options="border:false" title="语言编辑页" style="width: 100%;">
        <form runat="server">
            <%
                var model = this.Model as NtErp.Services.Models.Language;
            %>
            <div>
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px;">简称</td>
                        <td>
                            <input id="shortName" type="text" class="easyui-validatebox easyui-textbox"
                                name="ShortName"
                                data-options="required:true,prompt:'请输入简称'" maxlength="50" value="<%=model?.ShortName %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>显示名称</td>
                        <td>
                            <input id="displayName" type="text" class="easyui-validatebox easyui-textbox"
                                name="DisplayName"
                                data-options="required:true,prompt:'请输入显示名称'" maxlength="50" value="<%=model?.DisplayName %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>英文名称</td>
                        <td>
                            <input id="englishName" type="text" class="easyui-validatebox easyui-textbox"
                                name="EnglishName"
                                data-options="required:true,prompt:'请输入英文名称'" maxlength="50" value="<%=model?.EnglishName %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>数据库名称</td>
                        <td>
                            <input id="dataName" type="text" class="easyui-validatebox easyui-textbox"
                                name="DataName"
                                data-options="required:true,prompt:'请输入数据库名称'" maxlength="50" value="<%=model?.DataName %>" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button v_name="submitbtn" v_title="编辑页提交按钮" ID="btnSubmit" runat="server" class="easyui-linkbutton" OnClick="btnSubmit_Click" Text="提交" />
                            <%--<a href="javascript:void(0)" style="width: 100px;"  id="btnsubmit">提交</a>--%>
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
</body>
</html>
