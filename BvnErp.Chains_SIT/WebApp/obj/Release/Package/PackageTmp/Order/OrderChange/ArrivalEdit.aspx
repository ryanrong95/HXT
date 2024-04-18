<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArrivalEdit.aspx.cs" Inherits="WebApp.Order.OrderChange.ArrivalEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <script type="text/javascript">       
        $(function () {
            var OrderID = getQueryString("OrderID");
            $("#OrderID").val(OrderID);
            $('#products').myDatagrid({
                actionName: 'data',
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
            });
        });

        function Confirm() {
            var OrderID = $("#OrderID").val();
             MaskUtil.mask();
            $.post('?action=Post2Client', { OrderID: OrderID }, function (data) {                
                var result = JSON.parse(data);
                 MaskUtil.unmask();
                $.messager.alert('消息', result.info, 'info', function () {
                    if (result.result) {
                        closeWin();
                    } else {

                    }
                });
            });
        }

        function closeWin() {
            $.myWindow.close();
        }

    </script>
</head>
<body>
    <div class="easyui-panel" style="width: 100%; height: 100%; border: 0px;">
        <div style="margin-left: 5px; margin-top: 10px">
            <label style="font-size: 16px; font-weight: 600; color: orangered">到货变更确认</label>
        </div>
        <div style="text-align: center; height: 40%; margin: 5px;">
            <table id="products">
                <thead>
                    <tr>
                        <th data-options="field:'OrderID',align:'center'" style="width: 15%">订单号</th>
                        <th data-options="field:'Summary',align:'center'" style="width: 85%">变更内容</th>
                    </tr>
                </thead>
            </table>
        </div>

        <div class="sub-container" style="height: 20px;">
            <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Confirm()">确认</a>
            <input type="hidden" id="OrderID" />
        </div>
    </div>
</body>
</html>

