<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VoucherDetail.aspx.cs" Inherits="WebApp.Finance.Receipt.Order.VoucherDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>抵用券详情</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var FinanceReceiptId = '<%=this.Model.FinanceReceiptId%>';

        $(function () {
            $('#VoucherDetail').datagrid({
                url: "?action=data&FinanceReceiptId=" + FinanceReceiptId,
                nowrap:false,
                border:false,
                fitColumns:true,
                scrollbarSize:0,
                
                singleSelect:false,
                rownumbers:true,
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
                onLoadSuccess: function (data) {
                    
                },
            });
        });
    </script>
</head>
<body>
    <div>
        <div data-options="region:'center',border:false">
            <table id="VoucherDetail" data-options="toolbar:'#topBar'">
                <thead>
                    <tr>
                        <th data-options="field:'FinanceVoucherID',align:'center'" style="width: 120px;">抵用券</th>
                        <th data-options="field:'Amount',align:'center'" style="width: 80px;">面值</th>
                        <th data-options="field:'UseTime',align:'center'" style="width: 80px;">使用时间</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</body>
</html>
