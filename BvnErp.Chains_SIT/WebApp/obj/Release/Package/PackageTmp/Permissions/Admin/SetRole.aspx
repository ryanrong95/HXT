<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetRole.aspx.cs" Inherits="WebApp.Permissions.Admin.SetRole" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>设置角色</title>
<uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var RoleData = eval('(<%=this.Model.RoleData%>)');
        var AdminData = eval('(<%=this.Model.AdminData%>)');
        //数据初始化
        $(function () {            
            //初始化下拉框
            $('#Set').combobox({
                data: RoleData,
            });
            $('#Set').combobox('setValue', AdminData['RoleID']);
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
            data.append('ID', AdminData["ID"]);
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
            <table id="editTable">
                <tr>
                    <td style="padding-left: 20px;">选择角色：</td>
                    <td>
                        <input class="easyui-combobox input" id="Set" style="width:240px;" name="Set" data-options="valueField:'Value',textField:'Text',panelHeight:'80px',required:true,tipPosition:'bottom',missingMessage:'请选择角色'"/>
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