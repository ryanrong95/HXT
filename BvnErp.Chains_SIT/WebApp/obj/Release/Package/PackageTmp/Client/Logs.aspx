<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logs.aspx.cs" Inherits="WebApp.Client.Logs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //银行列表初始化
            $('#datagrid').myDatagrid({
                fitColumns: true,
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
                }
            });
        });

    </script>
</head>
<body class="easyui-layout">
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" data-options="singleSelect:true,fit:true,scrollbarSize:0,border:false" class="easyui-datagrid" style="width: 100%; height: 80%"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'Admin',align:'left'" style="width: 100px;">操作人</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 100px;">创建时间</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 260px;">摘要</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
