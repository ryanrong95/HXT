<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Plat.Roles.Edit" %>

<!DOCTYPE html>

<html >
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>角色编辑</title>
    <uc:EasyUI runat="server"></uc:EasyUI>

     <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '';
        gvSettings.menu = '角色管理';
        gvSettings.summary = '';

    </script>

</head>
<body>

    <div id="p" class="easyui-panel" data-options="border:false" title="角色编辑页" style="width: 100%;">
        <form runat="server">
            <div>
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px;">名称</td>
                        <td>
                            <input runat="server" id="_id" type="hidden" />
                            <input runat="server" id="_name" type="text" class="easyui-validatebox easyui-textbox" data-options="required:true,prompt:'请输入名称'" maxlength="50" />
                        </td>
                    </tr>
                    <tr>
                        <td>备注说明</td>
                        <td>
                            <input runat="server" id="_summary" type="text" class="easyui-textbox" data-options="multiline:true,prompt:'请输入备注说明'" style="width: 300px; height: 100px;" maxlength="300" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btnSubmit" runat="server" class="easyui-linkbutton" OnClick="btnSubmit_Click" Text="提交" />
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
</body>
</html>
