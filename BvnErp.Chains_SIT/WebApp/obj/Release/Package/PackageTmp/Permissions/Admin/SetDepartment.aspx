<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetDepartment.aspx.cs" Inherits="WebApp.Permissions.Admin.SetDepartment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>设置部门</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var DepartmentData = eval('(<%=this.Model.DepartmentData%>)');
        var AdminData = eval('(<%=this.Model.AdminData%>)');
        //数据初始化
        $(function () {
            //初始化下拉框
            $('#Set').combobox({
                data: DepartmentData,
            });
            $('#Set').combobox('setValue', AdminData['DepartmentID']);
            $('#IsLeader').prop('checked', AdminData['IsLeader']);
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
                    <td style="padding-left: 20px;">选择部门：</td>
                    <td>
                        <input class="easyui-combobox input" id="Set" style="width: 240px;" name="Set" data-options="valueField:'Key',textField:'Value',panelHeight:'180px',required:true,tipPosition:'bottom',missingMessage:'请选择部门'" />
                    </td>

                </tr>
                <tr>
                    <td style="padding-left: 20px;">
                        <%--<input id="IsLeader" class="easyui-checkbox" name="IsLeader" /><label for="IsLeader" style="margin-right: 30px">是否部门负责人</label>--%>
                        <input type="checkbox" id="IsLeader" name="IsLeader" value="1" /><label for="IsLeader" style="margin-right: 30px">是否部门负责人</label>
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
