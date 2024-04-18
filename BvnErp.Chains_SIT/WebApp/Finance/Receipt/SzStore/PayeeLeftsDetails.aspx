<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayeeLeftsDetails.aspx.cs" Inherits="WebApp.Finance.Receipt.SzStore.PayeeLeftsDetails" %>

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
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'center',border:false,">
        <table id="datagrid">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10px;">收款时间</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 10px;">收款人</th>
                    <th data-options="field:'Price',align:'center'" style="width: 10px;">金额</th>
                    <th data-options="field:'FlowFormCode',align:'center'" style="width: 10px;">流水号</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
