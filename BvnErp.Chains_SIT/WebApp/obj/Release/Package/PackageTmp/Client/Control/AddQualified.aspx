<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddQualified.aspx.cs" Inherits="WebApp.Client.Control.AddQualified" %>

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

        var Clients = eval('(<%=this.Model.Clients%>)');
        $(function () {
            $("#clients").combobox({
                data: Clients
            });

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                // var values = FormValues("form1");
                //  values['ID'] = ID;//会员ID            
                //提交后台
                MaskUtil.mask();//遮挡层
                $.post('?action=SaveQualifiedClient', { ID: $("#clients").combobox("getValue") }, function (res) {
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
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="text-align: center; margin: 40px;">
                <tr>
                    <td class="lbl">非合格报关客户：</td>
                    <td>
                        <input class="easyui-combobox" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:true,missingMessage:'请选择客户'" id="clients" name="clients" style="width: 450px; height: 30px" />
                    </td>
                </tr>

            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'">保存</a>
        <a id="btnReturn" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="closeWin()">取消</a>
    </div>
</body>
</html>
