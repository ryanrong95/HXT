<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Deduction.aspx.cs" Inherits="WebApp.Finance.Declare.Deduction" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        var IDs = getQueryString("IDs");

        var AddedValueTaxTotal = '<%=this.Model.AddedValueTaxTotal%>';
        var OrderCount = '<%=this.Model.OrderCount%>';

        //数据初始化
        $(function () {
            $("#AddedValueTaxTotal").html(AddedValueTaxTotal);
            $("#OrderCount").html(OrderCount);
        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }

        function Submit() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var DeductionTime = $("#DeductionTime").datebox("getValue");
            $.post('?action=Submit', {
                DeductionTime: DeductionTime,
                IDs: JSON.stringify(IDs),
            }, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        Close();
                    }
                });
            })
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table style="margin-top:20px;margin-left:50px">             
                <tr style="display: block;">
                    <td>抵扣日期：</td>
                    <td>
                        <input class="easyui-datebox" id="DeductionTime" data-options="required:true" style="height: 26px; width: 200px"" />
                    </td>
                </tr>
                <tr style="display: block; margin-top: 10px;">
                    <td colspan="2">
                        <span style="font-weight: bold;">勾选总条数：</span>
                        <span id="OrderCount"></span>
                        <span style="font-weight: bold; margin-left: 20px;">增值税总金额：</span>
                        <span id="AddedValueTaxTotal"></span>
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Submit()">确认抵扣</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
