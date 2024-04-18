<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssignServiceManager.aspx.cs" Inherits="WebApp.Client.Approval.AssignServiceManager" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>指定业务员</title>
      <uc:EasyUI runat="server" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <style type="text/css">
        .lbl {
            text-align: right;
        }
    </style>
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';
        var ServiceManager = eval('(<%=this.Model.ServiceManager%>)');
        var ClientAssignData = eval('(<%=this.Model.ClientAssignData%>)');

        $(function () {
            $("#ServiceManager").combobox({
                data: ServiceManager
            });
            $.each(ClientAssignData, function (index, val) {
                $("#ServiceManager").combobox('setValue', val.Admin.ID);
            });

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                var values = FormValues("form1");
                values['ID'] = ID;//会员ID            
                //提交后台
                MaskUtil.mask();//遮挡层
                $.post('?action=SaveClientAssign', { Model: JSON.stringify(values) }, function (res) {
                    MaskUtil.unmask();//关闭遮挡层
                    var result = JSON.parse(res);
                    $.messager.alert('消息', result.message, "info", function () {
                        if (result.success) {
                            closeWin();
                        }
                    });
                });
            });

            $('#btnReturn').on('click', function () {
                closeWin();
            });
        });

        function closeWin() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content" >
        <form id="form1" runat="server">
            <table id="editTable" style="text-align:center;margin:40px;">
                <tr>
                    <td class="lbl">业务员：</td>
                    <td>
                        <input class="easyui-combobox" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false,missingMessage:'请选择业务员'" id="ServiceManager" name="ServiceManager" style="width: 200px;" />
                    </td>
                </tr>

            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a id="btnReturn" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="closeWin()">取消</a>
    </div>
</body>
</html>




