<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LittleReceivedList.aspx.cs" Inherits="WebApp.Finance.Receipt.Store.LittleReceivedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {
            $('#datagrid').myDatagrid({
                nowrap:false,
                border:false,
                fitColumns:true,
                scrollbarSize:0,
                fit:true,
                singleSelect:false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },            
            });
        });

        function TypeNameOperation(val, row, index) {
            var inputs = "";

            if (row.AccountTypeInt == '<%=Yahv.Underly.AccountType.Reduction.GetHashCode()%>') {
                inputs = '减免（' + row.TypeName + '）';
            } else {
                inputs = row.TypeName;
            }

            return inputs;
        }

        function PriceOperation(val, row, index) {
            var inputs = "";

            inputs += '<span style="margin-left: 10px;">';

            if (row.AccountTypeInt == '<%=Yahv.Underly.AccountType.Reduction.GetHashCode()%>') {
                inputs += '-' + row.Price;
                inputs += '<a href="javascript:void(0);" style="margin:3px; color: #134da5; margin-left:15px;" onclick="CancelReduce(\'' + row.ReceivedID + '\')">取消减免</a>';
            } else {
                inputs += row.Price;
            }

            inputs += '</span>';

            return inputs;
        }

        function CancelReduce(ReceivedID) {
            var url = location.pathname + '?action=CancelReduce';
            var params = {
                "ReceivedID": ReceivedID,
            };

            MaskUtil.mask();
            $.post(url, params, function (res) {
                MaskUtil.unmask();
                var resData = JSON.parse(res);
                if (resData.success == "true") {
                    $.messager.alert('提示', '操作成功', 'info', function () {
                        $.myWindow.close();
                    });
                } else {
                    $.messager.alert('提示', resData.message);
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',border:false," style="height: 567px;">
        <table id="datagrid">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10px;">时间</th>
                    <th data-options="field:'ReceiptAdminName',align:'center'" style="width: 8px;">收款人</th>
                    <th data-options="field:'TypeName',align:'center',formatter:TypeNameOperation" style="width: 10px;">费用类型</th>
                    <th data-options="field:'Price',align:'left',formatter:PriceOperation" style="width: 10px;">实收</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
