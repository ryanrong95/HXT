<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Permissions.Role.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>角色管理</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var tariff = eval('(<%=this.Model.RoleData%>)');
        //数据初始化
        $(function () {
            $('#Name').textbox('setValue', tariff['Name']);
            $('#SysCode').textbox('setValue', tariff['SysCode']);
            $('#Summary').textbox('setValue', tariff['Summary']);
        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            if (tariff != null) {
                data.append('ID', tariff["ID"])
            }
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('提示', res.message, 'info', function () {
                            $.myWindow.close();
                        });
                    }
                }
            }).done(function (res) {
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table style="margin:10px; line-height: 30px">
                <tr>
                    <td>名称：</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name"
                            data-options="required:true,validType:'length[1,200]',tipPosition:'bottom',missingMessage:'请输入名称'" style="width:280px;" />
                    </td>
                </tr>
                <tr>
                    <td>系统代码：</td>
                    <td>
                        <input class="easyui-textbox" id="SysCode" name="SysCode"
                            data-options="validType:'length[1,200]',tipPosition:'bottom',missingMessage:'请输入系统代码'"  style="width:280px;"/>
                    </td>
                </tr>
                <tr>
                    <td>摘要：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary"
                            data-options="required:true,validType:'length[1,200]',tipPosition:'bottom',multiline:true,missingMessage:'请输入摘要'" style="width:280px;height:40px;" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>

