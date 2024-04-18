<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewInvoiceDetail.aspx.cs" Inherits="WebApp.Finance.ReceivableBill.ViewInvoiceDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {

            $('#datagrid').myDatagrid({
                border:false,
                fitColumns:true,
                fit:true,
                scrollbarSize: 0,
                rownumbers: true,
                nowrap: false,
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



    </script>
</head>
<body class="easyui-layout">
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="" data-options="">
            <thead>
                <tr>
                    <th data-options="field:'ProductName',align:'left'" style="width:10%;">品名</th>
                    <th data-options="field:'ProductModel',align:'left'" style="width: 10%;">型号</th>
                    <th data-options="field:'Unit',align:'center'" style="width: 6%;">单位</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 6%;">数量</th>
                    <th data-options="field:'DetailUnitPrice',align:'center'" style="width: 8%;">含税单价</th>
                    <th data-options="field:'DetailAmount',align:'center'" style="width: 8%;">含税金额</th>
                    <th data-options="field:'Name',align:'left'" style="width: 16%;">开票公司</th>
                    <th data-options="field:'InvoiceNo',align:'center'" style="width: 16%;">发票号码</th>
                    <th data-options="field:'UpdateDate',align:'center'" style="width: 8%;">开票日期</th>
                    <th data-options="field:'TaxAmount',align:'center'" style="width: 8%;">税额</th>
                    <%--  <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 50px;">操作</th>--%>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
