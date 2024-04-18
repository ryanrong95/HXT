<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Order.OrderChange.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
       $(function () {
            $('#datagrid').myDatagrid({
                nowrap: false,
                fitColumns: true,
                scrollbarSize: 0,
                fit: false,
                singleSelect: true,
                border: true,
                rownumbers: true, //显示行号
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
                    var heightValue = $("#datagrid").prev().find(".datagrid-body").find(".datagrid-btable").height() + 60;
                    $("#datagrid").prev().find(".datagrid-body").height(heightValue);
                    $("#datagrid").prev().height(heightValue);
                    $("#datagrid").prev().parent().height(heightValue);
                    $("#datagrid").prev().parent().parent().height(heightValue + 35);
                },
            });
        });
    </script>
</head>
<body>
    <div title="变更详情" closable="false" style="padding: 10px; width: 100%;">
        <table id="datagrid" class="easyui-datagrid" style="width: 100%;">
            <thead>
                <tr>
                    <th field="ProductModel" data-options="align:'center'" style="width: 100px">型号</th>
                    <th field="Type" data-options="align:'left'" style="width: 50px">变更内容</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 50px">添加时间</th>
                    <th field="Adder" data-options="align:'center'" style="width: 50px">添加人</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
